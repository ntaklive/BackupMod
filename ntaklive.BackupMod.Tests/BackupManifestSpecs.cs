using System.Collections.Generic;
using System.Reflection;
using ntaklive.BackupMod.Application.Base;
using ntaklive.BackupMod.Domain;
using Xunit;

namespace ntaklive.BackupMod.Tests
{
    public class BackupManifestSpecs
    {
        [Fact]
        public void Map_AllProperties_Success()
        {
            Backup backup = Mocks.Backup;

            BackupManifest backupManifest = BackupManifest.Empty.Map(backup);

            IEnumerable<PropertyInfo> backupDtoProperties = backupManifest.GetType().GetRuntimeProperties();
            backupDtoProperties.ForEach(property => Assert.True(property.GetValue(backupManifest) != null));
        }
    }
}