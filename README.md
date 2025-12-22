### Welcome to the Chronicles of the Omuns Repository!
This repository is the brain child of Derpipose and her husband KrystalHydralisk
This contains the current character builder for the Omuns RPG system, as well as a few other tools to help with gameplay.
Feel free to explore the code, contribute, or use it for your own Omuns adventures!

### Current status: In Development and not ready for release yet
The character builder is still being worked on, and there are many features yet to be added. Stay tuned for updates!
Current dev timeline is that I hope to have a working version by the end of 2025, christmas break for the player side. 
May 2026 for the Game Master, or Chronicler, side. 
Both will have local databases at first, with online databases to be added later.

The current dev plan can be found: https://docs.google.com/document/d/1-_hxALgBB9ue5jpOuSIECTwRzIaqAatZP465uhR4I44/edit?usp=sharing

### How to Get the App Running for Devs

#### Prerequisites
- .NET 10.0 SDK installed
- Visual Studio 2022 or VS Code with C# extensions
- SQL Server LocalDB (typically included with Visual Studio)

#### Setting Up the Database

1. **Open PowerShell** and navigate to the PlayerApp directory:
   ```powershell
   cd PlayerApp
   ```

2. **Apply Entity Framework migrations**:
   ```powershell
   dotnet ef database update
   ```
   This will create the LocalDB instance and apply all migrations to initialize the database schema.

3. **Build and run the app**:
   ```powershell
   dotnet run
   ```

#### First Time Setup in the App

When the app first loads, you'll see the Dashboard. To fully populate the database with classes and races:

1. Click **"Load All Classes"** - This fetches D&D classes from the remote JSON API and stores them in the database
2. Click **"Load All Races"** - This fetches races from the remote JSON API and stores them in the database

After this, you're ready to start creating characters!

#### Database Details
- **Connection String**: `Server=(localdb)\ChroniclesDB;Database=chronicles_of_omuns;Trusted_Connection=true`
- **Dev Provider**: SQL Server (Debug configuration)
- **Release Provider**: SQLite (Production configuration)
- **Migrations**: Auto-applied on app startup from `PlayerApp/Migrations/`

