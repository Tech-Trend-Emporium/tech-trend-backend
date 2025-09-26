# 🛒 Tech Trend Emporium

This repository contains the **Tech Trend Emporium** built with **.NET 8.0.0** and **PostgreSQL**.  
The system follows a **microservices architecture**, with services aligned to key business domains such as **Identity**, **Catalog**, **Cart**, **Wishlist**, **Reviews**, and **Approval Jobs**.  

The platform is designed to practice **modern software architecture**, **CI/CD pipelines**, and **cloud deployment strategies**.

---

## 📌 Vision

- Provide a **modular and scalable** e-commerce backend.
- Demonstrate the application of **Clean Architecture**, **Domain-Driven Design (DDD)**, and **microservices principles** in .NET.
- Integrate with **external APIs** (FakeStore API) for initial seeding of products and categories.
- Support **dynamic authentication and authorization** with multiple roles: `SUPERADMIN`, `ADMIN`, `EMPLOYEE`, `CUSTOMER`.
- Enable **continuous integration and deployment (CI/CD)** with Docker, GitHub Actions, and AWS.

---

## 🏗️ Architecture Overview

The solution is structured as a **set of independent services** communicating via HTTP and asynchronous messaging (event bus).  

**Microservices:**
1. **Identity API** – signup, login, logout, role-based access control.
2. **Catalog API** – products, categories, integration with FakeStore API.
3. **Approval Jobs API** – superadmin workflows for approvals/declines.
4. **User Admin API** – superadmin endpoints for managing users.
5. **Cart API** – shopping cart, coupons, totals.
6. **Wishlist API** – customer wishlists.
7. **Review API** – product reviews and ratings.
8. **Seeder Service** – background job to sync with FakeStore API.

**Shared Infrastructure:**
- **API Gateway** – routing, authentication, rate limiting.
- **Event Bus** – RabbitMQ (or AWS SNS/SQS).
- **Observability** – OpenTelemetry for logs, metrics, and traces.

---

## 🛠️ Tech Stack

- **.NET 8.0.0** (Minimal APIs + Clean Architecture modules)
- **PostgreSQL 15+**
- **Docker & Docker Compose**
- **Entity Framework Core** (migrations per service)
- **MediatR** for CQRS and domain events
- **MassTransit** for messaging (RabbitMQ/SQS)
- **Swagger / OpenAPI** for documentation
- **Serilog** + **OpenTelemetry** for logging & tracing
- **GitHub Actions** for CI/CD
- **AWS ECS/EKS** (deployment target)

---

## 📂 Repository Structure

```plaintext
.github/workflows/      # CI/CD pipelines
/docs/                  # Documentation (ADR, API contracts, diagrams)
/gateway/               # API Gateway (YARP or BFF)
/services/
  identity/             # Identity API
  catalog/              # Catalog API + Seeder
  approval/             # Approval Jobs API
  useradmin/            # User Admin API
  cart/                 # Cart API
  wishlist/             # Wishlist API
  review/               # Review API
/infra/                 # Infrastructure as Code (Terraform, ECS task defs, K8s manifests)
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
git clone https://github.com/<org>/<repo>.git
cd <repo>
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
cd services/catalog
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

## 🔄 CI/CD

CI: On every push to a feature/* branch
- Restore, build, test
- Build Docker image tagged with commit SHA
- Push to container registry

CD: On merge to main
- Deploy automatically to AWS ECS/EKS
- Notify team via Slack/Teams
- See workflows under .github/workflows/.

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
- Approval workflow for products & categories
- Cart, Wishlist, Reviews modules
- CI/CD pipelines with GitHub Actions
- Deployment to AWS ECS (Dev/Prod environments)
- Observability (metrics, logs, traces)

---

## 📜 License

MIT License. See LICENSE