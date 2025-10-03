# 🛒 Tech Trend Emporium

This repository contains **Tech Trend Emporium**, built with **.NET 8.0.0** and **SQL Server** on Azure.  
The system follows a **layered architecture** (Application, Infrastructure, Domain, Presentation) aligned with **Clean Architecture best practices**.

---

## 📌 Vision

- Provide a **scalable and maintainable** e-commerce backend.
- Demonstrate **Clean Architecture** with clear separation of concerns.
- Integrate with **external APIs** (FakeStore API) for initial seeding of products and categories.
- Support **authentication & authorization** with roles: `ADMIN`, `EMPLOYEE`, `SHOPPER`.
- Enable **CI/CD** with Docker, GitHub Actions, and **Azure**.

---

## 🏗️ Architecture Overview

The solution is structured into **four layers**:

1. **Domain** – enterprise business rules, entities, and core abstractions.
2. **Application** – use cases, business logic, interfaces, and DTOs.
3. **Infrastructure** – EF Core, repositories, external services, Azure integrations.
4. **Presentation** – ASP.NET Core Web API, controllers, filters, middlewares.

**Database Strategy:**
- **One SQL Server database** (Azure SQL for production).
- Schema evolution managed with **Entity Framework Core migrations**.
- Domain integrity is enforced through the **Application layer**.

**Deployment Strategy:**
- Services packaged into Docker images.
- Images stored in **Azure Container Registry (ACR)**.
- Containers deployed to **Azure Container Instances (ACI)**.
- Secrets and connection strings managed with **Azure Key Vault (AKV)**.

---

## 🛠️ Tech Stack

- **.NET 8.0.0** (ASP.NET Core Web API + Clean Architecture)
- **Azure SQL Database**
- **Docker & Docker Compose**
- **Entity Framework Core** (migrations managed centrally)
- **Swagger / OpenAPI** for documentation
- **Serilog** + **OpenTelemetry** (exporters to **Application Insights**)
- **GitHub Actions** for CI/CD
- **Azure** targets: **Azure Container Registry (ACR)**, **Azure Container Instances (ACI)**, **Azure Key Vault (AKV)**

---

## 📂 Repository Structure

```plaintext
.github/workflows/      # CI/CD pipelines
/docs/                  # Documentation (ADR, diagrams)
Application/            # Application layer (use cases, services, interfaces, DTOs)
Domain/                 # Domain layer (entities, enums, aggregates, core logic)
Infrastructure/         # Infrastructure layer (EF Core, repositories, integrations)
Starter/                # Seed from external API
TechTrendEmporium/      # Presentation layer (Web API, controllers, middlewares)
/tests/                 # Unit, Integration, E2E tests
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
- API container (localhost:5000)

### Database Migrations

Run migrations from the Infrastructure project:

```bash
cd Infrastructure/Persistence/Migrations/App
dotnet ef database update
```

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
- Deploy container to Azure Container Instances (ACI)
- Apply EF Core migrations automatically at startup
- Retrieve secrets and connection strings from Azure Key Vault (AKV)
- Notify team via Slack/Teams

Workflows are defined under .github/workflows/.

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

- Complete layered architecture refactoring (MVP).
- Integrate Azure Key Vault for production secrets.
- Improve CI/CD with GitHub Actions + ACR + ACI.
- Observability with OpenTelemetry → Application Insights.
- Add automated integration tests with Azure SQL.

---

## 📜 License

MIT License. See LICENSE.
