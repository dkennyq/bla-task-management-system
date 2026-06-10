# BLA Task Management System

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Vue.js](https://img.shields.io/badge/Vue.js-3.x-4FC08D?logo=vue.js)](https://vuejs.org/)
[![MongoDB](https://img.shields.io/badge/MongoDB-7.0-47A248?logo=mongodb)](https://www.mongodb.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?logo=docker)](https://www.docker.com/)

A modern full-stack task management application built with **Clean Architecture**, **TDD**, and **microservices** principles.

## 🎯 Project Overview

This is a technical interview exercise demonstrating:
- ✅ Clean Architecture principles
- ✅ Test-Driven Development (TDD)
- ✅ Microservices architecture
- ✅ Modern data storage (MongoDB + PostgreSQL with native drivers)
- ✅ RESTful APIs with .NET 8
- ✅ Responsive frontend with Vue.js 3
- ✅ Docker containerization

## 🏗️ Architecture

```
┌──────────────────┐
│   Vue.js App     │  Frontend (Port 3000)
│     :3000        │
└────────┬─────────┘
         │
         ├─────────────────────┬────────────────────┐
         │                     │                    │
         ▼                     ▼                    ▼
  ┌─────────────┐       ┌─────────────┐
  │  Tasks API  │       │  Users API  │
  │   :5001     │       │   :5002     │
  └──────┬──────┘       └──────┬──────┘
         │                     │
         ▼                     ▼
  ┌─────────────┐       ┌─────────────┐
  │   MongoDB   │       │ PostgreSQL  │
  │   tasksdb   │       │   usersdb   │
  └─────────────┘       └─────────────┘
```

### Services

1. **Tasks API** (`apps/tasks-api/`)
   - Manages task CRUD operations
   - MongoDB for flexible document storage
   - Port: 5001
   - Swagger: http://localhost:5001/swagger

2. **Users API** (`apps/users-api/`)
   - Manages user authentication and authorization
   - PostgreSQL with Npgsql driver
   - JWT-based authentication
   - Port: 5002
   - Swagger: http://localhost:5002/swagger

3. **Web UI** (`apps/web/`)
   - Vue.js 3 with Composition API
   - Pinia for state management
   - TailwindCSS for styling
   - Port: 3000

## 🚀 Quick Start

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (includes Docker Compose)
- [Git](https://git-scm.com/downloads)
- **OR** for local development:
  - [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
  - [Node.js 20+](https://nodejs.org/)
  - [MongoDB 7+](https://www.mongodb.com/try/download/community)
  - [PostgreSQL 16+](https://www.postgresql.org/download/)

### Option 1: Docker (Recommended)

```bash
# Clone the repository
git clone https://github.com/YOUR_USERNAME/bla-task-management-system.git
cd bla-task-management-system

# Start all services
docker-compose up -d

# Check service status
docker-compose ps

# View logs
docker-compose logs -f
```

**Access the application:**
- 🌐 Web UI: http://localhost:3000
- 📝 Tasks API: http://localhost:5001/swagger
- 👤 Users API: http://localhost:5002/swagger
- 🍃 MongoDB: mongodb://localhost:27017
- 🐘 PostgreSQL: postgresql://admin:admin123@localhost:5432/usersdb

**Demo Credentials:**
- Email: `admin@taskmanagement.com`
- Password: `Password123!`

### Option 2: Local Development

See [SETUP.md](docs/SETUP.md) for detailed local development setup.

## 📁 Project Structure

```
bla-task-management-system/
├── apps/
│   ├── tasks-api/          # Task Management Service (MongoDB)
│   │   ├── src/
│   │   │   ├── TasksApi.Domain/
│   │   │   ├── TasksApi.Application/
│   │   │   ├── TasksApi.Infrastructure/
│   │   │   └── TasksApi.WebApi/
│   │   └── tests/
│   ├── users-api/          # User Management Service (PostgreSQL)
│   │   ├── src/
│   │   │   ├── UsersApi.Domain/
│   │   │   ├── UsersApi.Application/
│   │   │   ├── UsersApi.Infrastructure/
│   │   │   └── UsersApi.WebApi/
│   │   └── tests/
│   └── web/                # Vue.js Frontend
│       ├── src/
│       └── tests/
├── infrastructure/
│   ├── docker/             # Dockerfiles
│   └── database/           # DB initialization scripts
└── docs/                   # Documentation
```

## 🧪 Running Tests

```bash
# All tests
docker-compose exec tasks-api dotnet test
docker-compose exec users-api dotnet test
docker-compose exec web npm run test

# Or use Makefile
make test
```

## 🛠️ Available Commands (Makefile)

```bash
make help          # Show all available commands
make build         # Build all Docker images
make up            # Start all services
make down          # Stop all services
make logs          # Show logs from all services
make test          # Run all tests
make clean         # Clean all containers and volumes
make reset-db      # Reset databases (WARNING: deletes data)
make install       # Install dependencies
```

## 📚 Documentation

- [Setup Guide](docs/SETUP.md) - Detailed setup instructions
- [Testing APIs](docs/TESTING_APIS.md) - **🆕 How to test APIs with Postman, Swagger, and curl**
- [User Stories](docs/USER_STORIES.md) - Project requirements and user stories (AI-agent ready)
- [GitHub Setup](docs/GITHUB_TASKS_SETUP.md) - Using GitHub Issues/Projects as Jira
- [GitHub Project](docs/GITHUB_PROJECT_SETUP.md) - GitHub Project board configuration
- [Postman Collection](docs/POSTMAN_COLLECTION.json) - Pre-configured API requests
- [Architecture](docs/ARCHITECTURE.md) - Architectural decisions and patterns
- [API Documentation](docs/API_DOCUMENTATION.md) - API endpoints and usage
- [GenAI Process](docs/GENAI_PROCESS.md) - AI tool usage documentation

## 🔑 Key Features

- ✅ **CRUD Operations** for tasks
- ✅ **User Authentication** with JWT
- ✅ **Clean Architecture** with clear separation of concerns
- ✅ **TDD** with comprehensive test coverage
- ✅ **Modern Data Storage**:
  - MongoDB with native driver (no EF/Dapper)
  - PostgreSQL with Npgsql (no EF/Dapper)
- ✅ **Microservices** architecture
- ✅ **Responsive UI** with Vue.js 3
- ✅ **Docker** ready with docker-compose
- ✅ **API Documentation** with Swagger/OpenAPI

## 🎨 Tech Stack

### Backend
- .NET 8
- ASP.NET Core Web API
- MongoDB.Driver 2.25.0
- Npgsql 8.0.3
- BCrypt.Net-Next
- System.IdentityModel.Tokens.Jwt
- xUnit, Moq, FluentAssertions

### Frontend
- Vue.js 3 (Composition API)
- Vite
- Pinia (State Management)
- Vue Router
- Axios
- TailwindCSS
- Vitest

### Infrastructure
- Docker & Docker Compose
- MongoDB 7
- PostgreSQL 16
- Nginx

## 🤝 Contributing

This is a technical interview project, but suggestions are welcome!

## 📝 License

This project is for educational purposes.

## 👨‍💻 Author

Created as a technical interview exercise for Ballast Lane Applications.

---

**Need help?** Check the [Setup Guide](docs/SETUP.md) or open an issue.
