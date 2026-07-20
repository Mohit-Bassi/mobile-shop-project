# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A full-stack advertisement/showcase site for a second-hand mobile phone shop: browse used phones, accessories, and repair services, and submit inquiries. **No online payments** — all transactions happen offline, in person. A single Admin manages listings via a JWT-protected admin panel.

Monorepo with three independently-run parts: one .NET backend and two React frontends.

```
backend/
  MobileShop.sln
  src/
    MobileShop.Api/            # Controllers, Program.cs (DI/middleware pipeline), appsettings
    MobileShop.Application/    # Services, DTOs, FluentValidation validators, repository/service interfaces
    MobileShop.Domain/         # Entities, enums — no EF Core references
    MobileShop.Infrastructure/ # EF Core (AppDbContext, configs, migrations), repositories, JWT/auth, image storage
    MobileShop.Common/         # Pagination (PagedResult/PageRequest), sorting (SortParser), exceptions
  tests/
    MobileShop.UnitTests/      # xUnit, mocked dependencies, no DB
    MobileShop.IntegrationTests/ # xUnit + WebApplicationFactory, SQLite in-memory
frontend/
  public-site/  # Anonymous storefront (port 5173)
  admin-panel/  # JWT-protected admin SPA (port 5174)
```

## Commands

### Backend (`backend/`)

```bash
dotnet build                          # build whole solution
dotnet test                           # run all tests (unit + integration)
dotnet test tests/MobileShop.UnitTests
dotnet test tests/MobileShop.IntegrationTests
dotnet test --filter "FullyQualifiedName~MobilesEndpointsTests"   # single test class
dotnet run --project src/MobileShop.Api --launch-profile https    # run API on https://localhost:7152
```

EF Core migrations (run from `backend/`):
```bash
dotnet ef migrations add <Name> --project src/MobileShop.Infrastructure --startup-project src/MobileShop.Api --output-dir Persistence/Migrations
dotnet ef database update --project src/MobileShop.Infrastructure --startup-project src/MobileShop.Api
```

Required user-secrets before first run (see README.md for full list): `ConnectionStrings:DefaultConnection`, `Jwt:SigningKey`, `AdminSeed:Email`, `AdminSeed:Password`. On first Development run, `Program.cs` applies migrations and seeds the admin user + sample catalog data automatically (`MobileShop.Infrastructure.Persistence.DbSeeder`).

### Frontend (`frontend/public-site/` or `frontend/admin-panel/`)

```bash
npm install
npm run dev      # vite dev server
npm run build     # tsc -b && vite build — always run this to typecheck, there is no separate `tsc --noEmit` script
npm run lint      # oxlint
npx playwright test    # E2E — see Testing notes below
```

Each app needs a local `.env` (gitignored): `VITE_API_BASE_URL=https://localhost:7152/api/v1`.

## Architecture

### Backend layering

Strict one-way dependency flow: `Api → Application → Domain`, with `Infrastructure` implementing interfaces defined in `Application` (`Api` and `Infrastructure` both depend on `Application`/`Domain`, but `Application` never depends on `Infrastructure`). Controllers call `Application` service interfaces; services call `Application` repository interfaces; `Infrastructure` provides the EF Core-backed implementations. DTOs cross the Controller↔Service boundary — Domain entities never leak to the API surface.

Public vs. Admin split runs through every layer for the same resource: e.g. `IMobileRepository` has both `GetActivePagedAsync`/`GetActiveDetailByIdAsync` (public, `Status == Active` only) and `GetAdminPagedAsync`/`CreateAsync`/`UpdateAsync`/`UpdateStatusAsync` (admin, all statuses). Admin controllers live under `Api/Controllers/Admin/` and are `[Authorize(Roles = "Admin")]`-gated; public controllers live directly under `Api/Controllers/`.

**Two DI registration paths** in `MobileShop.Infrastructure/DependencyInjection.cs`: `AddInfrastructure()` (SQL Server, used by the real app) vs. `AddInfrastructureServices()` (no DbContext registered, used when the `Testing` ASP.NET environment supplies its own SQLite-backed `AppDbContext`). `Program.cs` branches on `builder.Environment.IsEnvironment("Testing")` to pick one — registering both SQL Server and SQLite providers in the same service collection throws at runtime, so don't try to merge these paths.

**Soft-delete convention**: DELETE endpoints don't remove rows — they set `Status = Draft` (mobiles/accessories) or `IsActive = false` (categories/repair services), preserving inventory history and images. `PATCH .../status` is the explicit status-transition endpoint.

