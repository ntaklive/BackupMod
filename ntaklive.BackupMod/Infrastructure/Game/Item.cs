namespace ntaklive.BackupMod.Infrastructure.Game
{
    public class Item : IItem
    {
        public NameIdMapping? GetNameIdMapping()
        {
            return ItemClass.nameIdMapping;
        }
    }
}