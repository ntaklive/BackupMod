using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class Block : IBlock
{
    public NameIdMapping GetNameIdMapping()
    {
        return global::Block.nameIdMapping;
    }
}