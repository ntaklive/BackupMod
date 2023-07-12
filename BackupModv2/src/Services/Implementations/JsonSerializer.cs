using System.Text.Json;
using BackupMod.Services.Abstractions;

namespace BackupMod.Services;

public class JsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _serializerOptions;

    public JsonSerializer(System.Text.Json.JsonSerializerOptions serializerOptions)
    {
        _serializerOptions = serializerOptions;
    }

    public string Serialize<T>(T obj)
    {
        return System.Text.Json.JsonSerializer.Serialize(obj, _serializerOptions);
    }

    public T Deserialize<T>(string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json, _serializerOptions)!;
    }
}