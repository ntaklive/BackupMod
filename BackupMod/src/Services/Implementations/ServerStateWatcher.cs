using System;
using System.Threading;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Enum;
using BackupMod.Services.Abstractions.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace BackupMod.Services;

public class ServerStateWatcher : IServerStateWatcher
{
    private readonly TimeSpan _updateRate;
    private readonly IWorldService _worldService;
    private readonly ILogger<ServerStateWatcher> _logger;
    private readonly int _maxPlayersCount;

    [CanBeNull] private CancellationTokenSource _cts;
    private ServerState _state;

    public ServerStateWatcher(
        TimeSpan updateRate,
        IWorldService worldService,
        ILogger<ServerStateWatcher> logger)
    {
        _worldService = worldService;
        _logger = logger;
        _updateRate = updateRate;

        _maxPlayersCount = _worldService.GetMaxPlayersCount();
    }

    public event EventHandler<ServerState> StateUpdate;

    public async Task StartAsync(CancellationTokenSource cts)
    {
        if (_cts != null)
        {
            throw new InvalidOperationException("The watcher is already started");
        }
        
        _cts = new CancellationTokenSource();
        
        _state.AccessibilityState = ServerAccessibilityState.Unknown;
        _state.FillingState = ServerFillingState.Empty;

        while (!cts.IsCancellationRequested)
        {
            UpdateAccessibilityState();
            UpdateFillingState();

            try
            {
                await Task.Delay(_updateRate, cts.Token);
            }
            catch (TaskCanceledException)
            {
                // ignored
            }
        }
        
        _cts!.Cancel();
    }

    public ServerState GetServerState()
    {
        ThrowIfNotStarted();

        return _state;
    }

    private void UpdateAccessibilityState()
    {
        bool worldAccessible = _worldService.IsWorldAccessible();
        if (worldAccessible && _state.AccessibilityState != ServerAccessibilityState.Accessible)
        {
            _state.AccessibilityState = ServerAccessibilityState.Accessible;
            OnStateUpdate(_state);
        }
        else if (!worldAccessible && _state.AccessibilityState != ServerAccessibilityState.Inaccessible)
        {
            _state.AccessibilityState = ServerAccessibilityState.Inaccessible;
            OnStateUpdate(_state);
        }
    }

    private void UpdateFillingState()
    {
        int playersCount = _worldService.GetPlayersCount();
        if (playersCount == 0 && _state.FillingState != ServerFillingState.Empty)
        {
            _state.FillingState = ServerFillingState.Empty;
            _state.PlayersCount = playersCount;
            OnStateUpdate(_state);
        }
        else if (playersCount == _maxPlayersCount && _state.FillingState != ServerFillingState.Full)
        {
            _state.FillingState = ServerFillingState.Full;
            _state.PlayersCount = playersCount;
            OnStateUpdate(_state);
        }
        else if (playersCount != 0 && playersCount != _maxPlayersCount && playersCount != _state.PlayersCount)
        {
            _state.FillingState = ServerFillingState.HasPlayers;
            _state.PlayersCount = playersCount;
            OnStateUpdate(_state);
        }
    }

    protected virtual void OnStateUpdate(ServerState state)
    {
        _logger.LogTrace("Server state changed: {@State}", state);
        StateUpdate?.Invoke(this, state);
    }

    private void ThrowIfNotStarted()
    {
        if (_cts == null)
        {
            throw new InvalidOperationException("The watcher is not started");
        }
    }
}