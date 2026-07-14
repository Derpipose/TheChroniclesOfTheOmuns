### Welcome to the Chronicles of the Omuns Repository!
This repository is the brain child of Derpipose and her husband KrystalHydralisk
This contains the current character builder for the Omuns RPG system, as well as a few other tools to help with gameplay.
Feel free to explore the code, contribute, or use it for your own Omuns adventures!

### Current status: In Development and not ready for release yet
The character builder is still being worked on, and there are many features yet to be added. Stay tuned for updates!
Current dev timeline is that I hope to have a working version by the end of 2026, Christmas time to be exact,  for the player side.
June 2027 for the Game Master, or Chronicler, side.

The current dev plan can be found: [https://docs.google.com/document/d/1-_hxALgBB9ue5jpOuSIECTwRzIaqAatZP465uhR4I44/edit?usp=sharing](https://docs.google.com/document/d/1kHt8aTOYBO9ikv3AfFeUJ_B-ZeSJlld_mo9rzK_wiqk/edit?usp=sharing)


### How to Get the App Running for Devs

#### Prerequisites
- .NET 10.0 SDK installed
- Docker & Docker Compose
- Node.js + pnpm (for the React frontend)
- Access to the dev server via Tailscale (or run everything locally if you're not on the shared dev environment)

#### Setting Up the Application

1. **Start the dev stack** (API + PostgreSQL) from the solution root:
```powershell
   docker compose -f docker-compose.dev.yml up
```
   This spins up the dev environment on the `54xx` port range.

2. **Apply Entity Framework migrations** (from the `ChroniclesAPI` directory):
```powershell
   dotnet ef database update
```
   This applies all migrations against the PostgreSQL dev database (port `5433`).

3. **Before running the api, generate a JWT**:
```powershell
   [Convert]::ToBase64String((1..64 | ForEach-Object { [byte](Get-Random -Max 256) }))
```
Do this to generate a JWT and in your user secrets, set "Jwt:Key": to that. This will allow you to test out the API successfully. You'll need your own local db to connect to though. 


4. **Run the API with hot reload**:
```powershell
   dotnet watch
```

5. **Run the React frontend**:
```powershell
   cd PlayerAppReact
   pnpm dev
```
   The Vite dev server will print the local URL to use.

#### First Time Setup in the App

When the app first loads, navigate to sync the game data:

1. Go to **Races** and sync — fetches races from the remote JSON API and stores them in the database
2. Go to **Classes** and sync — fetches classes from the remote JSON API and stores them in the database

After this, you can:
- Go to **New Character** to create a character
- Select a race and class (both optional)
- Set character stats (default base 10, max depends on the setting. Fantasy, when implemented, will be a base of 18. Scifi is a base of 20)
- View created characters and their details

#### Authentication (Dev)

Auth uses an email-based one-time code (OTP) + JWT flow:

1. `POST auth/request` with an email — generates a one-time code using a cryptographically secure random number generator, e.g.:
```csharp
   var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
   var codeHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code)));
   // codeHash (not the raw code) is what gets persisted to AuthCode
```
2. The raw code is emailed to the user via Gmail SMTP (MailKit); only the SHA256 hash is stored.
3. `POST auth/verify` with the code — validates it against the stored hash and returns a JWT (claims: `Sub`, `Email`, `Jti`).

Dev and prod use separate JWT signing keys, configured via `dotnet user-secrets` locally.

#### Database Details
- **Location**: Dev database runs on the dev server (Tailscale) — you need Tailscale access to reach it
- **Provider**: PostgreSQL
- **Migrations**: Applied via `dotnet ef database update` from `ChroniclesAPI`

### What is being worked on?
Currently, there's a project that details everything that is being worked on actively. Each component has been broken down and made
into small bite-sized tasks. Since this is iterative development, there may be some tasks that go through and touch up the work of
previous tasks.

### License Notice
This project is source-available for viewing and learning purposes.
Contributions and reuse of ideas require explicit permission.