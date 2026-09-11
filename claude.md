# The Chronicles of the Omuns — Project Context (CLAUDE.md)

A self-hosted tabletop RPG character management and tracker web app (similar to D&D Beyond / Roll20), built for a custom RPG system and a 12-player home gaming group. Supports multiple **settings** (game worlds), each with its own character types sharing a common `Character` base. Built by Rhys — a junior engineer building to professional standards, learning as she goes — with a co-admin (husband, the Chronicler/GM, who will use a separate elevated-permission app) and a friend/buddy (Josh) contributing frontend scaffolding under Rhys's review.

No one is currently blocked by missing features — "unblock the most people first" and "never build the same thing twice" are the active prioritization heuristics.

---

## Solution Structure

Multi-project .NET solution: `TheChroniclesOfTheOmuns`

- **`PlayerApp.Models`** — shared domain model / class library. `DbContext` lives here.
- **`ChroniclesAPI`** — ASP.NET Core Web API.
- **`ChroniclesTest`** — xUnit test project.
- **`PlayerAppReact`** — React/TypeScript frontend.

EF Core handles schema/migrations; Dapper was discussed for the query layer but **not committed to** — don't assume it's in use.

---

## Domain Model

- Core entities: `Character`, `ScifiCharacter`, `CharacterStats`, `Player`, `RaceStats`, `StatModifier`, `ScifiRace`, `ScifiSubrace`, `ScifiTalent`, `ScifiClass`, `ScifiBackground`, `AuthCode`, `PlayerRoleAssignment`.
- Setting-specific entities are prefixed by setting (e.g. `Scifi...`); a parallel `Fantasy...` set is expected in Phase 2, and eventually a Post-Apocalyptic set further out.
- Join entities (`ScifiBackgroundTalent`, `ScifiClassTalent`) use **composite PKs**, no surrogate keys.
- `RaceStats` stores `Modifiers` and `FreeStatPicks` as **JSON columns**.
- `Character.Setting` uses `SettingEnum`.
- `StatType` enum values are **explicit** (Strength=0 through Intelligence=5) — never let the compiler auto-assign.
- Queries traverse **Background/Class → join entity → Talent** (never the reverse); `ThenInclude()` is the standard pattern.
- `JsonStringEnumConverter` is registered globally — enums serialize as strings over the wire (must also be set in EF Core `HasConversion` blocks — see Gotchas).

---

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
- Passwordless email OTP → JWT, stored in `localStorage`; `tokenStorage.ts` owns all localStorage access for the `'token'` key; `PlayerContext` resolves identity in protected controllers.

---

## Frontend

- React + TypeScript, Vite, TanStack Query, React Router v7, Tailwind CSS.
- JWT stored in `localStorage`.
- `ProtectedRoute` / `PublicOnlyRoute` components handle client-side route guarding.
- react-konva planned for the Phase 5 battle map.
- Player theme system is fully implemented end-to-end: backend `ThemeEnum` / `Player.Theme` / `SetTheme()`, endpoints, `PlayerDetailsDto`; frontend `playerApi.ts`, `usePlayer()`, `themeStorage.ts`, `ThemeSync` applying theme synchronously on load to avoid a flash of unstyled content. `theme.css` defines 5 variables × 3 intensities for the General palette.

---

## Infrastructure

