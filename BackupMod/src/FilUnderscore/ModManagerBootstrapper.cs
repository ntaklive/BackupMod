namespace BackupMod.FilUnderscore;

/// <summary>
/// FilUnderscore`s ModManager support <br/>
/// Important! Initialize the mod manager settings after the Bootstrapper.Initialize() method was invoked
/// </summary>
public static class ModManagerBootstrapper
{
    private static int _test123 = 0; // Note: This value will be the default setting when reset.
    
    public static void Initialize(Mod modInstance)
    {
        
        if (!ModManagerAPI.IsModManagerLoaded()) return;
        
        ModManagerAPI.ModSettings modSettings = ModManagerAPI.GetModSettings(modInstance);
            
        modSettings.Hook(
            "test123",                                     // This is the Mod Setting key. This must always be unique for all settings for the mod.
            "xuiModSettingTest123",                        // This is the localization key for the setting's label. The value is fetched from the Localization.txt file in your mod's Config folder.
            value => _test123 = value,                 // This is the value setter, it updates the value of the variable we have hooked onto.
            () => _test123,                            // This is the value getter, it gets the value of the variable we have hooked onto.
            toStr => (toStr.ToString(), toStr.ToString()), // This is the string representation of the currently applied setting.
            str =>                                         // This is the converter from the String representation of the setting, back to the variable type of the setting.
            {
                bool success = int.TryParse(str, out int val); // Attempt to convert the input to an integer. 
                // If success is true, the input will be accepted.
                // If success is false, the input will be rejected, and the text will change to red to indicate
                // to the user that an invalid input has been entered and will not be saved.
                return (val, success);
            });
    }
}