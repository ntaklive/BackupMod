using System;
using System.Threading;
using System.Threading.Tasks;
using BackupMod.Services.Abstractions.Models;

namespace BackupMod.Services.Abstractions;

public interface IServerStateWatcher
{
    public event EventHandler<ServerState> StateUpdate;  

    public Task StartAsync(CancellationToken token);
    
    public ServerState GetCurrentServerState();
}