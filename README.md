# BackupMod
![BackupModDemonstrationScreenV1.0.0](docs/demo.png)

A 7 days to die modlet which makes it possible to automatically backup and restore your game saves

# Settings

You can configure the modlet by modifying the 'settings.json' file.
If you changed the configuration when you were in game, you should re-enter to your world to apply changed settings.
By default, your backups for each save will be saved in the '%AppData%/Roaming/7DaysToDie/Backups/%WorldName%/%SaveName%' folder.

```
// World backup files limit.
// Default: 5
"BackupsLimit" : 5,

// Auto backup delay (seconds).
// Default: 600 (10 minutes)
"AutoBackupDelay" : 600,

// Enable in-chat notifications.
// Default: true
"EnableChatMessages" : true,

// Custom backups folder path.
// IMPORTANT: You must escape all '\' characters or use '/'
// Example: "C:\\Backups".
// Default: ""
"CustomBackupsFolder" : "",

// Backup when entering the world
// Default: true
"BackupOnWorldLoaded" : true
```

# Commands
You can use the '**backup**' command or its shortened version - the '**bp**' command

'**backup**' - perform a forceful backup  
'**backup info**' - show the current configuration of the mod  
'**backup restore**' - restore a save from a backup

# Installation
1. Extract the downloaded .zip archive
2. Move the extracted 'BackupMod' folder to your 'Mods' folder

# Compatibility
Required game version: Alpha 20

# EAC Compatibility
Server only. To load in single-player mode, you must disable EAC.

# Links
[Official] [community.7daystodie.com](https://community.7daystodie.com/topic/28451-backup-mod/)  
[Unofficial] [7daystodiemods.com](https://7daystodiemods.com/backup-mod/)

# Support
[!["Buy Me A Coffee"](docs/buymeacoffee.svg)](https://www.buymeacoffee.com/ntaklive)
