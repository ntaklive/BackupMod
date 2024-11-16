using System;
using System.Text;

namespace BackupMod.Utilities;

public static class Md5HashHelper
{
    public static string ComputeTextHash(string text)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        
        string hash = BitConverter.ToString(
            md5.ComputeHash(Encoding.UTF8.GetBytes(text))
        ).Replace("-", string.Empty);

        return hash;
    }
}