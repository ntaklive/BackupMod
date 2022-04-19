using NtakliveBackupMod.Scripts.Services.Abstractions;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

public class Block : IBlock
{
    public NameIdMapping GetNameIdMapping()
    {
        return global::Block.nameIdMapping;
    }
}