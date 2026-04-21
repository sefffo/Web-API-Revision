<div align="center">

# 🛒 E-Commerce REST API

### Production-grade ASP.NET Core 10 Web API — Clean Architecture, JWT Auth, Docker, Redis, Stripe & Fawaterak, Azure CI/CD

[![Live API](https://img.shields.io/badge/Live%20API-Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://web-api-revesion-c2chh0cyctd7dpcn.eastasia-01.azurewebsites.net/swagger/index.html)
[![Docker Hub](https://img.shields.io/badge/Docker%20Hub-saif31%2Fecomm--api-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://hub.docker.com/r/saif31/ecomm-api)
[![Dashboard](https://img.shields.io/badge/Admin%20Dashboard-Live-0ea5e9?style=for-the-badge&logo=vercel&logoColor=white)](https://ecommerce-dashboard-one-tawny.vercel.app/)
[![CI/CD](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?style=for-the-badge&logo=githubactions&logoColor=white)](https://github.com/sefffo/Web-API-Revision/actions)

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
[![Redis](https://img.shields.io/badge/Redis-7-DC382D?logo=redis&logoColor=white)](https://redis.io/)
[![Azure](https://img.shields.io/badge/Azure-App%20Service-0078D4?logo=microsoftazure&logoColor=white)](https://azure.microsoft.com/)

</div>

---

## 🚀 Try It Right Now

| Resource | Link |
|---|---|
| 🎛️ **Admin Dashboard** | <https://ecommerce-dashboard-one-tawny.vercel.app/> |
| 📖 **Live API + Swagger** | <https://web-api-revesion-c2chh0cyctd7dpcn.eastasia-01.azurewebsites.net/swagger/index.html> |
| 🐳 **Docker Image** | <https://hub.docker.com/r/saif31/ecomm-api> — `docker pull saif31/ecomm-api:latest` |
| 💻 **Frontend Repo** | <https://github.com/sefffo/ecommerce-dashboard> |

### 🔑 Demo Credentials (SuperAdmin)

```
Email:    superadmin@ecommerce.com
Password: SuperAdmin@123
```

Log in at the dashboard URL above — you get full access to manage orders, products, brands, types, users, and roles.

---

## 📑 Table of Contents

1. [Overview](#-overview)
2. [Architecture](#-architecture--clean-architecture--onion)
3. [Design Patterns](#-design-patterns-used)
4. [Tech Stack](#-tech-stack)
5. [Features](#-features)
6. [Project Structure](#-project-structure)
7. [Authentication & Authorization](#-authentication--authorization)
8. [Caching Strategy](#-caching-strategy)
9. [Payments](#-payments)
10. [Running Locally](#-running-locally)
11. [Docker Deployment](#-docker-deployment)
12. [CI/CD Pipeline](#-cicd-pipeline)
13. [API Reference](#-api-reference)
14. [Dashboard Preview](#-dashboard-preview)
15. [What I Learned](#-what-i-learned)

---

## 📖 Overview

A full-featured e-commerce backend built from the ground up with production concerns in mind — **not a tutorial clone**. Every architectural decision was made deliberately: from module boundaries, to caching invalidation, to refresh-token rotation, to multi-stage Docker builds.

- **7 independent projects** organized by Clean Architecture rings (Domain → Application → Infrastructure → Presentation)
- **2 EF Core DbContexts** — one for business data, one for ASP.NET Identity (separation of concerns)
- **JWT + refresh tokens** with rotation & revocation
- **Role-based authorization** (`SuperAdmin`, `Admin`, `User`)
- **Redis caching** with automatic invalidation on mutation
- **Stripe + Fawaterak** payment integrations
- **Containerized** with a multi-stage Dockerfile + `docker-compose.yml`
- **Continuously deployed** to Azure App Service via GitHub Actions

---

## 🏗️ Architecture — Clean Architecture / Onion

```
┌──────────────────────────────────────────────────────────┐
│                  ECommerce.Web (Host)                    │ ← Program.cs, middleware, DI wiring
│  CORS · JWT · Swagger · Global exception handler         │
├──────────────────────────────────────────────────────────┤
│            ECommerce.Presentation (Controllers)          │ ← HTTP boundary, zero business logic
│  Authentication · Products · Orders · Basket · Payment   │
├──────────────────────────────────────────────────────────┤
│  ECommerce.Services + ECommerce.Services.Abstraction     │ ← Application layer (use cases)
│  AuthService · ProductService · OrderService · …         │
├──────────────────────────────────────────────────────────┤
│               ECommerce.Persistence                      │ ← EF Core, repositories, seed data
│  StoreDbContext · IdentityDbContext · Specs Evaluator    │
├──────────────────────────────────────────────────────────┤
│                  ECommerce.Domain                        │ ← Pure C#, no dependencies
│  Entities · Interfaces · Specifications base             │
├──────────────────────────────────────────────────────────┤
│                ECommerce.SharedLibrary                   │ ← Cross-cutting: DTOs, Result<T>,
│  DTOs · Result<T> · PaginatedResult<T> · Settings        │   pagination, options classes
└──────────────────────────────────────────────────────────┘
```

**Dependency rule:** every arrow points inward. `Domain` knows nothing about EF Core, ASP.NET, or Redis. Swap Redis for Memcached tomorrow — only `Persistence` / `Services` change.

---

## 🎨 Design Patterns Used

| Pattern | Where | Why |
|---|---|---|
| **Repository** | `ECommerce.Persistence/Repositories` | Abstracts EF Core away from services; each aggregate has a dedicated repo |
| **Unit of Work** | `IUnitOfWork` + `UnitOfWork.cs` | Single `SaveChangesAsync()` across multiple repos — atomic writes |
| **Specification** | `ECommerce.Domain/Specifications` + `SpecificationsEvaluator` | Reusable query logic (filtering / sorting / paging / includes) — no LINQ in services |
| **Result Pattern** | `ECommerce.SharedLibrary.CommonResult.Result<T>` | Explicit success/failure instead of exceptions for control flow |
| **DTO / AutoMapper** | `MappingProfiles/` | Never leak entities to the API surface |
| **Factory** | `ECommerce.Web/Factories` | Custom validation error responses for ModelState |
| **Middleware** | `CustomMiddleWares/ExceptionHandlerMiddleWare` | Centralized error handling → consistent `ProblemDetails` |
| **Options Pattern** | `JwtOptions`, `StripeSettings` | Strongly-typed configuration binding |
| **Dependency Injection** | `WebAppRegistrations` extension | Clean DI composition via `IServiceCollection` extensions |

---

## 🧰 Tech Stack

<div align="center">

| Layer | Technology |
|---|---|
| **Runtime** | ASP.NET Core 10 · C# 12 |
| **Data** | Entity Framework Core 10 · SQL Server 2022 |
| **Cache** | Redis 7 (StackExchange.Redis) |
| **Auth** | ASP.NET Identity · JWT · Refresh Tokens |
| **Payments** | Stripe · Fawaterak |
| **Validation** | FluentValidation / Data Annotations |
| **Mapping** | AutoMapper |
| **Docs** | Swashbuckle (Swagger) |
| **Container** | Docker (multi-stage) + Docker Compose |
| **Cloud** | Azure App Service (East Asia) |
| **CI/CD** | GitHub Actions |

</div>

---

## ✨ Features

### 🔐 Identity & Auth
- Register / Login with JWT access tokens
- **Refresh-token rotation** — old token invalidated on every refresh
- **Revoke endpoint** — logs user out on all devices instantly
- Role management: `SuperAdmin` > `Admin` > `User`
- `[Authorize(Roles = "Admin,SuperAdmin")]` on protected endpoints
- Google OAuth hooks (see `AuthenticationController`)

### 🛍️ Products
- Paginated search (by name) + filter (brand, type) + sort
- Admin-only create / (soft)-delete
- Image upload endpoint with server-side validation

### 🛒 Basket
- **Redis-backed** basket (TTL-based, no DB bloat)
- One basket per authenticated user
- Survives page reloads; cleared on successful order

### 📦 Orders
- Checkout: basket → address → delivery method → payment
- **9-state status machine** (Pending → Processing → PaymentPending → PaymentReceived → Paid → Preparing → Shipped → Delivered; or Cancelled)
- PATCH endpoint for admin status updates with role gate
- Order history per user + admin "all orders" view

### 💳 Payments
- Stripe integration via webhooks
- Fawaterak integration (Egypt) — PDF integration guide included in repo
- Payment confirmation updates order status automatically

### ⚡ Performance
- Redis response caching on read-heavy endpoints (products, brands, types)
- **Cache-key invalidation** on create/update/delete — no stale data
- JsonSerializer configured once with camelCase policy so cache hits match fresh responses

### 🧱 Robustness
- Global exception middleware → consistent JSON error shape
- `Result<T>` pattern avoids throwing for expected failures (validation, 404, 403)
- Structured logging at every service boundary

---

## 📂 Project Structure

```
Web-API-Revision/
├── ECommerce.Domain/                    # Pure entities + interfaces
│   ├── Entities/
│   │   ├── IdentityModule/              # User, RefreshToken
│   │   ├── ProductModule/               # Product, Brand, Type
│   │   ├── OrderModule/                 # Order, OrderItem, DeliveryMethod, OrderStatus
│   │   └── BasketModule/                # CustomerBasket, BasketItem
│   └── Interfaces/                       # IUnitOfWork, IGenericRepository<T>, ISpecifications<T>
│
├── ECommerce.Persistence/               # EF Core + Identity + Redis adapters
│   ├── Data/DbContexts/                 # StoreDbContext, IdentityDbContext
│   ├── Repositories/                    # GenericRepository, BasketRepository (Redis)
│   ├── SpecificationsEvaluator.cs       # Applies specs to IQueryable
│   └── Data/DataSeed/                   # Seed products, roles, super-admin
│
├── ECommerce.Services.Abstraction/       # Service interfaces (DI contracts)
├── ECommerce.Services/                  # Application layer
│   ├── AuthenticationService.cs
│   ├── ProductService.cs
│   ├── OrderService.cs                  # Status state-machine lives here
│   ├── BasketService.cs
│   ├── CacheService.cs                  # Redis wrapper + JSON options
│   ├── UploadService.cs
│   ├── Payment Service/                 # StripeService, FawaterakService
│   ├── MappingProfiles/                 # AutoMapper
│   └── Specifications/                  # Query specs
│
├── ECommerce.Presentation/              # Thin controllers (HTTP only)
│   └── Controllers/
│       ├── AuthenticationController.cs
│       ├── ProductsController.cs
│       ├── OrderController.cs
│       ├── BasketController.cs
│       ├── PaymentController.cs
│       └── UploadController.cs
│
├── ECommerce.SharedLibrary/             # Cross-cutting (DTOs, Result<T>, Settings)
├── ECommerce.Web/                       # Host project
│   ├── Program.cs
│   ├── Extensions/WebAppRegistrations.cs
│   ├── CustomMiddleWares/ExceptionHandlerMiddleWare.cs
│   ├── Factories/                       # ModelState error factory
│   └── appsettings{.Docker,.Development}.json
│
├── Dockerfile                           # Multi-stage (SDK → runtime)
├── docker-compose.yml                   # API + SQL Server 2022 + Redis 7
└── .github/workflows/azure-deploy.yml   # CI → Azure App Service
```

---

## 🔒 Authentication & Authorization

**Access + refresh token flow**

```
┌──────────┐                         ┌─────────────┐
│  Client  │  POST /login            │    API      │
│          │ ───────────────────▶    │             │
│          │   access (15m)          │             │
│          │   refresh (7d)          │             │
│          │ ◀───────────────────    │             │
│          │                         │             │
│          │  requests with Bearer   │             │
│          │ ───────────────────▶    │             │
│          │                         │             │
│          │  when access expires:   │             │
│          │  POST /refresh-token    │             │
│          │ ───────────────────▶    │             │
│          │   NEW access + NEW refresh            │
│          │   (old refresh revoked) │             │
│          │ ◀───────────────────    │             │
└──────────┘                         └─────────────┘
```

- Every refresh returns a **fresh pair** and invalidates the previous refresh token.
- `POST /Authentication/revoke-token/{email}` instantly signs a user out everywhere (admin power).
- Roles are encoded as claims inside the JWT — no extra DB round-trip per request.

---

## ⚡ Caching Strategy

```
         ┌──────────────────────┐
         │   GET /api/Products  │
         └──────────┬───────────┘
                    │
           ┌────────▼────────┐
           │   Redis hit?    │
           └────┬──────┬─────┘
              yes      no
               │        │
       ┌───────▼──┐  ┌──▼────────────────┐
       │ Return   │  │ Query SQL Server  │
       │ cached   │  │ (Specifications)  │
       │ JSON     │  │                   │
       └──────────┘  │ Serialize w/      │
                     │ camelCase policy  │
                     │ SET with TTL      │
                     └────────┬──────────┘
                              ▼
                        return JSON
```

**Invalidation:** on `POST/PUT/PATCH/DELETE`, the affected cache keys (e.g. `products:all`, `products:{id}`, `order:{id}`) are explicitly evicted — no "wait-for-TTL-to-expire" bugs.

---

## 💳 Payments

- **Stripe** — card payments via `PaymentIntent` API. Webhooks update the order status to `PaymentReceived`.
- **Fawaterak** — Egyptian payment gateway. Integration guide PDF included in the repo (`fawaterak-integration-guide.pdf`).

Both live behind `IPaymentService` so the controller doesn't care which one ran.

---

## 🏃 Running Locally

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Docker Desktop (for SQL Server + Redis)

### Option A — Docker Compose (easiest)

```bash
git clone https://github.com/sefffo/Web-API-Revision.git
cd Web-API-Revision
docker-compose up --build
```

→ Swagger opens at **http://localhost:5262/swagger**

This spins up **SQL Server 2022 + Redis 7 + the API** on a private Docker network with health-checks gating startup order.

### Option B — Native dotnet + local SQL Server

```bash
dotnet restore
dotnet ef database update --project ECommerce.Persistence --startup-project ECommerce.Web
dotnet run --project ECommerce.Web
```

Configure `appsettings.Development.json` with your local connection strings first.

---

## 🐳 Docker Deployment

**Image:** [`saif31/ecomm-api:latest`](https://hub.docker.com/r/saif31/ecomm-api) (published to Docker Hub)

### Multi-stage Dockerfile — how & why

```dockerfile
# Stage 1: SDK image (~800MB) — compiles the code
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Trick: copy .csproj files FIRST, restore, THEN copy source.
# Result: changing a .cs file reuses the cached restore layer → 10× faster rebuilds.
COPY *.slnx .
COPY */*.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish ECommerce.Web/ECommerce.Web.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime image (~220MB) — only what's needed to RUN
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ECommerce.Web.dll"]
```

- **Final image ≈ 220 MB** instead of ~800 MB — no SDK, no source code, no NuGet cache leaks
- Layer-cached restore = seconds-fast rebuilds on code-only changes
- Runs as PID 1 (exec form) so SIGTERM triggers graceful shutdown

### Run the published image anywhere

```bash
docker pull saif31/ecomm-api:latest
docker run -p 5262:8080 \
  -e ConnectionStrings__DefaultConnection="..." \
  -e ConnectionStrings__RedisConnection="..." \
  -e JwtOptions__securityKey="..." \
  saif31/ecomm-api:latest
```

---

## 🔁 CI/CD Pipeline

**File:** [`.github/workflows/azure-deploy.yml`](.github/workflows/azure-deploy.yml)

```
┌───────────────────────────────────────────────────────────────┐
│  git push origin master                                       │
└──────────────┬────────────────────────────────────────────────┘
               ▼
┌──────────────────────────────┐       ┌──────────────────────┐
│   GitHub Actions Runner      │       │   Azure App Service  │
│                              │       │   Web-API-Revesion   │
│  1. Checkout                 │       │   (East Asia)        │
│  2. Setup .NET 10 SDK        │       │                      │
│  3. dotnet restore           │       │  Receives: /publish  │
│  4. dotnet build -c Release  │       │  Zero-downtime swap  │
│  5. dotnet publish           │ ───▶  │  Live in ~45s        │
│  6. azure/webapps-deploy@v3  │       │                      │
└──────────────────────────────┘       └──────────────────────┘
```

- **Trigger:** `push` to `master` + manual `workflow_dispatch`
- **Secret:** `AZURE_WEBAPP_PUBLISH_PROFILE` stored in GitHub repo secrets
- **Deploy target:** `Web-API-Revesion` on Azure App Service
- **Deploy time:** ~90 seconds end-to-end

---

## 📖 API Reference

Full interactive docs at the **[Swagger UI](https://web-api-revesion-c2chh0cyctd7dpcn.eastasia-01.azurewebsites.net/swagger/index.html)**. Highlights:

### Authentication
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/Authentication/Register` | Create account |
| POST | `/api/Authentication/Login` | Returns access + refresh tokens |
| POST | `/api/Authentication/RefreshToken` | Rotate tokens |
| POST | `/api/Authentication/revoke-token/{email}` | Sign user out (Admin) |
| POST | `/api/Authentication/AssignRole` | Change user role (SuperAdmin) |
| GET  | `/api/Authentication/CurrentUser` | Me + role |
| GET  | `/api/Authentication/users` | List users (Admin) |

### Products
| Method | Endpoint | Description |
|---|---|---|
| GET  | `/api/Products` | Paginated, filterable, sortable |
| GET  | `/api/Products/{id}` | By id |
| GET  | `/api/Products/brands` / `/types` | Taxonomies |
| POST | `/api/Products` | Create (Admin) |

### Orders
| Method | Endpoint | Description |
|---|---|---|
| POST  | `/api/Order` | Checkout |
| GET   | `/api/Order/{id}` | Detail |
| GET   | `/api/Order/Admin/AllOrders` | Admin list |
| PATCH | `/api/Order/{orderId}/status` | Admin status update |

### Basket
| Method | Endpoint |
|---|---|
| GET/POST/DELETE | `/api/Basket/{id}` |

---

## 🖼️ Dashboard Preview

The React admin dashboard consumes this API:

| Page | Desktop | Mobile |
|---|---|---|
| Overview (KPIs + chart) | ✅ | ✅ |
| Orders (table → cards) | ✅ | ✅ |
| Order Detail (status machine) | ✅ | ✅ |
| Products (image grid) | ✅ | ✅ |
| Users (role management) | ✅ | ✅ |

**Try it:** <https://ecommerce-dashboard-one-tawny.vercel.app/> with `superadmin@ecommerce.com` / `SuperAdmin@123`.

Frontend repo: <https://github.com/sefffo/ecommerce-dashboard>

---

## 🎓 What I Learned

This project was my deep-dive into **production software engineering**, not tutorials:

| Area | Takeaway |
|---|---|
| **Clean Architecture** | Why rings matter — swapping SQL Server or Redis would touch 1 project, not 7 |
| **Specifications** | Keeping LINQ out of services makes query logic testable and composable |
| **Result pattern** | Exceptions are for *exceptional* cases; business failures (validation / 404 / 403) are values |
| **Token rotation** | Security isn't just "use JWT" — it's rotation, revocation, and clock-skew handling |
| **Cache invalidation** | "One of the two hard problems in CS" — solved here with explicit key eviction on every write |
| **JSON casing bug** | Hit a real production bug where cached Pascal-case JSON leaked through; fixed at the serializer layer |
| **Docker multi-stage** | 72% smaller final image + faster rebuilds via layer caching trick |
| **docker-compose health-checks** | `depends_on: service_healthy` prevents "DB not ready" race conditions |
| **Azure App Service** | Remote publish profile → GitHub Actions → zero-downtime deploy |
| **CORS** | `AllowCredentials()` requires explicit origin predicates (can't use `AllowAnyOrigin`) |
| **SignalR of DI** | Extension methods on `IServiceCollection` keep `Program.cs` readable at 7 projects' worth of registrations |

---

<div align="center">

### 🔗 Related

[**Frontend (React Dashboard)**](https://github.com/sefffo/ecommerce-dashboard) · [**Docker Hub**](https://hub.docker.com/r/saif31/ecomm-api) · [**Live Swagger**](https://web-api-revesion-c2chh0cyctd7dpcn.eastasia-01.azurewebsites.net/swagger/index.html) · [**Live Dashboard**](https://ecommerce-dashboard-one-tawny.vercel.app/)

---

**Built by [Saif Lotfy](https://www.linkedin.com/in/saif-lotfy-769451310/)** — backend engineer, Cairo 🇪🇬

*If this project helped you, a ⭐ on the repo would mean the world.*

</div>
