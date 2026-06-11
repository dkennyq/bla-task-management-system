# Testing APIs

Use the Docker stack when testing APIs. The recommended workflow is to run **all services in containers**, then exercise the APIs through Swagger, Postman, or `curl` against the published host ports.

## Start the stack
From the repository root:

```bash
docker compose up -d
```

Verify the containers:

```bash
docker compose ps
```

Expected ports:
- Web: `http://localhost:3000`
- Tasks API: `http://localhost:5001`
- Users API: `http://localhost:5002`
- MongoDB: `localhost:27017`
- PostgreSQL: `localhost:5432`
- Seq: `http://localhost:8081`

If something fails to start:
```bash
docker compose logs -f tasks-api users-api mongodb postgres
```

## Authentication
Most API endpoints require a JWT.

### Demo login
- Email: `admin@taskmanagement.com`
- Password: `Password123!`

### Get a token
```powershell
$response = Invoke-RestMethod -Method Post -Uri 'http://localhost:5002/api/users/login' `
  -ContentType 'application/json' `
  -Body '{"email":"admin@taskmanagement.com","password":"Password123!"}'

$token = $response.token
$refreshToken = $response.refreshToken
```

## Core endpoints
### Users API
- `POST /api/users/login`
- `POST /api/users/register`
- `POST /api/users/refresh`
- `GET /api/users/me`
- `PUT /api/users/me`
- `POST /api/users/me/reset-password`
- `GET /api/users`
- `POST /api/users/admin/create`
- `PUT /api/users/admin/{id}/role`

### Tasks API
- `GET /api/tasks`
- `GET /api/tasks/{id}`
- `POST /api/tasks`
- `PUT /api/tasks/{id}`
- `DELETE /api/tasks/{id}`

## Test with Swagger
Swagger is available from the running API containers:
- Tasks API: http://localhost:5001/swagger
- Users API: http://localhost:5002/swagger

Recommended flow:
1. Open Users API Swagger.
2. Run `POST /api/users/login` with the demo credentials.
3. Copy the JWT token.
4. Click **Authorize** and paste `Bearer <token>`.
5. Test protected endpoints such as `GET /api/users/me` or `GET /api/tasks`.

## Test with curl
### Login
```bash
curl -X POST http://localhost:5002/api/users/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@taskmanagement.com","password":"Password123!"}'
```

### Get current user
```bash
curl http://localhost:5002/api/users/me \
  -H "Authorization: Bearer <token>"
```

### Get tasks
```bash
curl http://localhost:5001/api/tasks \
  -H "Authorization: Bearer <token>"
```

### Create a task
```bash
curl -X POST http://localhost:5001/api/tasks \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{"title":"Container test task","description":"Created against Docker stack","priority":"High","dueDate":"2026-06-15T23:59:59Z"}'
```

## Test with Postman
1. Import `docs/POSTMAN_COLLECTION.json`.
2. Confirm the collection targets `localhost:5001` and `localhost:5002`.
3. Run **Login (Manager)** first to populate `jwt_token` and `refresh_token`.
4. Run the protected Tasks API and Users API requests.

## Helpful Docker commands while testing
```bash
# Tail API logs while calling endpoints
docker compose logs -f tasks-api users-api

# Open the web app against the containerized APIs
# http://localhost:3000

# Stop containers but keep data
docker compose down

# Stop containers and delete persisted data
docker compose down -v
```

`docker compose down` keeps MongoDB, PostgreSQL, and Seq data. `docker compose down -v` removes those volumes and gives you a clean reset for repeatable testing.

## Related docs
- [Setup Guide](./SETUP.md)
- [API Security Guide](./API_SECURITY_GUIDE.md)
- [Postman Collection](./POSTMAN_COLLECTION.json)
