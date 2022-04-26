using NtakliveBackupMod.Services.Abstractions;

namespace NtakliveBackupMod.Services.Implementations;

public class Item : IItem
{
    public NameIdMapping GetNameIdMapping()
    {
        return ItemClass.nameIdMapping;
    }
}