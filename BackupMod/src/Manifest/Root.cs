using System.Text.Json.Serialization;

namespace BackupMod.Manifest;

public class BackupManifest
{
    public BackupManifest()
    {
    }

    [JsonIgnore] public string Filepath { get; set; }
    public string BackupFilename { get; set; }
    public string Title { get; set; }
    public WorldManifestPart World { get; set; }
    public SaveManifestPart Save { get; set; }
    public CreationTimeManifestPart CreationTime { get; set; }
    public AdditionalInfoManifestPart AdditionalInfo { get; set; }
}