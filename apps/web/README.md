# BLA Task Management - Web Frontend

The frontend is designed to run as part of the Docker Compose stack. In the recommended setup, the Vue app runs in the `web` container and talks to the containerized APIs through host URLs exposed on ports `5001` and `5002`.

## Recommended workflow: run with Docker
From the repository root:

```bash
docker compose up -d web tasks-api users-api mongodb postgres seq
```

Or start the full stack:

```bash
docker compose up -d
```

Frontend URL:
- Web UI: http://localhost:3000

Backend URLs used by the frontend container:
- `VITE_TASKS_API_URL=http://localhost:5001/api`
- `VITE_USERS_API_URL=http://localhost:5002/api`

These variables are already defined in `docker-compose.yml`, so you usually do **not** need a local `.env` file for normal Docker-based development.

## Frontend-specific Docker details
The `web` service uses:
- Port mapping: `3000:3000`
- Bind mount: `./apps/web:/app`
- Anonymous volume for dependencies: `/app/node_modules`

That means frontend source edits on your machine are reflected inside the container, while container-managed `node_modules` stays isolated from the host.

## Login for UI testing
- Email: `admin@taskmanagement.com`
- Password: `Password123!`

Use this account to validate:
- login flow
- task list loading
- authenticated calls to `/api/tasks`
- authenticated calls to `/api/users/me`

## Common frontend commands
Run from the repository root.

```bash
# Rebuild after dependency or Dockerfile changes
docker compose up -d --build web

# Watch frontend logs
docker compose logs -f web

# Run frontend tests inside the container
docker compose exec web npm run test

# Run lint inside the container
docker compose exec web npm run lint

# Run type-check inside the container
docker compose exec web npm run type-check
```

## API paths the frontend uses
The frontend Axios clients point to the `/api` base path and then call endpoints such as:
- Users: `/api/users/login`, `/api/users/register`, `/api/users/me`
- Tasks: `/api/tasks`, `/api/tasks/{id}`

If the web UI loads but API calls fail, verify that `tasks-api` and `users-api` are running and that the frontend container still has the expected `VITE_*` values from `docker-compose.yml`.

## When to use local frontend-only development
Use local Node/Vite only if you specifically want to debug the frontend outside Docker.

```bash
npm install
npm run dev
```

If you do this, set your own `VITE_TASKS_API_URL` and `VITE_USERS_API_URL` to match whichever backend you are running. The Docker setup remains the default and recommended path.

## Troubleshooting
### Frontend cannot reach the APIs
```bash
docker compose ps
docker compose logs -f web tasks-api users-api
```
Confirm these host URLs work:
- http://localhost:5001/swagger
- http://localhost:5002/swagger

### Need a clean restart
```bash
docker compose down
docker compose up -d
```

### Need a full data reset
```bash
docker compose down -v
docker compose up -d
```

`docker compose down` keeps persisted MongoDB, PostgreSQL, and Seq data. `docker compose down -v` removes those volumes.
