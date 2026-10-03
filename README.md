# GymAssistant_API

ASP.NET Core 9 Web API (EF Core + SQL Server, Identity/JWT, SignalR) for the Gym Assistant app.

## Quick start with Docker (recommended for front-end devs)

**Requirements:** Docker Desktop only (no .NET SDK, no SQL Server install).

```bash
git clone <repo-url>
cd GymAssistant_API
docker compose up --build -d
```
The first run takes a few minutes. When `docker compose ps` shows `gymassistant-api` as Up, the API is ready. The database is created, migrated and seeded automatically.

| What | URL |
|---|---|
| API base | http://localhost:5000 |
| Scalar docs | http://localhost:5000/scalar |
| Swagger UI | http://localhost:5000/swagger |
| SignalR hub | http://localhost:5000/chathub |
| Mail inbox (Mailpit) | http://localhost:8025 |
| SQL Server | `localhost,1433` — `sa` / `Dev_Pass_123!` / DB `GymAssistant` |

### Test login
```bash
curl -X POST http://localhost:5000/api/Auth/login -H "Content-Type: application/json" \
  -d '{"email":"admin@gymassistant.com","password":"Admin123!"}'
```
Seeded accounts: `admin@gymassistant.com / Admin123!`, `trainer@gymassistant.com / Trainer123!`, `trainer2@gymassistant.com / Trainer2123!`, plus clients (`Client123!`, `Client2123!`).

### Common commands
```bash
docker compose logs -f api         # follow API logs
docker compose up --build -d api   # rebuild after backend changes
docker compose down                # stop (data kept)
docker compose down -v             # stop and wipe database + uploads
```

### Configuration
- Copy `.env.example` to `.env` to change DB password, JWT key or front-end URL.
- CORS allows `http://localhost:3000`, `:5173`, `:4200`. Add more via `Cors__AllowedOrigins__N` in `docker-compose.yml`.
- Emails (password reset, etc.) are captured by Mailpit, nothing is sent for real.
- Google/Facebook login uses placeholders in Docker, so social login doesn't work locally.
- Push notifications: uncomment the Firebase volume in `docker-compose.yml` and add `Firebase/fitrix-adminsdk.json` (gitignored).

### Troubleshooting
- **Port in use** (1433/5000/8025): stop the conflicting app or change the left side of the port mapping in `docker-compose.yml`.
- **API can't reach DB**: run `docker compose logs db db-init api`.
- **Start clean**: `docker compose down -v && docker compose up --build -d`.

More details: [docs/DOCKER.md](docs/DOCKER.md).

## Running without Docker
1. Install .NET 9 SDK and SQL Server.
2. Create `appsettings.json` (gitignored) with `ConnectionStrings:DefaultConnection`, `JWT`, `EmailSettings`, `Authentication:Google/Facebook` and `Firebase:CredentialPath`.
3. Create the empty database, then `dotnet run`. Migrations and seeding run on startup.

## Tests
See [tests/README.md](tests/README.md). `docker-compose.test.yml` starts the SQL Server used by integration/E2E tests.