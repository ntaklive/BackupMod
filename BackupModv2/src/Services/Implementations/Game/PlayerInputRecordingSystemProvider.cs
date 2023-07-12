using BackupMod.Services.Abstractions.Game;

namespace BackupMod.Services.Game;

public class PlayerInputRecordingSystemProvider : IPlayerInputRecordingSystemProvider
{
    public PlayerInputRecordingSystem GetPlayerInputRecordingSystem()
    {
        return PlayerInputRecordingSystem.Instance;
    }
}