# Running the API with Docker (Front-end guide)

## Prerequisites
Docker Desktop (Compose v2). Nothing else — no .NET SDK or SQL Server needed.

## Start
```bash
docker compose up --build -d      # first run takes a few minutes
docker compose ps                 # wait until db is healthy and api is running
```
Optional: `copy .env.example .env` to override defaults.

## URLs
| What | URL |
|---|---|
| API | http://localhost:5000 |
| Scalar docs | http://localhost:5000/scalar |
| Swagger UI | http://localhost:5000/swagger |
| OpenAPI JSON | http://localhost:5000/openapi/v1.json |
| SignalR chat hub | `http://localhost:5000/chathub` |
| Email inbox (Mailpit) | http://localhost:8025 |
| SQL Server | `localhost,1433` — user `sa`, password `Dev_Pass_123!`, DB `GymAssistant` |

The database is created, migrated and seeded automatically on startup (login route: `POST /api/Auth/login`). All emails (reset password, etc.) land in Mailpit.

## Seeded accounts
| Role | Email | Password |
|---|---|---|
| Admin | admin@gymassistant.com | Admin123! |
| Trainer | trainer@gymassistant.com | Trainer123! |
| Trainer | trainer2@gymassistant.com | Trainer2123! |
| Client | see `Data/ApplicationDbContextInitialiser.cs` | Client123! / Client2123! |

## CORS
Allowed origins in dev: `http://localhost:3000`, `:5173`, `:4200`. To add more, add `Cors__AllowedOrigins__N` entries in `docker-compose.yml` and run `docker compose up -d`.

## Useful commands
```bash
docker compose logs -f api        # API logs
docker compose up --build -d api  # rebuild after backend changes
docker compose down               # stop (keeps data)
docker compose down -v            # stop and wipe DB + uploads
```

## Notes
- Google/Facebook login use placeholder credentials in Docker (social login won't work locally).
- Push notifications (Firebase): uncomment the volume line in `docker-compose.yml` and place `Firebase/fitrix-adminsdk.json`.
- Never put real secrets in compose/.env files committed to git.
