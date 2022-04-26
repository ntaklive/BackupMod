# BackupMod
A 7 days to die modlet which makes it possible to automatically backup your game saves

# Description
This modlet makes it possible to automatically backup your game saves.

# Settings

You can configure the modlet by modifying the 'settings.json' file.
If you changed the configuration when you were in game, you should re-enter to your world to apply changed settings.
By default, your backups for each save will be saved in the '%AppData%\Roaming\7DaysToDie\Saves\\*WorldName*\Backups\\*SaveName*' folder.

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
'**backup**' or '**bp**' - perform a forceful backup

# Installation
1. Extract the downloaded .zip archive
2. Move the extracted 'BackupMod' folder to your 'Mods' folder

# Compatibility
Required game version: Alpha 20

# EAC Compatibility
Server only. To load in single-player mode, you must disable EAC.

# Links
[community.7daystodie.com](https://community.7daystodie.com/topic/28451-backup-mod/)
[7daystodiemods.com](https://7daystodiemods.com/backup-mod/)
