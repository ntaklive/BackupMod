using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class PlayerInputRecordingSystemProvider : IPlayerInputRecordingSystemProvider
{
    public PlayerInputRecordingSystem GetPlayerInputRecordingSystem()
    {
        return PlayerInputRecordingSystem.Instance;
    }
}