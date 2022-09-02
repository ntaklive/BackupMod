using BackupMod.Services.Abstractions.Enum;

namespace BackupMod.Services.Abstractions.Models;

public struct ServerState
{
    public ServerAccessibilityState AccessibilityState { get; set; }

    public ServerFillingState FillingState { get; set; }

    public int PlayersCount { get; set; }
}