# FleetPing web

Angular 9 dashboard for fleet-ping. Generated with Angular CLI 9.0.5.

## Pages

- **Overview** - fleet counts, how many trackers reported recently, 30 day maintenance spend, vehicles due for service
- **Map** - Leaflet map of last known positions, refreshed every 10s. Green = moving, yellow = stopped, grey = no GPS in the last 15 min (or mock position)
- **Vehicles** - list, create/edit, detail with GPS trail (1h / 6h / 24h) and maintenance log (add, edit, delete, filter)

All pages except login sit behind `AuthGuard`. Credentials are checked against `/api/auth/me` and kept in
`sessionStorage`; `AuthInterceptor` adds the Basic header to API calls and sends the user back to
login on a 401.

## Layout

```
src/app/
├── core/           models, api services, auth interceptor/guard, CoreModule
├── auth/login/
├── dashboard/
├── map/            fleet-map, vehicle-trail
├── maintenance/    maintenance-list, maintenance-form
└── vehicles/       vehicle-list, vehicle-detail, vehicle-form
```

## Development

```bash
npm ci
npm start        # http://localhost:4200, expects the API on http://localhost:5000
npm test
npm run lint
```

The API url comes from `src/environments/environment.ts`. Production builds use `/api`, which the nginx
config in this folder proxies to the `api` container.

Leaflet marker images are copied to `assets/leaflet/` by `angular.json`; the map component points
`L.Icon.Default` at them since the default url detection breaks once the css is bundled.
