# Mobile Shop Project

A full-stack advertisement/showcase website for a second-hand mobile phone shop: browse used phones, accessories, and repair services, and submit inquiries. No online payments — all transactions happen offline. A single Admin manages listings via a JWT-protected admin panel.

## Stack

- **Backend**: .NET 9 Web API (layered architecture — Controllers/Application/Domain/Infrastructure)
- **Frontend**: Two React (Vite) apps — `public-site` (anonymous storefront) and `admin-panel` (JWT-protected)
- **Database**: SQL Server (LocalDB for local dev), images stored in-DB (`VARBINARY(MAX)`)
- **Auth**: JWT (access + rotating refresh tokens) for the single Admin account

## Project Structure

```
backend/
  MobileShop.sln
  src/
    MobileShop.Api/            # Controllers, Program.cs, appsettings
    MobileShop.Application/    # Services, DTOs, validators
    MobileShop.Domain/         # Entities, enums
    MobileShop.Infrastructure/ # EF Core, repositories, JWT, image storage
    MobileShop.Common/         # Shared helpers
  tests/
    MobileShop.UnitTests/
    MobileShop.IntegrationTests/
frontend/
  public-site/    # React app — storefront (port 5173)
  admin-panel/    # React app — admin (port 5174)
```

## Getting Started

### Backend

```bash
cd backend
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=MobileShopDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" --project src/MobileShop.Api
dotnet user-secrets set "Jwt:SigningKey" "<a long random string>" --project src/MobileShop.Api
dotnet user-secrets set "AdminSeed:Email" "admin@mobileshop.local" --project src/MobileShop.Api
dotnet user-secrets set "AdminSeed:Password" "<a strong password>" --project src/MobileShop.Api

dotnet run --project src/MobileShop.Api
```

Swagger UI available at `/swagger` in Development.

Run tests:

```bash
dotnet test
```

### Frontend

```bash
cd frontend/public-site && npm install && npm run dev   # http://localhost:5173
cd frontend/admin-panel && npm install && npm run dev   # http://localhost:5174
```

Run Playwright E2E tests (per app):

```bash
npx playwright test
```
