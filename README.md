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

#### Setting Up the Application

1. **Open PowerShell** and navigate to the PlayerAppBlazor directory:
   ```powershell
   cd PlayerAppBlazor
   ```

2. **Apply Entity Framework migrations**:
   ```powershell
   dotnet ef database update
   ```
   This will create the SQLite database at `%LOCALAPPDATA%\TheChroniclesOfTheOmuns\chronicles.db` and apply all migrations.

3. **Build and run the app**:
   ```powershell
   dotnet run
   ```
   The app will be available at `https://localhost:5001`

#### First Time Setup in the App

When the app first loads, navigate to sync the game data:

1. Go to **Races** page and click **"Sync DB with Races"** - This fetches races from the remote JSON API and stores them in the database
2. Go to **Classes** page and click **"Sync DB with Classes"** - This fetches classes from the remote JSON API and stores them in the database

After this, you can:
- Go to **New Character** to create a character
- Select a race and class (both optional)
- Set character stats (default base 10, max 18)
- View created characters and their details

#### Database Details
- **Location**: `%LOCALAPPDATA%\TheChroniclesOfTheOmuns\chronicles.db` (cross-platform SQLite)
- **Provider**: SQLite with Entity Framework Core 10.0.1
- **Migrations**: Auto-applied on app startup from `PlayerAppBlazor/Migrations/`

