# Setup Guide

This guide covers how to set up the BLA Task Management System locally with or without Docker.

## 📋 Table of Contents

- [Prerequisites](#prerequisites)
- [Docker Setup (Recommended)](#docker-setup-recommended)
- [Local Development Setup](#local-development-setup)
- [Database Setup](#database-setup)
- [Running the Application](#running-the-application)
- [Troubleshooting](#troubleshooting)

## Prerequisites

### For Docker Setup (Recommended)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows, Mac, Linux)
  - Windows: Docker Desktop 4.0+ with WSL2
  - Mac: Docker Desktop 4.0+
  - Linux: Docker Engine 20.10+ and Docker Compose 2.0+
- [Git](https://git-scm.com/downloads)
- 8GB RAM minimum, 16GB recommended
- 10GB free disk space

### For Local Development
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+ with npm](https://nodejs.org/)
- [MongoDB 7+](https://www.mongodb.com/try/download/community)
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/downloads)

## Docker Setup (Recommended)

### 1. Install Docker Desktop

**Windows:**
```powershell
# Download and install Docker Desktop from:
# https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe

# After installation, verify:
docker --version
docker-compose --version
```

**Mac:**
```bash
# Download and install Docker Desktop from:
# https://desktop.docker.com/mac/main/amd64/Docker.dmg

# Verify installation:
docker --version
docker-compose --version
```

**Linux:**
```bash
# Install Docker Engine
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Verify
docker --version
docker-compose --version
```

### 2. Clone and Start

```bash
# Clone the repository
git clone https://github.com/YOUR_USERNAME/bla-task-management-system.git
cd bla-task-management-system

# Start all services
docker-compose up -d

# Check status
docker-compose ps

# View logs
docker-compose logs -f
```

### 3. Verify Services

Wait about 30 seconds for all services to start, then check:

```bash
# Check if all services are healthy
docker-compose ps

# You should see:
# - mongodb (healthy)
# - postgres (healthy)
# - tasks-api (running)
# - users-api (running)
# - web (running)
```

### 4. Access the Application

- 🌐 **Web UI**: http://localhost:3000
- 📝 **Tasks API Swagger**: http://localhost:5001/swagger
- 👤 **Users API Swagger**: http://localhost:5002/swagger

**Demo Credentials:**
- Email: `admin@taskmanagement.com`
- Password: `Password123!`

### 5. Stopping Services

```bash
# Stop all services
docker-compose down

# Stop and remove volumes (WARNING: deletes all data)
docker-compose down -v
```

## Local Development Setup

### 1. Install Prerequisites

#### Install .NET 8 SDK

**Windows:**
```powershell
# Download from: https://dotnet.microsoft.com/download/dotnet/8.0
# Or use winget:
winget install Microsoft.DotNet.SDK.8

# Verify:
dotnet --version  # Should show 8.0.x
```

**Mac:**
```bash
# Using Homebrew:
brew install --cask dotnet-sdk

# Verify:
dotnet --version
```

**Linux:**
```bash
# Ubuntu/Debian:
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0

# Verify:
dotnet --version
```

#### Install Node.js 20+

**Windows:**
```powershell
# Download from: https://nodejs.org/
# Or use winget:
winget install OpenJS.NodeJS.LTS

# Verify:
node --version  # Should show v20.x or higher
npm --version
```

**Mac:**
```bash
# Using Homebrew:
brew install node@20

# Verify:
node --version
npm --version
```

**Linux:**
```bash
# Using NodeSource:
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt-get install -y nodejs

# Verify:
node --version
npm --version
```

#### Install MongoDB 7+

**Windows:**
```powershell
# Download MongoDB Community Server from:
# https://www.mongodb.com/try/download/community

# Or use Chocolatey:
choco install mongodb

# Start MongoDB:
net start MongoDB

# Verify:
mongosh --version
```

**Mac:**
```bash
# Using Homebrew:
brew tap mongodb/brew
brew install mongodb-community@7.0

# Start MongoDB:
brew services start mongodb-community@7.0

# Verify:
mongosh --version
```

**Linux:**
```bash
# Ubuntu/Debian:
wget -qO - https://www.mongodb.org/static/pgp/server-7.0.asc | sudo apt-key add -
echo "deb [ arch=amd64,arm64 ] https://repo.mongodb.org/apt/ubuntu jammy/mongodb-org/7.0 multiverse" | sudo tee /etc/apt/sources.list.d/mongodb-org-7.0.list
sudo apt-get update
sudo apt-get install -y mongodb-org

# Start MongoDB:
sudo systemctl start mongod

# Verify:
mongosh --version
```

#### Install PostgreSQL 16+

**Windows:**
```powershell
# Download from: https://www.postgresql.org/download/windows/
# Or use Chocolatey:
choco install postgresql16

# The installer will ask for a password - remember it!
# Default port: 5432
```

**Mac:**
```bash
# Using Homebrew:
brew install postgresql@16

# Start PostgreSQL:
brew services start postgresql@16

# Create default user:
createuser -s postgres
```

**Linux:**
```bash
# Ubuntu/Debian:
sudo sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list'
wget -qO- https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo tee /etc/apt/trusted.gpg.d/pgdg.asc &>/dev/null
sudo apt-get update
sudo apt-get install -y postgresql-16

# Start PostgreSQL:
sudo systemctl start postgresql

# Create user:
sudo -u postgres createuser -s $USER
```

### 2. Setup Databases

#### MongoDB Setup

```bash
# Connect to MongoDB
mongosh

# Create database and collection
use tasksdb

# Create collection
db.createCollection('tasks')

# Create indexes
db.tasks.createIndex({ "userId": 1 })
db.tasks.createIndex({ "status": 1 })

# Exit
exit
```

#### PostgreSQL Setup

```bash
# Create database
createdb usersdb

# Run initialization scripts
psql -d usersdb -f infrastructure/database/postgres/01-init.sql
psql -d usersdb -f infrastructure/database/postgres/02-seed.sql
```

### 3. Clone and Install Dependencies

```bash
# Clone repository
git clone https://github.com/YOUR_USERNAME/task-management-system.git
cd task-management-system

# Restore .NET dependencies
dotnet restore

# Install Node.js dependencies
cd apps/web
npm install
cd ../..
```

### 4. Configure Connection Strings

Create `appsettings.Development.json` files:

**tasks-api:**
```bash
# apps/tasks-api/src/TasksApi.WebApi/appsettings.Development.json
```
```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "tasksdb"
  },
  "Jwt": {
    "Secret": "your-super-secret-jwt-key-change-this-in-production-min-32-chars",
    "Issuer": "TaskManagementSystem",
    "Audience": "TaskManagementApp",
    "ExpirationMinutes": 60
  }
}
```

**users-api:**
```bash
# apps/users-api/src/UsersApi.WebApi/appsettings.Development.json
```
```json
{
  "Postgres": {
    "ConnectionString": "Host=localhost;Port=5432;Database=usersdb;Username=YOUR_USERNAME;Password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Secret": "your-super-secret-jwt-key-change-this-in-production-min-32-chars",
    "Issuer": "TaskManagementSystem",
    "Audience": "TaskManagementApp",
    "ExpirationMinutes": 60
  }
}
```

### 5. Run Services Locally

Open 3 terminal windows:

**Terminal 1 - Users API:**
```bash
cd apps/users-api/src/UsersApi.WebApi
dotnet run
# Should start on http://localhost:5002
```

**Terminal 2 - Tasks API:**
```bash
cd apps/tasks-api/src/TasksApi.WebApi
dotnet run
# Should start on http://localhost:5001
```

**Terminal 3 - Web UI:**
```bash
cd apps/web
npm run dev
# Should start on http://localhost:3000
```

## Running Tests

```bash
# Run all .NET tests
dotnet test

# Run specific project tests
dotnet test apps/tasks-api/tests/TasksApi.Application.Tests/
dotnet test apps/users-api/tests/UsersApi.Application.Tests/

# Run web tests
cd apps/web
npm run test
```

## Troubleshooting

### Docker Issues

**Port Already in Use:**
```bash
# Check what's using the ports
netstat -ano | findstr :5001  # Windows
lsof -i :5001                 # Mac/Linux

# Stop the process or change ports in docker-compose.yml
```

**Container Won't Start:**
```bash
# Check logs
docker-compose logs [service-name]

# Rebuild containers
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
```

**Database Connection Failed:**
```bash
# Restart database containers
docker-compose restart mongodb postgres

# Check if databases are healthy
docker-compose ps
```

### Local Development Issues

**MongoDB Connection Failed:**
```bash
# Check if MongoDB is running
mongosh --eval "db.adminCommand('ping')"

# Start MongoDB
# Windows: net start MongoDB
# Mac: brew services start mongodb-community@7.0
# Linux: sudo systemctl start mongod
```

**PostgreSQL Connection Failed:**
```bash
# Check if PostgreSQL is running
psql -U postgres -c "SELECT version();"

# Start PostgreSQL
# Windows: (starts automatically as service)
# Mac: brew services start postgresql@16
# Linux: sudo systemctl start postgresql
```

**.NET Build Errors:**
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

**Node.js Errors:**
```bash
# Clear cache and reinstall
cd apps/web
rm -rf node_modules package-lock.json
npm install
```

## Next Steps

- Read [USER_STORY.md](USER_STORY.md) to understand the project requirements
- Check [ARCHITECTURE.md](ARCHITECTURE.md) for architectural decisions
- See [API_DOCUMENTATION.md](API_DOCUMENTATION.md) for API usage examples

## Need Help?

- Check Docker logs: `docker-compose logs -f`
- Verify all services are running: `docker-compose ps`
- Reset everything: `docker-compose down -v && docker-compose up -d`

---

[← Back to README](../README.md)
