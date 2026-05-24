# LibraryApi

ASP.NET Core Web API for a community library management system.

## Requirements

- Visual Studio 2022
- SQL Server LocalDB (included with Visual Studio)
- .NET 8 SDK

## Database Setup

Connection string used:

To get your LocalDB pipe name, run in PowerShell:

Update the connection string in `appsettings.json` with your pipe name.

## Authentication (OIDC via Keycloak)

This app uses Keycloak as the OIDC provider.

### Option A — Run local Keycloak (recommended)

```bash
docker run -d --name keycloak \
  -p 8080:8080 \
  -e KC_BOOTSTRAP_ADMIN_USERNAME=admin \
  -e KC_BOOTSTRAP_ADMIN_PASSWORD=admin \
  quay.io/keycloak/keycloak:latest start-dev
```

Then:
1. Go to http://localhost:8080
2. Login with admin/admin
3. Create realm: `library`
4. Create client: `library-api` (OpenID Connect, Direct Access Grants enabled)
5. Create test user with username/password

### Get a test token

```bash
curl -X POST http://localhost:8080/realms/library/protocol/openid-connect/token \
  -d "client_id=library-api" \
  -d "username=testuser" \
  -d "password=Test1234!" \
  -d "grant_type=password"
```

Use the `access_token` from the response as Bearer token.

### Option B — Point to reviewer's own OIDC provider

Update `appsettings.json`:
```json
"Authentication": {
  "Authority": "https://your-provider/realms/your-realm",
  "Audience": "library-api"
}
```

## Data Access

Uses **Entity Framework Core** with SQL Server. EF Core was chosen over Dapper for:
- Automatic migrations
- LINQ query support
- Relationship management between entities

## API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | /api/Books | Public | List books (filter by title/author) |
| POST | /api/Books | Public | Add new book |
| GET | /api/Books/{id} | Public | Get book with available copies |
| GET | /api/me | Required | Get current member profile |
| GET | /api/me/loans | Required | List active loans |
| POST | /api/loans/borrow/{bookId} | Required | Borrow a book |
| POST | /api/loans/return/{loanId} | Required | Return a book |

## Business Rules

- Maximum 3 active loans per member
- Cannot borrow if no copies available
- Members can only return their own loans
- Member provisioned automatically on first SSO sign-in

## Unit Tests

Run via Test > Run All Tests in Visual Studio.
3 tests covering: loan limit rule, availability check, SSO provisioning.