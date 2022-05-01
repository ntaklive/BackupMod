using System.IO;
using BackupMod.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BackupMod.DI;

public static class FoldersBootstrapper
{
    public static void CreateRequiredFolders(IServiceCollection services)
    {
        ServiceProvider resolver = services.BuildServiceProvider();

        var directories = resolver.GetRequiredService<IGameDirectoriesProvider>();

        string backupsFolderPath = directories.GetBackupsFolderPath();
        if (!new DirectoryInfo(backupsFolderPath).Exists)
        {
            new DirectoryInfo(backupsFolderPath).Create();
        }
    }
}