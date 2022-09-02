using BackupMod.Manifest;
using BackupMod.Services.Abstractions;
using BackupMod.Services.Abstractions.Filesystem;
using Microsoft.Extensions.Configuration;

namespace BackupMod.Services;

public class BackupManifestService : IBackupManifestService
{
    private readonly IFileService _fileService;
    private readonly IJsonSerializer _jsonSerializer;

    public BackupManifestService(
        IFileService fileService,
        IJsonSerializer jsonSerializer)
    {
        _fileService = fileService;
        _jsonSerializer = jsonSerializer;
    }
    
    public BackupManifest ReadManifest(string filepath)
    {
        IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddJsonFile(filepath).Build();
        var manifest = configurationRoot.Get<BackupManifest>();

        manifest.Filepath = filepath;
        
        return manifest;
    }

    public void CreateManifest(string filepath, BackupManifest manifest)
    {
        string json = _jsonSerializer.Serialize(manifest);
        _fileService.WriteAllText(filepath, json);
    }
}