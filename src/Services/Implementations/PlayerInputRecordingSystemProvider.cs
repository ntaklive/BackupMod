using NtakliveBackupMod.Scripts.Services.Abstractions;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

public class PlayerInputRecordingSystemProvider : IPlayerInputRecordingSystemProvider
{
    public PlayerInputRecordingSystem GetPlayerInputRecordingSystem()
    {
        return PlayerInputRecordingSystem.Instance;
    }
}