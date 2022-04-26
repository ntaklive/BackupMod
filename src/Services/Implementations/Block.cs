using NtakliveBackupMod.Services.Abstractions;

namespace NtakliveBackupMod.Services.Implementations;

public class Block : IBlock
{
    public NameIdMapping GetNameIdMapping()
    {
        return global::Block.nameIdMapping;
    }
}