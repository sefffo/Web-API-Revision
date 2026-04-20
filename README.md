<h1 align="center">🛒 ECommerce .NET Web API</h1>

<p align="center">
  A fully-featured, production-deployed RESTful E-Commerce API built with <strong>ASP.NET Core</strong>, following <strong>Clean Architecture</strong> principles.
</p>

<p align="center">
  <a href="https://web-api-revesion-c2chh0cyctd7dpcn.eastasia-01.azurewebsites.net/swagger">
    <img src="https://img.shields.io/badge/Live%20API-Swagger%20UI-85EA2D?style=for-the-badge&logo=swagger&logoColor=black" alt="Swagger UI" />
  </a>
  &nbsp;
  <a href="https://hub.docker.com/repository/docker/saif31/ecomm-api/general">
    <img src="https://img.shields.io/badge/Docker%20Hub-saif31%2Fecomm--api-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker Hub" />
  </a>
  &nbsp;
  <a href="https://github.com/sefffo/Web-API-Revision/actions">
    <img src="https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?style=for-the-badge&logo=githubactions&logoColor=white" alt="GitHub Actions" />
  </a>
</p>

---

## 🌐 Live Links

| Resource | URL |
|---|---|
| 🚀 **Live API (Swagger)** | [web-api-revesion-c2chh0cyctd7dpcn.eastasia-01.azurewebsites.net/swagger](https://web-api-revesion-c2chh0cyctd7dpcn.eastasia-01.azurewebsites.net/swagger) |
| 🐳 **Docker Hub Image** | [hub.docker.com/r/saif31/ecomm-api](https://hub.docker.com/repository/docker/saif31/ecomm-api/general) |
| 💼 **Portfolio** | [saif-lotfydev.vercel.app](https://saif-lotfydev.vercel.app/) |
| 🔗 **LinkedIn** | [linkedin.com/in/saif-lotfy](https://www.linkedin.com/in/saif-lotfy-769451310/) |

---

## 📋 Table of Contents

- [About the Project](#-about-the-project)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Features](#-features)
- [Project Structure](#-project-structure)
- [API Endpoints](#-api-endpoints)
- [Running Locally](#-running-locally)
- [Running with Docker](#-running-with-docker)
- [Deployment](#-deployment)
- [Environment Variables](#-environment-variables)
- [Design Diagrams](#-design-diagrams)
- [Author](#-author)

---

## 📖 About the Project

This is a production-grade E-Commerce REST API built as a deep-dive revision and learning project. It covers advanced backend concepts including Clean Architecture, the Repository & Unit of Work patterns, JWT Authentication, Google OAuth, Redis caching, payment gateway integration, and full cloud deployment on Microsoft Azure with CI/CD via GitHub Actions.

---

## 🏗 Architecture

The solution follows **Clean Architecture** with strict separation of concerns across 6 layers:

```
ECommerce.Domain                  →  Entities, Interfaces, Business Rules
ECommerce.Persistence             →  EF Core DbContexts, Repositories, Migrations, Data Seeding
ECommerce.Services.Abstraction    →  Service Interfaces (contracts)
ECommerce.Service                 →  Service Implementations, AutoMapper Profiles
ECommerce.SharedLibrary           →  DTOs, Settings, Shared Models
ECommerce.Web                     →  ASP.NET Core Web API (Controllers, Middlewares, Program.cs)
```

### Design Patterns Used

- ✅ **Repository Pattern** — abstracts data access logic
- ✅ **Unit of Work Pattern** — manages transactions across multiple repositories
- ✅ **Specification Pattern** — encapsulates query logic (filtering, sorting, paging)
- ✅ **Result Pattern** — consistent API response wrapping
- ✅ **Factory Pattern** — custom API validation responses

---

## 🛠 Tech Stack

### Backend

| Technology | Purpose |
|---|---|
| **ASP.NET Core (.NET 10)** | Web API Framework |
| **Entity Framework Core** | ORM for SQL Server |
| **ASP.NET Core Identity** | User management & roles |
| **StackExchange.Redis** | Basket & caching (via Upstash) |
| **AutoMapper** | Object-to-object mapping |
| **JWT Bearer** | Token-based authentication |
| **Google OAuth 2.0** | External authentication |
| **Fawaterak** | Payment gateway integration |

### Infrastructure & DevOps

| Technology | Purpose |
|---|---|
| **Azure App Service** | Cloud hosting (East Asia) |
| **Azure SQL Server** | Production database |
| **Upstash Redis** | Managed Redis (free tier) |
| **Docker** | Containerization |
| **GitHub Actions** | CI/CD pipeline |
| **Swagger / OpenAPI** | API documentation |

---

## ✨ Features

- 🔐 **JWT Authentication** with refresh tokens
- 🔑 **Google OAuth 2.0** external login
- 👤 **Role-based authorization** (Admin / User)
- 🛍️ **Product management** with image upload support
- 🧺 **Shopping basket** powered by Redis (TTL-based expiry)
- 📦 **Order management** with full order lifecycle
- 💳 **Payment integration** via Fawaterak gateway
- ⚡ **Response caching** with Redis
- 📄 **Pagination, Filtering & Sorting** via Specification Pattern
- 🌱 **Auto database migrations & seeding** on startup
- 🐳 **Dockerized** with Docker Compose support
- 🚀 **CI/CD** — auto deploys to Azure on every push to `master`

---

## 📁 Project Structure

```
📦 ECommerceSolution
 ┣ 📂 ECommerce.Domain
 ┃ ┣ 📂 Entities
 ┃ ┃ ┣ 📂 ProductModule
 ┃ ┃ ┣ 📂 OrderModule
 ┃ ┃ ┣ 📂 BasketModule
 ┃ ┃ └ 📂 IdentityModule
 ┃ └ 📂 Interfaces
 ┣ 📂 ECommerce.Persistence
 ┃ ┣ 📂 Data
 ┃ ┃ ┣ 📂 DbContexts
 ┃ ┃ ┣ 📂 DataSeed
 ┃ ┃ └ 📂 IdentityData
 ┃ └ 📂 Repositories
 ┣ 📂 ECommerce.Services.Abstraction
 ┣ 📂 ECommerce.Service
 ┃ ┣ 📂 Services
 ┃ └ 📂 MappingProfiles
 ┣ 📂 ECommerce.SharedLibrary
 ┣ 📂 ECommerce.Web
 ┃ ┣ 📂 Controllers
 ┃ ┣ 📂 Extensions
 ┃ ┣ 📂 CustomMiddleWares
 ┃ ┣ 📂 Factories
 ┃ └ 📄 Program.cs
 ┣ 📄 Dockerfile
 ┣ 📄 docker-compose.yml
 └ 📂 .github/workflows
    └ 📄 azure-deploy.yml
```

---

## 📡 API Endpoints

### 🔐 Authentication

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/Authentication/register` | Register new user |
| `POST` | `/api/Authentication/login` | Login with JWT response |
| `GET` | `/api/Authentication/google-login` | Initiate Google OAuth |
| `POST` | `/api/Authentication/refresh-token` | Refresh JWT token |

### 🛍️ Products

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/Products` | Get all products (with filtering & paging) |
| `GET` | `/api/Products/{id}` | Get product by ID |
| `POST` | `/api/Products` | Create product *(Admin only)* |
| `PUT` | `/api/Products/{id}` | Update product *(Admin only)* |
| `DELETE` | `/api/Products/{id}` | Delete product *(Admin only)* |

### 🧺 Basket

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/Basket/{id}` | Get customer basket |
| `POST` | `/api/Basket` | Create or update basket |
| `DELETE` | `/api/Basket/{id}` | Delete basket |

### 📦 Orders

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/Order` | Create new order |
| `GET` | `/api/Order` | Get user orders |
| `GET` | `/api/Order/{id}` | Get order by ID |

### 💳 Payment

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/Payment` | Initiate payment via Fawaterak |
| `GET` | `/api/Payment/success` | Payment success callback |
| `GET` | `/api/Payment/fail` | Payment failure callback |
| `POST` | `/api/Payment/callback` | Webhook callback |

> 📄 Full interactive docs available at the **[Live Swagger UI](https://web-api-revesion-c2chh0cyctd7dpcn.eastasia-01.azurewebsites.net/swagger)**

---

## 💻 Running Locally

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (local instance)
- [Redis](https://redis.io/download/) (local instance or use Upstash)

### Steps

```bash
# 1. Clone the repo
git clone https://github.com/sefffo/Web-API-Revision.git
cd Web-API-Revision

# 2. Update appsettings.json with your local connection strings
# DefaultConnection, IdentityConnection, RedisConnection

# 3. Restore & Run
dotnet restore
dotnet run --project ECommerce.Web/ECommerce.Web.csproj
```

The app will automatically run migrations and seed the database on first startup.

Open Swagger at: `http://localhost:5262/swagger`

---

## 🐳 Running with Docker

### Option 1 — Pull from Docker Hub

```bash
docker pull saif31/ecomm-api:latest
docker run -p 8080:8080 saif31/ecomm-api:latest
```

🐳 Docker Hub: [hub.docker.com/r/saif31/ecomm-api](https://hub.docker.com/repository/docker/saif31/ecomm-api/general)

### Option 2 — Docker Compose (Full Stack)

```bash
# Clone the repo
git clone https://github.com/sefffo/Web-API-Revision.git
cd Web-API-Revision

# Start all services (API + SQL Server + Redis)
docker compose up --build
```

This spins up the API, SQL Server, and Redis together automatically.

---

## 🚀 Deployment

The project is deployed on **Microsoft Azure** with full CI/CD via **GitHub Actions**.

### Infrastructure

| Component | Service |
|---|---|
| **App Hosting** | Azure App Service (Free tier, East Asia) |
| **Database** | Azure SQL Server — 2 databases (Free tier) |
| **Cache / Basket** | Upstash Redis (Free tier, managed) |
| **CI/CD** | GitHub Actions → auto deploy on push to `master` |

### Deployment Flow

```
Push to master
     ↓
GitHub Actions triggered
     ↓
Ubuntu VM: Restore → Build → Publish
     ↓
Deploy to Azure App Service
     ↓
App starts → Auto-migrate DBs → Seed data
     ↓
Live at Azure URL 🚀
```

---

## ⚙️ Environment Variables

The following environment variables must be set in Azure (or your local `appsettings.json`):

```
ConnectionStrings__DefaultConnection      →  Azure SQL main DB connection string
ConnectionStrings__IdentityConnection     →  Azure SQL identity DB connection string
ConnectionStrings__RedisConnection        →  Upstash Redis connection string

JwtOptions__Issuer                        →  API base URL
JwtOptions__Audience                      →  API audience URL
JwtOptions__securityKey                   →  JWT signing key (secret)

GoogleOAuth__ClientId                     →  Google OAuth Client ID
GoogleOAuth__ClientSecret                 →  Google OAuth Client Secret

FawaterakSettings__SuccessUrl             →  Payment success redirect URL
FawaterakSettings__FailUrl                →  Payment fail redirect URL
FawaterakSettings__PendingUrl             →  Payment pending redirect URL
FawaterakSettings__CallbackUrl            →  Payment webhook callback URL

URLs__BaseUrl                             →  App base URL (for image serving)
```

> ⚠️ Never commit real credentials to the repository. Use Azure Environment Variables or `dotnet user-secrets` locally.

---

## 🎨 Design Diagrams

The repo includes Excalidraw architecture diagrams for key concepts:

| Diagram | Description |
|---|---|
| `JWT Auth.excalidraw.png` | JWT authentication flow |
| `Identity Module.excalidraw.png` | ASP.NET Identity setup |
| `OAuth Steps.excalidraw.png` | Google OAuth 2.0 flow |
| `ExplainningRefreshToken.excalidraw.png` | Refresh token lifecycle |
| `Specification design pattern.excalidraw.png` | Specification pattern explained |
| `ResultPattern.excalidraw.png` | Result pattern for API responses |
| `Adding Create Product Endpoint for admin.excalidraw.png` | Admin product endpoint flow |

---

## 👨‍💻 Author

**Saif Lotfy** — Backend Engineer & Student

| | |
|---|---|
| 🌐 **Portfolio** | [saif-lotfydev.vercel.app](https://saif-lotfydev.vercel.app/) |
| 💼 **LinkedIn** | [linkedin.com/in/saif-lotfy](https://www.linkedin.com/in/saif-lotfy-769451310/) |
| 📧 **Email** | [saiflotfy26@gmail.com](mailto:saiflotfy26@gmail.com) |
| 📱 **Phone** | +20 1277934002 |

---

<p align="center">Built with ❤️ using ASP.NET Core &nbsp;·&nbsp; Deployed on Azure &nbsp;·&nbsp; Dockerized</p>
