# BackupMod
![BackupModDemonstrationScreenV1.1.2](docs/demo.png)

A 7 days to die modlet which makes it possible to automatically backup and restore your game saves.

# Settings
You can configure the modlet by modifying the 'settings.json' file, or you can also use [FilUnderscore's Mod Manager](https://github.com/FilUnderscore/ModManager/releases).  
If you changed the configuration when you were in game, you should re-enter your world to apply the changed settings.

```
{
  "General": {
    // Backups limit
    // Default: 10
    "BackupsLimit": 10,
    
    // Custom backups folder path
    // IMPORTANT: You must escape all '\' characters or use '/'
    // Example: "C:\\7DaysToDie\\Backups"
    // Default: ""
    "CustomBackupsFolder": "",
    
    // Write debug information to the logs/log.txt file and to the in-game/server console
    // Default: false
    "DebugMode": false
  },
  
  "AutoBackup": {
    // Is automatic backup enabled
    // Default: true
    "Enabled": true,
    
    // Auto backup delay (seconds)
    // Min: 10
    // Default: 1200 (20 minutes)
    "Delay": 1200,
    
    // Reset the delay timer after a manual backup
    // Default: false
    "ResetDelayTimerAfterManualBackup": false,

    // Skip the next scheduled backup, if there are no players on the server
    // Default: false
    "SkipIfThereAreNoPlayers": false
  },
  
  "Archive": {
    // Keep the last backup of the day
    // Default: false
    "Enabled": false,

    // Backups limit
    // Default: 10
    "BackupsLimit": 10,
    
    // Custom archive folder path.
    // IMPORTANT: You must escape all '\' characters or use '/'
    // Example: "C:\\7DaysToDie\\Archive".
    // Default: ""
    "CustomArchiveFolder": ""
  },
  
  "Events": {
    // Backup when world loaded
    // Default: true
    "BackupOnWorldLoaded": true,

    // Backup when the last player on the server disconnected
    // Default: false
    "BackupOnServerIsEmpty": false
  },
  
  "Notifications": {
    // Are ALL chat notifications enabled
    // Default: true
    "Enabled": true,

    "Countdown" : {
      // Are countdown chat notifications enabled
      // Default: true
      "Enabled": true,
      
      // Do a countdown * seconds before backup starts
      // Min: 1
      // Default: 5
      "CountFrom": 5
    }
  }
}
```
By default, your backups for each save will be saved in the '**%AppData%/7DaysToDie/Backups/%WorldName%/%SaveName%**' directory (on windows).  
and your archived backups for each save will be saved in the '**%AppData%/7DaysToDie/Archive/%WorldName%/%SaveName%**' folder (on windows).  

P.S. You can also change the **UserGameData** property in your serverconfig.xml or in the game startup arguments. Then by default it will use these directories:
'**%UserGameData%/Backups/%WorldName%/%SaveName%**' for backups, and  
'**%UserGameData%/Archive/%WorldName%/%SaveName%**' for archive

# Commands
You can use the '**backup**' command or its shortened version - the '**bp**' command:

| Command        | Explanation                                                |
| ---            | ---                                                        |
| backup         | Perform an immediate backup                                |
| backup info    | Show the current configuration of this mod                 |
| backup list    | Show all available backups                                 |
| backup restore | Restore a save from a backup                               |
| backup delete  | Delete a backup                                            |
| backup start   | Start an auto-backup (even if disabled in `settings.json`) |
| backup stop    | Stop the auto-backup currently in progress                 |

# Installation
1. Extract the downloaded .zip archive
2. Move the extracted 'BackupMod' folder to your 'Mods' folder

# Compatibility
Required game version: V1.1+

# EAC Compatibility
Server only. To load in single-player mode, you must disable EAC.

# Links
[Official] [Official TFP 7 Days To Die Forum](https://community.7daystodie.com/topic/28451-backup-mod/)  
[Official] [Nexus Mods](https://www.nexusmods.com/7daystodie/mods/2210)  
[Unofficial] [7daystodiemods.com](https://7daystodiemods.com/backup-mod/)

# Support
Coin: USDT\
Network: BSC - BNB Smart Chain (BEP20)\
Address: 0xb7a5bf6e55739aaf848a0a54161a45e9b580a5a2
