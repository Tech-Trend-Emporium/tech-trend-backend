# 🛒 Tech Trend Emporium

This repository contains **Tech Trend Emporium**, built with **.NET 8.0.0** and **PostgreSQL**.  
The system follows a **microservices-inspired modular architecture** with multiple **DbContexts** (one per bounded context) pointing to the **same database** for learning purposes.

> **Note:** In production, true microservices usually own **separate databases**. Here we keep **one DB** to reduce operational overhead while preserving clean boundaries in code.

---

## 📌 Vision

- Provide a **modular and scalable** e-commerce backend.
- Demonstrate **Clean Architecture**, **DDD boundaries**, and **microservice-aligned** practices in .NET.
- Integrate with **external APIs** (FakeStore API) for initial seeding of products and categories.
- Support **authentication & authorization** with roles: `ADMIN`, `EMPLOYEE`, `SHOPPER` (an optional `SUPERADMIN` can be added for governance flows).
- Enable **CI/CD** with Docker, GitHub Actions, and **Azure**.

---

## 🏗️ Architecture Overview

The solution is structured as a set of **bounded contexts**, each with its **own DbContext** and tables it owns.  
Contexts communicate via HTTP and (optionally) asynchronous messaging.

**Contexts / Services:**
1. **Identity API** – signup, login, logout, role-based access control.
2. **Catalog API** – products, categories, inventory, reviews; seeding via FakeStore.
3. **Promotions API** – coupons and validation.
4. **Shopping API** – cart, cart items, wishlist, wishlist items.
5. **Governance API** – approval workflows for product/category operations.
6. **Seeder Service** – background job to sync with FakeStore API.

**Database Strategy (important):**
- **One PostgreSQL database / default schema**.
- **Multiple DbContexts** (Identity, Catalog, Promotions, Shopping, Governance).
- **Foreign keys only inside a context** (e.g., `Product → Category`, `CartItem → Cart`).
- **No physical FKs across contexts**: cross-context references are by **IDs/codes** (e.g., `Review.UserId`, `Cart.CouponCode`, `CartItem.ProductId`).  
  Integrity is enforced at the application layer (API calls or read models).

**Shared Infrastructure:**
- **API Gateway** – YARP (optionally fronted by **Azure API Management**).
- **Event Bus** – RabbitMQ (local/dev) or **Azure Service Bus** (cloud).
- **Observability** – **OpenTelemetry** + **Azure Application Insights / Azure Monitor**.

---

## 🛠️ Tech Stack

- **.NET 8.0.0** (Minimal APIs + modular Clean Architecture)
- **PostgreSQL 15+**
- **Docker & Docker Compose**
- **Entity Framework Core** (migrations **per context**)
- **MediatR** for CQRS and domain events
- **MassTransit** for messaging (RabbitMQ / **Azure Service Bus**)
- **Swagger / OpenAPI** for documentation
- **Serilog** + **OpenTelemetry** (exporters to **Application Insights**)
- **GitHub Actions** for CI/CD
- **Azure** targets: **Azure Kubernetes Service (AKS)** or **Azure Container Apps**, **Azure Container Registry (ACR)**

---

## 📂 Repository Structure

```plaintext
.github/workflows/      # CI/CD pipelines
/docs/                  # Documentation (ADR, API contracts, diagrams)
/gateway/               # API Gateway (YARP or BFF)
/services/
  identity/             # Identity API (Users, Sessions)
  catalog/              # Catalog API + Seeder (Products, Categories, Inventory, Reviews)
  promotions/           # Promotions API (Coupons)
  shopping/             # Shopping API (Cart, CartItems, Wishlist, WishlistItems)
  governance/           # Governance API (Approval Jobs)
/infra/                 # IaC (Bicep/Terraform), AKS/Container Apps manifests (Helm/Kustomize/YAML)
/tests/                 # Unit, Integration, Contract, E2E tests
```

---

## 🚀 Getting Started

### Prerequisites

- Docker
- Docker Compose
- .NET 8 SDK

### Clone Repository
```bash
git clone https://github.com/Tech-Trend-Emporium/tech-trend-backend.git
cd tech-trend-backend
```

Run Locally with Docker Compose
```bash
docker compose up --build
```


This will spin up:

- PostgreSQL (localhost:5432)
- RabbitMQ (management UI: http://localhost:15672)
- Microservices containers (each on different ports)

### Database Migrations

Run migrations per service:

```bash
# Catalog
cd services/catalog
dotnet ef database update -c CatalogDbContext

# Identity
cd ../identity
dotnet ef database update -c IdentityDbContext

# Promotions
cd ../promotions
dotnet ef database update -c PromotionsDbContext

# Shopping
cd ../shopping
dotnet ef database update -c ShoppingDbContext

# Governance
cd ../governance
dotnet ef database update -c GovernanceDbContext
```

Alternative: use a dedicated MigrationsDbContext that maps all tables only for schema evolution.

---

## 🧪 Testing

Each service includes unit and integration tests:

```bash
dotnet test
```

Contract and E2E tests are under /tests.

---

## 🔄 CI/CD (Azure)

CI (on every push/PR to main or feature/*):

- Restore, build, test
- Build Docker images tagged with commit SHA
- Push images to Azure Container Registry (ACR)

CD (on merge to main):

- Deploy to AKS (Helm/Kustomize) or Azure Container Apps
- Apply EF Core migrations per context (job or init container)
- Notify team via Slack/Teams
- See workflows under .github/workflows/

Use GitHub OIDC to authenticate to Azure (no long-lived secrets).
Environments: dev, prod with approvals and protection rules.

## 📖 Documentation

- Wiki: Branching strategy, PR guidelines, architectural decisions.
- ADR: Stored under /docs/adr/.
- API Docs: Each service exposes /swagger.

---

## 🤝 Contributing

We follow a Trunk-Based Development strategy:
- All changes are merged into main via Pull Request.
- Each PR requires 2 approvals.
- Keep PRs small and focused.

---

## 🗺️ Roadmap

- Identity + Catalog services (MVP)
- Governance (approval) workflow for products & categories
- Promotions (coupons) and Shopping (cart, wishlist)
- CI/CD with GitHub Actions → deploy to Azure (AKS/Container Apps)
- Observability via OpenTelemetry → Application Insights
- Optional: switch to separate databases per context as the system evolves

---

## 📜 License

MIT License. See LICENSE.