### Image pipeline

Images are stored **in SQL Server** (`VARBINARY(MAX)`), not on disk — a deliberate tradeoff for this shop's scale. Split across two tables: `Images` (lightweight metadata — owner, primary flag, order — safe to query without touching binary data) and `ImageVariants` (the actual bytes, one row per resolution). On upload, `ImageStorageService` (Infrastructure/Services) uses ImageSharp to generate three WebP variants — thumbnail/medium/full — in one transaction; the first uploaded image for a listing is auto-marked primary. The public streaming endpoint (`GET /api/v1/images/{id}/{variant}`) sets `Cache-Control: immutable` + `ETag` and is wrapped in ASP.NET Core `OutputCache` so repeat requests never hit SQL Server. `SixLabors.ImageSharp` is pinned to the 3.1.x line — v4+ requires a paid commercial license.

### Auth

JWT access tokens (15 min) + rotating opaque refresh tokens (SHA-256 hash stored in `RefreshTokens`, plaintext only ever in an httpOnly cookie). Refresh tokens are **single-use**: each refresh rotates to a new token and revokes the old one — reusing an already-rotated token is rejected. The refresh cookie is `SameSite=None; Secure` (not `Strict`) because the admin-panel SPA and the API are different origins making cross-origin XHR calls; `SameSite=Strict`/`Lax` cookies are never sent on cross-origin requests regardless of same-host scheme/port. Login (`/auth/login`) is rate-limited to 5 attempts/5min per IP via ASP.NET Core's built-in rate limiter, plus DB-backed account lockout (`Users.FailedLoginCount`/`LockoutUntilUtc`) as defense-in-depth. Public inquiry submission is separately rate-limited to 10/10min.

### Frontend patterns (both apps)

Both apps follow the same structure: `api/` (axios calls), `hooks/` (TanStack Query wrappers around `api/`), `pages/`, `components/`, `types/api.ts` (hand-written types mirroring backend DTOs — no codegen).

- **public-site**: React Router pages, MUI, no auth.
- **admin-panel**: adds `store/authStore.ts` (Zustand — access token kept **in memory only**, never persisted to storage) and `auth/ProtectedRoute.tsx`. `api/client.ts` has an axios response interceptor that on 401 calls `/auth/refresh` (via a separate `refreshClient` instance to avoid interceptor recursion) and retries the original request once. Because the access token is memory-only, a full page reload loses it — `ProtectedRoute` handles this by attempting one silent refresh (using the httpOnly cookie) before redirecting to `/login`.
- **MUI version note**: this repo pins `@mui/material` ^9.x, where shorthand system props like `fontWeight`, `justifyContent`, `alignItems`, `flexWrap` are **not** accepted directly on `Typography`/`Stack` — pass them inside `sx={{ ... }}` instead, or TypeScript will reject the props.
- **react-hook-form + zod v4**: `z.coerce.number()` and similar coercing schemas mean the parsed input type differs from the output type. `useForm` must be given both: `useForm<z.input<typeof schema>, unknown, z.output<typeof schema>>(...)`, otherwise the resolver's types don't line up. See any of `frontend/admin-panel/src/components/*FormDialog.tsx` for the pattern.
- **MUI TextFields bound via `register()`** (uncontrolled — no `value` prop) don't shrink their label when populated programmatically via `reset()`. Fix applied throughout: `slotProps={{ inputLabel: { shrink: true } }}`.

### Testing

- **Backend integration tests** use `CustomWebApplicationFactory` (SQLite in-memory, ASP.NET `Testing` environment) — see `backend/tests/MobileShop.IntegrationTests/CustomWebApplicationFactory.cs`. `AppDbContext.OnModelCreating` applies a conditional `decimal→double` value converter when `Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite"`, because SQLite can't `ORDER BY` its default decimal representation — SQL Server (dev/prod) is unaffected.
- **Playwright configs start their own dependencies** via `webServer` arrays — `public-site/playwright.config.ts` starts the API + itself; `admin-panel/playwright.config.ts` starts the API + public-site + itself (needed because one admin E2E test verifies a published listing shows up on the public site).
- **admin-panel E2E must run with `workers: 1`** and each spec file authenticates once via a shared `beforeAll`-created browser page reused across all `test()` calls in that file (see `e2e/admin.spec.ts`) — refresh tokens are single-use, so parallel or per-test fresh browser contexts loading the same session would race and invalidate each other. Requires `E2E_ADMIN_PASSWORD` env var (matches the `AdminSeed:Password` user-secret) — never hardcode the real admin password in test source.
