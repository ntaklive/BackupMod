using BackupMod.Services.Abstractions.Game;

namespace BackupMod.Services.Game;

public class Item : IItem
{
    public NameIdMapping GetNameIdMapping()
    {
        return ItemClass.nameIdMapping;
    }
}