namespace ntaklive.BackupMod.Infrastructure.Game
{
    public class Block : IBlock
    {
        public NameIdMapping? GetNameIdMapping()
        {
            return global::Block.nameIdMapping;
        }
    }
}