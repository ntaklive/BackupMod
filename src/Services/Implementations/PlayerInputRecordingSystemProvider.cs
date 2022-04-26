using NtakliveBackupMod.Services.Abstractions;

namespace NtakliveBackupMod.Services.Implementations;

public class PlayerInputRecordingSystemProvider : IPlayerInputRecordingSystemProvider
{
    public PlayerInputRecordingSystem GetPlayerInputRecordingSystem()
    {
        return PlayerInputRecordingSystem.Instance;
    }
}