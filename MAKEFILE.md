# 📘 MAFEFILE – Tech Trend Emporium Internal Setup

This file contains **internal environment details** and **test data** for developers and testers.


## 👥 Test Users (Seeded)

| ID | Email              | Username   | First Name | Last Name | Password    |
|----|--------------------|-------------|-------------|------------|-------------|
| 1  | john@gmail.com     | johnd       | john        | doe        | m38rmF$     |
| 2  | morrison@gmail.com | mor_2314    | david       | morrison   | 83r5^_      |
| 3  | kevin@gmail.com    | kevinryan   | kevin       | ryan       | kev02937@   |
| 4  | don@gmail.com      | donero      | don         | romer      | ewedon      |
| 5  | derek@gmail.com    | derek       | derek       | powell     | jklg*_56    |
| 6  | david_r@gmail.com  | david_r     | david       | russell    | 3478*#54    |
| 7  | miriam@gmail.com   | snyder      | miriam      | snyder     | f238&@*$    |
| 8  | william@gmail.com  | hopkins     | william     | hopkins    | William56$hj|
| 9  | kate@gmail.com     | kate_h      | kate        | hale       | kfejk@*_    |
| 10 | jimmie@gmail.com   | jimmie_k    | jimmie      | klein      | klein*#%*   |

> ⚠️ These users are seeded **for testing only** and do not exist in production environments.


## 🔑 Key Vault Configuration (Azure)

Secrets stored in **Azure Key Vault (AKV)** include:
- `DbConnectionString`
- `JwtSecretKey`

During deployment:
- Containers fetch secrets at startup.
- The `ConnectionStrings__Default` is overridden dynamically from AKV.


## 🌍 Local Environment Overview

| Component | Service | URL |
|------------|----------|-----|
| API | TechTrend API | http://localhost:8080/swagger |
| DB | PostgreSQL | localhost:5433 |
| Azure Container | techtrend-api | Runs API in production |
| Key Vault | techtrend-akv | Holds secrets and connection strings |

## 🧰 Useful Commands

**Rebuild everything:**
```bash
docker compose down -v && docker compose up --build
```

**Check logs (API):**

```bash
docker compose logs api -f
```

**Access DB shell:**
```bash
docker compose exec db psql -U postgres -d techtrend_db
```

## 🧠 Notes

- Use the provided test accounts to validate authentication and roles.
- Always reapply migrations after schema changes.
- AKV integration is active only in Azure deployments (not local).
- Ensure Docker is using Linux containers for compatibility.