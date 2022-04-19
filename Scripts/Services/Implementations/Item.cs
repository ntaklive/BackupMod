using NtakliveBackupMod.Scripts.Services.Abstractions;

namespace NtakliveBackupMod.Scripts.Services.Implementations;

public class Item : IItem
{
    public NameIdMapping GetNameIdMapping()
    {
        return ItemClass.nameIdMapping;
    }
}