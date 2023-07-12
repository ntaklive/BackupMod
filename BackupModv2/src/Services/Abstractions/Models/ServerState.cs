using BackupMod.Services.Abstractions.Enum;

namespace BackupMod.Services.Abstractions.Models;

public struct ServerState
{
    public ServerState()
    {
        AccessibilityState = ServerAccessibilityState.Unknown;
        FillingState = ServerFillingState.Unknown;
        PlayersCount = 0;
    }
    
    public ServerAccessibilityState AccessibilityState { get; set; }

    public ServerFillingState FillingState { get; set; }

    public int PlayersCount { get; set; }
}