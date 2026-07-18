# Mobile Shop Project

A full-stack advertisement/showcase website for a second-hand mobile phone shop: browse used phones, accessories, and repair services, and submit inquiries. No online payments — all transactions happen offline, in person. A single Admin manages listings via a JWT-protected admin panel.

## Stack

- **Backend**: .NET 9 Web API — layered architecture (Controllers → Services → Repositories), EF Core, JWT auth, FluentValidation
- **Frontend**: Two React (Vite + TypeScript + MUI) apps — `public-site` (anonymous storefront) and `admin-panel` (JWT-protected)
- **Database**: SQL Server (LocalDB for local dev); product/repair photos stored in-DB as `VARBINARY(MAX)`, resized into thumbnail/medium/full WebP variants on upload
- **Auth**: JWT access tokens (15 min) + rotating opaque refresh tokens in an httpOnly cookie, for the single Admin account
- **Testing**: xUnit (unit + integration, SQLite in-memory) on the backend; Playwright E2E on both frontends

## Features

- **Public site**: browse/filter mobiles (brand, price, condition, sort) and accessories (category, compatible model), view repair services, submit inquiries
- **Admin panel**: dashboard summary, full CRUD for mobiles/accessories/categories/repair services (incl. photo upload/reorder/primary selection), inquiries list with status updates
- **Security**: rate-limited login (5/5min) and inquiry submission (10/10min), account lockout after repeated failed logins, parameterized queries via EF Core, security headers (CSP/X-Frame-Options/etc.), CORS locked to the two frontend origins

## Project Structure

```
backend/
  MobileShop.sln
  src/
    MobileShop.Api/            # Controllers, Program.cs, appsettings
    MobileShop.Application/    # Services, DTOs, validators, repository/service interfaces
    MobileShop.Domain/         # Entities, enums
    MobileShop.Infrastructure/ # EF Core, repositories, JWT/auth, image storage
    MobileShop.Common/         # Pagination, sorting, exceptions
  tests/
    MobileShop.UnitTests/
    MobileShop.IntegrationTests/
frontend/
  public-site/    # React app — storefront (port 5173)
  admin-panel/    # React app — admin (port 5174)
```

## Getting Started

### Prerequisites

- .NET 9 SDK, Node 20+, SQL Server LocalDB (or any SQL Server instance)
- Trust the local dev HTTPS cert once: `dotnet dev-certs https --trust`

### Backend

```bash
cd backend
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=MobileShopDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" --project src/MobileShop.Api
dotnet user-secrets set "Jwt:SigningKey" "<a long random string>" --project src/MobileShop.Api
dotnet user-secrets set "AdminSeed:Email" "admin@mobileshop.local" --project src/MobileShop.Api
dotnet user-secrets set "AdminSeed:Password" "<a strong password>" --project src/MobileShop.Api

dotnet run --project src/MobileShop.Api --launch-profile https
```

On first run in Development, this applies EF Core migrations and seeds the admin user (from the secrets above) plus sample catalog data. Swagger UI is at `/swagger`.

Run backend tests:

```bash
dotnet test
```

### Frontend

Each app needs a `.env` (gitignored) pointing at the API:

```
VITE_API_BASE_URL=https://localhost:7152/api/v1
```

```bash
cd frontend/public-site && npm install && npm run dev   # http://localhost:5173
cd frontend/admin-panel && npm install && npm run dev   # http://localhost:5174
```

### End-to-end tests (Playwright)

Each app's `playwright.config.ts` starts the backend API (and, for admin-panel, the public-site too) automatically — just run:

```bash
cd frontend/public-site && npx playwright test
```

```bash
# admin-panel E2E logs in as the seeded admin, so the password must be supplied out-of-band
# (never hardcode it in the test files):
cd frontend/admin-panel && E2E_ADMIN_PASSWORD='<the AdminSeed:Password value>' npx playwright test
```

The admin-panel suite runs with a single worker and shares one authenticated browser context per spec file — the refresh token is single-use/rotating, so parallel contexts reloading the same session would invalidate each other, and login itself is rate-limited.
