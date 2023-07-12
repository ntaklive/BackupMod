namespace ntaklive.BackupMod.Abstractions;

public interface IMappable<in TFrom, out TTo>
    where TFrom : class
    where TTo : class
{
    public TTo Map(TFrom obj);
}