- Docker Compose with separate dev (`54xx` ports) and prod (`55xx` ports) configs on the same server.
- Hot reload: `dotnet watch` (API) and Vite dev server (React).
- Server runs on Tailscale (`derpserver.flyingfox-gourami.ts.net`), accessed via VS Code Remote SSH.
- CORS: configured for Tailscale hostname + localhost origins using `SetIsOriginAllowed` predicate (**dev only** — don't carry this pattern into prod config).
- Dev Dockerfiles must include a **non-root `USER` directive** (matching host UID/GID) to prevent root-owned build artifacts poisoning `obj/` directories.
- Public exposure plan: DuckDNS + Nginx + Let's Encrypt (Tailscale free plan caps at 6 users, not viable at 12-player scale). Dev stays Tailscale-only. Separate DuckDNS subdomains for dev/prod, routed via Nginx `server_name` blocks. Certs already generated via `tailscale cert`; still need to resolve TLS termination approach (Tailscale `serve` vs. nginx TLS termination inside the existing container).
- GitHub Actions CI/CD on a self-hosted runner: pg_dump backups pre-deploy, test gating, secret injection.

---

## API Conventions

- Routing: `api/[controller]` for generic entities; `api/scifi/[controller]` for Scifi-specific entities (mirror this for future settings, e.g. `api/fantasy/[controller]`).
- Character CRUD and stats endpoints are **player-scoped** via `PlayerContext` (extracts player ID from JWT claims).
- Stats mutation split by character state:
  - Bulk `PUT` (`SetStat`) for **inactive** characters.
  - Single-stat `PATCH` (`IncreaseStat` / `DecreaseStat`) for **active** characters.
- Consumables use a single atomic `POST /consumables/{id}/use` endpoint to prevent apply/decrement drift (see Gotchas — this pattern came directly from an EF Core tracking-order bug).
- Swagger UI via **Swashbuckle.AspNetCore** — explicitly not Scalar.
- API testing is done with **`.http` files**, not curl.

---

## Secrets

- Local dev: `dotnet user-secrets` (connection string lives here, never in `appsettings.json`).
- Production: GitHub Actions environment variables, using double-underscore notation for nested config keys.
- Shared secrets (JWT keys, connection strings) live outside the repo — collaborators need them configured locally via the same mechanism.

---

## Settled Architecture Decisions (do not re-litigate)

1. **"If it's computable, don't store it."** Applied to `HasSubrace`, `IsPlayable`, and similar fields.
2. **No shared components across settings; clean breaks over incremental migration.** Setting-specific models are prefixed (`ScifiTalent`, `ScifiClass`, etc.). When a design is rejected outright, delete legacy code and tests in one clean, descriptive commit rather than converting incrementally — this came from the `.Legacy` update that broke everything hard; all `.Legacy` code was removed and tests rebuilt from scratch.
3. **Single atomic endpoints over split apply/decrement flows** — learned from EF Core tracking-order bugs.
4. **Parameterless protected/private constructor required for every EF-materialized entity**, alongside any public constructor. Check this first before chasing other explanations for materialization failures.
5. **Stat storage:** `dice roll + racial modifier + 10`; edit mode subtracts additions back for display only. Settled from playtester feedback — do not re-litigate.
6. **FK placement for cascade cleanup:** repoint FKs to the setting-specific subcharacter (e.g. `ScifiCharacterId`) so draft-switching via lock transaction cascades cleanly without orphaned rows.
7. **Background/class talent overlap** is resolved at character-creation runtime via name-match against `ScifiClass`, not by duplicating talent links onto background — avoids a second source of truth drifting.
8. **Minimal blast-radius changes** — prefer targeted fixes over refactors with wide call-site impact.
9. **ID-only constructors for join entities** — navigation properties are populated by EF Core at query time; constructors take IDs only.
10. **JSON columns for collections** — `IReadOnlyList<T>` properties use EF Core value converters with an explicit `ValueComparer<IReadOnlyList<T>>` to avoid change-tracking warnings.
11. **Pin EF Core package versions explicitly** across all projects to avoid NU1605 downgrade errors.
12. **`TalentImplementationStatus` enum** tags deferred talents by blocker type.
13. **Ammo** uses an `AmmoType` enum with Lesser/Standard/Greater tiers.
14. **Fantasy uses mana** (no talent-system equivalent); **Scifi uses cybernetics**. The combat/roll/HP engine is setting-agnostic except for healing pace.

---

## Gotchas & Hard-Won Knowledge

- **EF Core tracking order matters:** `db.Add(entity)` must precede any call that relies on navigation-property fixup — fixup only fires on tracking-state-change events. Null-conditional operators on nav properties silently no-op rather than throw, masking this bug class.
- **PostgreSQL type-conversion migrations:** changing a column type (e.g. `text` → `integer` for an enum) needs an explicit `USING` clause that EF's auto-generated migration omits. Replace with raw SQL: `ALTER COLUMN ... TYPE integer USING (CASE ...)` (or `DropColumn`+`AddColumn` when tables are empty). The `Down()` method needs a matching reverse `CASE`. Prefer a catch-all `ELSE -1` (not `ELSE 0`) to surface unexpected data rather than silently mapping it to a valid enum value.
- **`JsonStringEnumConverter`** must be registered both in EF Core `HasConversion` blocks *and* globally in `AddControllers().AddJsonOptions()`.
- **Enum-to-enum ordinal casting is dangerous** — use explicit switch mappings instead.
- **Input validation UX:** use `onBlur`, not `onChange`, for clamping, so players can clear and retype freely.
- **Swashbuckle:** nested DTOs with the same name across controllers cause duplicate schema ID errors — extract to a top-level DTO folder.
- **`[Conditional("DEBUG")]` over `#if DEBUG`** — preferred pattern for `SimulatePersist` methods and other debug-only bridge code (used until EF Core handles ID assignment).
- **Don't duplicate sources of truth** — e.g. a "Ballistics" vs "Ballistic" typo is what surfaced the background/class-overlap talent duplication risk.
- **Production builds catch things dev doesn't:** `tsc -b && vite build` surfaces unused declarations and possibly-undefined variables that Vite's dev server ignores.
- **Chrome's "Always use secure connections"** silently rewrites `http://` to `https://` before the request leaves the browser — a common source of mysterious connection failures on new devices.

---

## File Ordering Convention

Within a class file, order members as:
1. Identities/properties
2. Setters
3. Getters
4. Others
5. Simulates (`SimulatePersist` / debug-only bridge methods, kept at the bottom of the file)

---

## Tools & Stack Summary

- **Backend:** C# / .NET 10, ASP.NET Core, EF Core 10, Npgsql 10, MailKit (Gmail SMTP), JWT Bearer auth, Swashbuckle (Swagger UI)
- **Frontend:** React, TypeScript, Vite, TanStack Query, React Router v7, Tailwind CSS
- **Database:** PostgreSQL (dev container on Tailscale server, port 5433)
- **Testing:** xUnit v3, NSubstitute
- **Infrastructure:** Docker Compose, Tailscale, Nginx, DuckDNS, VS Code Remote SSH, GitHub Actions (self-hosted runner)
- **Terminal editing:** vim, not nano
- **Infra-as-code preference:** git-tracked, reproducible infrastructure (Docker Compose, CI/CD) over ad-hoc server config

---

## Roadmap (settled order — do not re-litigate)

1. **Phase 1: Scifi Core** — in progress/live
2. **Phase 2: Fantasy Core**
3. **Phase 3: Campaign Entity**
4. **Phase 4a: Minimal Enemy/NPC Stat Block**
5. **Phase 5: Live Session Layer** — SignalR hub (transient, no dedicated DB table), react-konva battle map (token positions as a JSON blob on Campaign), self-hosted ambient audio (Tabletop Audio, itch.io, Freesound) with SignalR broadcast for audio control
6. **Phase 4b: Chronicler Convenience Tooling**

---

## In-Progress / Not Yet Finalized

- `ScifiSubrace` navigation and the `HasSubrace` computed property exist in **commented-out** form on `ScifiRace`, pending full integration — don't assume this is live.
- TLS termination approach (Tailscale `serve` vs. nginx TLS termination) is unresolved.
- Public exposure (DuckDNS/Nginx/Let's Encrypt) is planned but not yet implemented — needed once player count exceeds Tailscale's free-tier user cap.
- Player-level theme picker UI: static swatch preview (2–3 color chips), apply-on-select using the same variable-injection as apply-on-load, decoupled from setting/campaign.
- Additional theme palettes beyond General, applied to setting-specific screens.
- Live Session Layer (SignalR, react-konva battle map, ambient audio) — Phase 5, further out.

---

## How Rhys Works — Read This Before Responding

- **Ask design/clarifying questions before writing code**, especially where there are database implications — don't assume. Confirm data shapes and architecture conversationally before writing any code.
- **Rhys writes her own code; Claude reviews rather than delivering full implementations by default** — except when Rhys is fatigued or a piece is genuinely too complex to fill in from hints, or she explicitly asks for the code.
- **Directness and scope discipline.** Move at pace, assume competence. Don't over-explain things Rhys already knows, don't re-ask answered questions, don't treat her as a beginner on established concepts, don't re-litigate settled decisions (see sections above).
- **One bug at a time.** Identify, diagnose, fix, move forward — don't continue exploring alternatives already ruled out.
- **Address warnings early**, while the codebase is still clean.
- **Don't reference code patterns or architecture not confirmed to exist in the current session.** If unsure whether something exists in the codebase, ask or check rather than assume.

### Teaching Mode (currently active for React/frontend work)

Rhys is actively **learning React** and wants to be taught, not handed answers. Default to teaching mode for React/frontend work unless told otherwise:

- **Hint before you solve.** When introducing a new concept or the next step in a setup/build process, give a nudge — what to look at, what question to ask, what the shape of the solution is — before giving the actual code or command.
- **Break work into discrete steps.** One step at a time. Don't dump a 10-step setup in one message — give the next actionable step, let Rhys attempt it or ask questions, then move forward.
- **Invite questions at each step**, rather than steamrolling into the next one.
- **Socratic first, complete code on request.** If Rhys seems stuck, tired, or the concept is too dense for hints alone, switch to giving fuller code — but let her signal that, or ask if she wants the direct answer, rather than assuming.
- **Tie new React concepts back to CotO's actual code** where possible (e.g. relate a new hook to `usePlayer()`, relate routing questions to `App.tsx`/`ThemeSync`, etc.) rather than generic textbook examples.
- **Adjust immediately when asked** — if Rhys says "just give me the code" or "skip ahead," drop the hinting and deliver directly for that ask, then return to teaching mode afterward unless told otherwise.