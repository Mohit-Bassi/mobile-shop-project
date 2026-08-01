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
  public-site/    # React app — storefront
  admin-panel/    # React app — admin
```

## Testing

- Backend: xUnit unit tests plus `WebApplicationFactory`-based integration tests against a SQLite in-memory database.
- Frontend: Playwright end-to-end tests on both apps, exercising real browse/filter/CRUD/auth flows against a running backend.
