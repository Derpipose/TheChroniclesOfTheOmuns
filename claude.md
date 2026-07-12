# The Chronicles of the Omuns — Project Context

A self-hosted tabletop RPG character management and tracker web app (similar to D&D Beyond / Roll20) for a custom RPG system. Supports multiple settings (Scifi, Fantasy). Built for a small group of players by the repo owner (junior engineer, building to professional standards), with a co-admin (husband) and a friend ("buddy") who contributed frontend scaffolding.

## Solution Structure

Multi-project .NET solution: `TheChroniclesOfTheOmuns`

- **`PlayerApp.Models`** — shared domain model / class library. `DbContext` lives here.
- **`ChroniclesAPI`** — ASP.NET Core Web API.
- **`ChroniclesTest`** — xUnit test project.
- **`PlayerAppReact`** — React/TypeScript frontend.

EF Core handles schema/migrations; Dapper was discussed for the query layer but not committed to.

## Domain Model

- Core entities: `Character`, `ScifiCharacter`, `CharacterStats`, `Player`, `RaceStats`, `StatModifier`, `ScifiRace`, `ScifiSubrace`, `ScifiTalent`, `ScifiClass`, `ScifiBackground`, `AuthCode`, `PlayerRoleAssignment`.
- Setting-specific entities are prefixed by setting (e.g. `Scifi...`); a parallel `Fantasy...` set is expected eventually.
- Join entities (`ScifiBackgroundTalent`, `ScifiClassTalent`) use **composite PKs**, no surrogate keys.
- `RaceStats` stores `Modifiers` and `FreeStatPicks` as **JSON columns**.
- `Character.Setting` uses `SettingEnum`.
- `StatType` enum values are **explicit** (Strength=0 through Intelligence=5) — never let the compiler auto-assign.
- Queries traverse **Background/Class → join entity → Talent** (never the reverse); `ThenInclude()` is the standard pattern.

## Roles & Auth

- `PlayerRole` enum (`Admin`, `Chronicler`) stored in `PlayerRoleAssignment` join table, composite PK on `(PlayerId, Role)`. `Player` is the implicit base state (no role row needed for a plain player).
- OTP / magic-link auth flow:
  - `POST api/player/register`
  - `POST auth/request`
  - `POST auth/verify` (returns JWT)
- Email delivery via Gmail SMTP (MailKit) — moved off Resend due to domain verification limitations.
- OTPs generated with `RandomNumberGenerator.GetInt32`, SHA256-hashed before storage.
- JWT includes `Sub`, `Email`, `Jti` claims. Separate JWT signing keys for dev and prod.
- Email enumeration protection on the `auth/request` endpoint.

## Frontend

- React + TypeScript, Vite, TanStack Query, React Router v7, Tailwind CSS.
- JWT stored in `localStorage`.
- `ProtectedRoute` / `PublicOnlyRoute` components handle client-side route guarding.

## Infrastructure

- Docker Compose with separate dev (`54xx` ports) and prod (`55xx` ports) configs on the same server.
- Hot reload: `dotnet watch` (API) and Vite dev server (React).
- Server runs on Tailscale (`derpserver.flyingfox-gourami.ts.net`), accessed via VS Code Remote SSH.
- CORS: configured for Tailscale hostname + localhost origins using `SetIsOriginAllowed` predicate (**dev only** — don't carry this pattern into prod config).
- Dev Dockerfiles must include a **non-root `USER` directive** (matching host UID/GID) to prevent root-owned build artifacts poisoning `obj/` directories.
- Public exposure plan: DuckDNS + Nginx + Let's Encrypt (Tailscale free plan caps at 6 users, not viable at 12-player scale). Dev stays Tailscale-only. Separate DuckDNS subdomains for dev/prod, routed via Nginx `server_name` blocks.

## API Conventions

- Routing: `api/[controller]` for generic entities; `api/scifi/[controller]` for Scifi-specific entities (mirror this for future settings, e.g. `api/fantasy/[controller]`).
- Character CRUD and stats endpoints are **player-scoped** via `PlayerContext` (extracts player ID from JWT claims).
- Stats mutation split by character state:
  - Bulk `PUT` (`SetStat`) for **inactive** characters.
  - Single-stat `PATCH` (`IncreaseStat` / `DecreaseStat`) for **active** characters.
- Swagger UI via **Swashbuckle.AspNetCore** — explicitly not Scalar.

## Secrets

- Local dev: `dotnet user-secrets` (connection string lives here, never in `appsettings.json`).
- Production: GitHub Actions environment variables, using double-underscore notation for nested config keys.
- Shared secrets (JWT keys, connection strings) live outside the repo — collaborators need them configured locally via the same mechanism.

## Key Engineering Principles (apply proactively)

1. **EF Core parameterless constructor pattern** — every EF-materialized entity needs a `protected` or `private` parameterless constructor alongside any public constructor. Apply this to any new/refactored entity *before* chasing other explanations for materialization failures.
2. **"If it's computable, don't store it"** — e.g. `HasSubrace` is derived from `Subraces.Any()`, not a stored column. Apply this rule consistently to new model properties.
3. **ID-only constructors for join entities** — navigation properties are populated by EF Core at query time; constructors take IDs only.
4. **JSON columns for collections** — `IReadOnlyList<T>` properties use EF Core value converters with an explicit `ValueComparer<IReadOnlyList<T>>` to avoid change-tracking warnings.
5. **PostgreSQL type-conversion migrations** — when changing a column type (e.g. `text` → `integer` for an enum), EF's auto-generated migration omits the required `USING` clause. Replace with raw SQL: `ALTER COLUMN ... TYPE integer USING (CASE ...)`. The `Down()` method needs a matching reverse `CASE`. Prefer a catch-all of `ELSE -1` (not `ELSE 0`) to surface unexpected data rather than silently mapping it to a valid enum value.
6. **`[Conditional("DEBUG")]` over `#if DEBUG`** — preferred pattern for `SimulatePersist` methods and other debug-only bridge code (used until EF Core handles ID assignment).
7. **Clean breaks over incremental migration** — when a design is rejected outright, delete legacy code and tests in one clean, descriptive commit rather than converting incrementally.
8. **Pin EF Core package versions** explicitly across all projects to avoid NU1605 downgrade errors.
9. **Preserve test scaffold through refactors** — build config, dependencies, and CI integration stay; only test files get replaced.

## Working Style / Preferences

- Ask design/clarifying questions before writing code, especially where there are database implications — don't assume.
- API testing is done with **HTTP files**, not curl.
- Address warnings early, while the codebase is still clean.
- Terminal file editing uses **vim**, not nano.

## Tools & Stack Summary

- **Backend**: C# / .NET 10, ASP.NET Core, EF Core 10, Npgsql 10, MailKit (Gmail SMTP), JWT Bearer auth, Swashbuckle (Swagger UI)
- **Frontend**: React, TypeScript, Vite, TanStack Query, React Router v7, Tailwind CSS
- **Database**: PostgreSQL (dev container on Tailscale server, port 5433)
- **Testing**: xUnit v3, NSubstitute
- **Infrastructure**: Docker Compose, Tailscale, Nginx, DuckDNS, VS Code Remote SSH, GitHub Actions (self-hosted runner)

## In-Progress / Not Yet Finalized

- `ScifiSubrace` navigation and the `HasSubrace` computed property exist in **commented-out** form on `ScifiRace`, pending full integration — don't assume this is live.
- Public exposure (DuckDNS/Nginx/Let's Encrypt) is planned but not yet implemented.