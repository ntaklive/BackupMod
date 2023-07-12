using BackupMod.Services.Abstractions.Game;

namespace BackupMod.Services.Game;

public class Block : IBlock
{
    public NameIdMapping GetNameIdMapping()
    {
        return global::Block.nameIdMapping;
    }
}