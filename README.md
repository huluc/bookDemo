# BookDemo API

A layered ASP.NET Core Web API for managing books — a portfolio project
demonstrating production-style patterns: Clean Architecture, JWT
authentication, API versioning, HATEOAS, caching strategies, and rate
limiting.

Built with **.NET 10**.

---

## 📖 Documentation

- **Interactive API Docs:** [bookdemo-api.docs.buildwithfern.com](https://bookdemo-api.docs.buildwithfern.com)
- **OpenAPI Spec (local):** available at `/scalar` when running the API locally, powered by `Microsoft.AspNetCore.OpenApi` + Scalar

## 🧪 Postman Collection

A Postman collection export is included for local testing:
[`BookDemo.postman_collection.json`](https://github.com/huluc/bookDemo/blob/master/bookDemo.postman_collection.json)
> This is a snapshot — for the always-up-to-date reference, see the
> [interactive docs](https://bookdemo-api.docs.buildwithfern.com).

To use it:
1. Download the file above
2. Open Postman → Import → select the file
3. Set the `baseURL` variable to your local API URL (e.g. `https://localhost:7093`)

---

## 🧱 Architecture

The project follows a Clean Architecture / layered approach with strict
dependency direction — inner layers know nothing about outer ones:

```
BookDemo.Domain          → entities, no external dependencies
BookDemo.Application     → business logic, service interfaces, DTOs
BookDemo.Infrastructure  → EF Core, Identity, caching, repositories
BookDemo.Presentation    → controllers, filters, HATEOAS formatting
BookDemo.API             → composition root (Program.cs, service wiring)
```

Layer independence is enforced deliberately — for example, `ApplicationUser`
lives in Infrastructure (not Domain) to avoid coupling the domain model to
Identity/EF Core, and Application service interfaces only expose primitive
types or dedicated DTOs.

## 🔑 Key Features

**API Design**
- Repository pattern with a `ServiceManager` facade
- API versioning (v1/v2) via `Asp.Versioning.Mvc`, with independent
  controllers and versioned service namespaces (`V1/IBookService`,
  `V2/IBookService`, shared `BookServiceBase`)
- HATEOAS with content negotiation, custom media types, and an open-generic
  `BookLinks<T>` link builder
- Global exception handling middleware

**Authentication & Authorization**
- ASP.NET Core Identity with a custom `ApplicationUser`
- JWT access tokens (HMAC-SHA256)
- Opaque, SHA256-hashed refresh tokens with rotation, reuse detection, and
  multi-device support
- Role-based authorization, with roles seeded on startup

**Performance**
- Two-layer HTTP caching: `HybridCache` for application-level caching
  (behind an `IBookCache` abstraction, with tag-based invalidation across
  both API versions) and `Marvin.Cache.Headers` for HTTP-level caching
  (`Cache-Control`, `ETag`, conditional requests)
- Per-IP rate limiting using the Token Bucket algorithm
  (`PartitionedRateLimiter`), with a custom 429 response including
  `Retry-After`

**Documentation**
- Versioned OpenAPI documents (v1/v2) via `Microsoft.AspNetCore.OpenApi`
- Interactive Scalar UI with Bearer JWT support, applied automatically to
  `[Authorize]` endpoints only
- Published, browsable documentation via Fern

---

## 🛠️ Tech Stack

| Category | Technology |
|---|---|
| Runtime | .NET 10 |
| ORM | Entity Framework Core |
| Database | SQL Server (LocalDB) |
| Auth | ASP.NET Core Identity, JWT Bearer |
| API Versioning | Asp.Versioning.Mvc |
| Documentation | Microsoft.AspNetCore.OpenApi, Scalar, Fern |
| Caching | HybridCache, Marvin.Cache.Headers |
| Logging | NLog |
| JSON Patch | Newtonsoft.Json |
| API Testing | Postman |

---

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server (LocalDB is sufficient for local development)

### Setup

```bash
# Clone the repository
git clone <repo-url>
cd BookDemo

# Apply database migrations
ef database update
# (or: dotnet ef database update, from the BookDemo.API project directory)

# Run the API
dotnet run --project BookDemo.API
```

The API runs on `https://localhost:7093` by default (see
`BookDemo.API/Properties/launchSettings.json`).

Once running, explore the API interactively at:

```
https://localhost:7093/scalar
```

---

## 📌 Roadmap

- [x] Layered architecture, repository pattern, HATEOAS
- [x] API versioning (v1/v2)
- [x] HTTP + application-level caching
- [x] Rate limiting
- [x] Identity, JWT auth, refresh tokens
- [x] OpenAPI/Scalar documentation
- [ ] Category feature
- [ ] Linux → Git → Docker/Azure

---

## 📝 License

This is a personal portfolio project built to explore and demonstrate
production-style ASP.NET Core Web API patterns.
