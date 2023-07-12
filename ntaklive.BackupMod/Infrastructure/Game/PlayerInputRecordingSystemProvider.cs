namespace ntaklive.BackupMod.Infrastructure.Game
{
    public class PlayerInputRecordingSystemProvider : IPlayerInputRecordingSystemProvider
    {
        public PlayerInputRecordingSystem GetPlayerInputRecordingSystem()
        {
            return PlayerInputRecordingSystem.Instance;
        }
    }
}