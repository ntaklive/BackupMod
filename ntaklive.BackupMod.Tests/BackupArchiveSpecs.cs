using System;
using System.IO;
using ntaklive.BackupMod.Domain;
using Xunit;

namespace ntaklive.BackupMod.Tests
{
    public class BackupArchiveSpecs
    {
        [Fact] 
        public void Throw_IfFilepathIsNull_Success()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ConstructBackupArchiveObject(null));
        }
    
        [Fact]
        public void Check_IfTwoObjectsAreEqual_True()
        {
            string filepath = Path.Combine($"{Guid.NewGuid().ToString().Replace("-", "")}.zip");
            File.WriteAllBytes(filepath, Array.Empty<byte>());
        
            var obj1 = new BackupArchive(filepath);
            var obj2 = new BackupArchive(filepath);

            File.Delete(filepath);
        
            Assert.True(obj1 == obj2);
        }

        [Fact]
        public void Check_IfTwoObjectsAreNotEqual_True()
        {
            string filepath1 = Path.Combine($"{Guid.NewGuid().ToString().Replace("-", "")}.zip");
            File.WriteAllBytes(filepath1, Array.Empty<byte>());
        
            string filepath2 = Path.Combine($"{Guid.NewGuid().ToString().Replace("-", "")}.zip");
            File.WriteAllBytes(filepath2, Array.Empty<byte>());
        
            var obj1 = new BackupArchive(filepath1);
            var obj2 = new BackupArchive(filepath2);
        
            File.Delete(filepath1);
            File.Delete(filepath2);
        
            Assert.True(obj1 != obj2);
        }

        private void ConstructBackupArchiveObject(
            string? filepath)
        {
            _ = new BackupArchive(filepath);
        }
    }
}