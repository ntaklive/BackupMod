using System.IO;
using BackupMod.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BackupMod.DI;

public static class FoldersBootstrapper
{
    public static void CreateRequiredFolders(IServiceCollection services)
    {
        ServiceProvider resolver = services.BuildServiceProvider();

        var directories = resolver.GetRequiredService<IGameDirectoriesService>();

        string backupsFolderPath = directories.GetBackupsFolderPath();
        var backupsFolder = new DirectoryInfo(backupsFolderPath);
        if (!backupsFolder.Exists)
        {
            backupsFolder.Create();
        }
        
        string archiveFolderPath = directories.GetArchiveFolderPath();
        var archiveFolder = new DirectoryInfo(archiveFolderPath);
        if (!archiveFolder.Exists)
        {
            archiveFolder.Create();
        }
    }
}