namespace BackupMod.Services.Abstractions;

public interface IJsonSerializer
{
    public string Serialize<T>(T obj);
    public T Deserialize<T>(string json);
}