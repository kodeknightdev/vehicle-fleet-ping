# fleet-ping

Vehicle tracking and maintenance log for a small delivery fleet: where the vans are,
what has been done to them, and which ones are due for a service.

```
fleet-ping/
├── backend/            ASP.NET Core 3.1 Web API + EF Core 3.1 (PostgreSQL)
├── frontend/           Angular 9 dashboard, Leaflet maps
└── docker-compose.yml  db + api + web
```

## Quick start (Docker)

```bash
docker-compose up --build
```

| Service | URL |
|---------|-----|
| Dashboard | http://localhost:8080 |
| API | http://localhost:5000/api |
| Swagger | http://localhost:5000/swagger |
| Postgres | localhost:5432 (fleet / fleet) |

Sign in with `dispatch` / `dispatch`. The compose setup seeds a few demo vehicles and runs
the GPS simulator, so markers start moving on the map after a few seconds.

## Local development

Requirements: .NET Core 3.1 SDK, Node 12, PostgreSQL 12.

```bash
# database only
docker-compose up -d db

# api on http://localhost:5000 (migrates + seeds on startup in Development)
cd backend/FleetPing.Api
dotnet run

# frontend on http://localhost:4200
cd frontend
npm ci
npm start
```

New migration after changing the model:

```bash
cd backend/FleetPing.Api
dotnet ef migrations add SomeChange
```

## API overview

All endpoints except `/api/health` require HTTP Basic auth. Users are configured under `Auth:Users`.

| Method | Path | |
|--------|------|-|
| GET | `/api/health` | liveness |
| GET | `/api/auth/me` | current user, used by the login page |
| GET/POST | `/api/vehicles` | list / create |
| GET/PUT/DELETE | `/api/vehicles/{id}` | |
| GET/POST | `/api/vehicles/{id}/maintenance` | list (`from`, `to`, `type` filters) / create |
| GET/PUT/DELETE | `/api/vehicles/{id}/maintenance/{logId}` | |
| POST | `/api/vehicles/{id}/pings` | tracker position report |
| GET | `/api/vehicles/{id}/pings?from=&to=` | position history (trail) |
| GET | `/api/vehicles/{id}/position` | last known position |
| GET | `/api/positions` | last known position of every non-retired vehicle |
| GET | `/api/dashboard` | counts, GPS reporting, 30 day spend, service due |

Example tracker report:

```bash
curl -u dispatch:dispatch -H "Content-Type: application/json" \
  -d '{"latitude":45.52,"longitude":-122.67,"speedKph":32,"headingDeg":90}' \
  http://localhost:5000/api/vehicles/1/pings
```

### Positions

Last known position comes from the newest GPS ping. Vehicles that have never reported get a
fixed mock position near the depot (`source: "mock"`) so they still show up on the map.

The GPS simulator (`Simulator:Enabled`) is a background service that moves every active
vehicle around the depot and writes a ping every `Simulator:IntervalSeconds`.

### Service due

Each vehicle has a `serviceIntervalKm` (default 15,000). The last oil change or inspection in
the maintenance log resets the interval; a vehicle within 1,000 km of the next service, or past
it, is flagged as due.

## Configuration

| Key | Default | |
|-----|---------|-|
| `ConnectionStrings:Fleet` | | Npgsql connection string |
| `Auth:Users` | | list of `{ Username, Password, DisplayName }` |
| `Cors:Origins` | `http://localhost:4200` | allowed browser origins |
| `MigrateOnStartup` | `true` | apply EF migrations on boot |
| `SeedDemoData` | `false` | insert demo vehicles when the db is empty |
| `Simulator:Enabled` | `false` | fake GPS traffic |
| `EnableSwagger` | Development only | |

Environment variables use `__` as separator, e.g. `ConnectionStrings__Fleet`.
