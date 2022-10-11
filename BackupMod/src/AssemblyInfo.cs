using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Backup Mod")]
[assembly: AssemblyDescription("This modlet makes it possible to automatically backup and restore your game saves")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ntaklive")]
[assembly: AssemblyProduct("ntaklive`s Backup Mod")]
[assembly: AssemblyCopyright("Copyright ©  2022")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("4166C0E5-89BD-4F24-99B6-06B8FD5D008A")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.1.2")]
[assembly: AssemblyFileVersion("2.1.2")]

namespace BackupMod;

public static class AssemblyInfo
{
    public static string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString()[..^2];
    
    public static string[] Authors => new []
    {
        "ntaklive"
    };

    public static string AssemblyDirectoryPath
    {
        get
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}