using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class Item : IItem
{
    public NameIdMapping GetNameIdMapping()
    {
        return ItemClass.nameIdMapping;
    }
}