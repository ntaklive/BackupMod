using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Game;
using BackupMod.Services.Abstractions.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace BackupMod.Services;

public class AutoBackupProcess : IAutoBackupProcess
{
    private readonly ModConfiguration _configuration;
    private readonly IBackupManager _backupManager;
    private readonly IServerStateWatcher _serverStateWatcher;
    private readonly IWorldSaveAlgorithm _worldSaveAlgorithm;
    [CanBeNull] private readonly IChatService _chatService;
    private readonly ILogger<AutoBackupProcess> _logger;

    private CancellationTokenSource _stateWatcherCts;
    private CancellationTokenSource _timerCts;
    private readonly TimeSpan _delay;

    private ServerState _currentServerState;

    public AutoBackupProcess(
        ModConfiguration configuration,
        IBackupManager backupManager,
        IServerStateWatcher serverStateWatcher,
        IWorldSaveAlgorithm worldSaveAlgorithm,
        [CanBeNull] IChatService chatService,
        ILogger<AutoBackupProcess> logger)
    {
        _configuration = configuration;
        _backupManager = backupManager;
        _serverStateWatcher = serverStateWatcher;
        _worldSaveAlgorithm = worldSaveAlgorithm;
        _chatService = chatService;
        _logger = logger;
        
        _delay = TimeSpan.FromSeconds(_configuration.AutoBackup.Delay);
        if (_configuration.Notifications.Countdown.Enabled)
        {
            _delay = _delay.Subtract(TimeSpan.FromSeconds(_configuration.Notifications.Countdown.CountFrom));
        }
    }

    public async Task StartAsync(CancellationToken token)
    {
        _timerCts = CancellationTokenSource.CreateLinkedTokenSource(token);
        _stateWatcherCts = CancellationTokenSource.CreateLinkedTokenSource(token);

        async void OnServerStateWatcherOnStateUpdate(object sender, ServerState state) => await ServerStateWatcherOnStateUpdate(state);

        try
        {
            Task watcherTask = _serverStateWatcher.StartAsync(_stateWatcherCts.Token);

            _serverStateWatcher.StateUpdate += OnServerStateWatcherOnStateUpdate;

            _logger.LogInformation("AutoBackup process started");
            await Task.Delay(_delay, _timerCts.Token);
            
            TimeSpan previousBackupSecondsElapsed = TimeSpan.Zero;
            while (!_stateWatcherCts.IsCancellationRequested)
            {
                try
                {
                    TimeSpan delay = _delay - previousBackupSecondsElapsed;
                    if (delay < TimeSpan.Zero)
                    {
                        delay = TimeSpan.Zero;
                    }
                    await Task.Delay(delay, _timerCts.Token);
                    previousBackupSecondsElapsed = TimeSpan.Zero;
                }
                catch (TaskCanceledException)
                {
                    _timerCts = new CancellationTokenSource();
                    continue;
                }

                ServerState serverState = _serverStateWatcher.GetCurrentServerState();
                if (_configuration.AutoBackup.SkipIfThereAreNoPlayers &&
                    serverState.FillingState == ServerFillingState.Empty)
                {
                    _logger.LogInformation("There are no players on the server. The current backup has been skipped");

                    continue;
                }
                
                _stateWatcherCts.Token.ThrowIfCancellationRequested();

                if (_configuration.Notifications.Countdown.Enabled)
                {
                    await ChatCountdown(_stateWatcherCts.Token);
                }

                var backupStopwatch = Stopwatch.StartNew();
                await BackupAsync(_stateWatcherCts.Token);
                backupStopwatch.Stop();
                previousBackupSecondsElapsed = backupStopwatch.Elapsed;
            }
            
            if (watcherTask.Status != TaskStatus.RanToCompletion)
            {
                _stateWatcherCts.Cancel();
                _timerCts.Cancel();
                await watcherTask;
            }
        }
        catch (TaskCanceledException) 
        {
            // ignored
        }
        finally
        {
            _serverStateWatcher.StateUpdate -= OnServerStateWatcherOnStateUpdate;
            
            _logger.LogInformation("AutoBackup process terminated");
        }
    }

    public void ResetDelayTimer()
    {
        _timerCts.Cancel();
    }

    private async Task ServerStateWatcherOnStateUpdate(ServerState state)
    {
        try
        {
            if (state.AccessibilityState == ServerAccessibilityState.Inaccessible)
            {
                _stateWatcherCts.Cancel();
                _timerCts.Cancel();
                return;
            }
            
            if (_configuration.Events.BackupOnServerIsEmpty &&
                _currentServerState.FillingState != ServerFillingState.Empty &&
                state.FillingState == ServerFillingState.Empty)
            {
                _logger.LogInformation("The server is empty. Performing a backup");
                
                await BackupAsync(_stateWatcherCts.Token);
            }
        }
        catch (TaskCanceledException)
        {
            // ignored
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An event cannot be handled properly");
        }
        finally
        {
            _currentServerState = state;
        }
    }

    private async Task ChatCountdown(CancellationToken token)
    {
        if (_chatService == null)
        {
            return;
        }
        
        for (int i = _configuration.Notifications.Countdown.CountFrom; i > 0; i--)
        {
            token.ThrowIfCancellationRequested();
            
            _chatService.SendMessage($"Backup in {i}...");
            await Task.Delay(TimeSpan.FromSeconds(1), token);
        }
    }

    private async Task BackupAsync(CancellationToken token)
    {
        const int maxRetries = 5;
        const int retryDelaySeconds = 10;
        TimeSpan beforeBackupDelay = TimeSpan.FromSeconds(3);

        _logger.LogInformation("The world backup is starting...");
        _chatService?.SendMessage("The world backup is starting...");
        
        _worldSaveAlgorithm.Save();
        
        await Task.Delay(beforeBackupDelay, token);
        
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                (BackupInfo backupInfo, TimeSpan timeElapsed) result =
                    await _backupManager.CreateAsync("Automatic backup", token);

                string nextBackupTime = DateTime.Now.Add(_delay - result.timeElapsed + beforeBackupDelay).ToShortTimeString();

                _logger.LogInformation("The world backup has completed successfully!");
                _logger.LogInformation($"Time spent: {(beforeBackupDelay + result.timeElapsed).TotalSeconds:F2} seconds");
                _logger.LogInformation($"The backup file location: \"{result.backupInfo.Filepath}\"");
                _logger.LogInformation($"The next backup will be at {nextBackupTime}");

                _chatService?.SendMessage("The world backup has completed successfully!");
                _chatService?.SendMessage($"Time spent: {(beforeBackupDelay + result.timeElapsed).TotalSeconds:F2} seconds");
                _chatService?.SendMessage($"The next backup will be at {nextBackupTime}");
                
                break;
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
            catch (Exception ex)
            {
                if (attempt == maxRetries)
                {
                    throw;
                }

                _logger.LogInformation($"Attempt {attempt} failed: {ex.Message}. Retrying in {retryDelaySeconds} seconds...");
                _chatService?.SendMessage($"Attempt {attempt} failed: {ex.Message}. Retrying in {retryDelaySeconds} seconds...");
                    
                await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds), token);
            }
        }
    }
}