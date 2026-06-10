# GitHub Repository Setup Instructions

## ✅ Current Status

Your project has been successfully initialized locally at:
```
C:\Users\devke\source\task-management-system
```

Git repository is initialized with an initial commit.

## 🚀 Next Steps: Create GitHub Repository

You have two options:

### Option 1: Create Repository via GitHub CLI (Fastest)

If you have GitHub CLI installed:

```powershell
# Login to GitHub (if not already logged in)
gh auth login

# Create repository
cd C:\Users\devke\source\task-management-system
gh repo create task-management-system --public --source=. --remote=origin --push

# Your repository will be created and pushed automatically!
```

### Option 2: Create Repository via GitHub Web (Recommended if no CLI)

#### Step 1: Create Repository on GitHub

1. Go to https://github.com/new
2. Fill in the details:
   - **Repository name**: `task-management-system`
   - **Description**: "Modern task management system with .NET microservices, Vue.js, MongoDB, and PostgreSQL"
   - **Visibility**: Public (or Private if preferred)
   - ⚠️ **DO NOT** initialize with README, .gitignore, or license (we already have these)
3. Click **"Create repository"**

#### Step 2: Push Your Local Repository

After creating the repository, GitHub will show you commands. Use these:

```powershell
# Navigate to your project
cd C:\Users\devke\source\task-management-system

# Add GitHub as remote origin (replace YOUR_USERNAME)
git remote add origin https://github.com/YOUR_USERNAME/task-management-system.git

# Rename branch to main (GitHub's default)
git branch -M main

# Push to GitHub
git push -u origin main
```

**If using SSH instead of HTTPS:**
```powershell
git remote add origin git@github.com:YOUR_USERNAME/task-management-system.git
git branch -M main
git push -u origin main
```

#### Step 3: Verify Upload

1. Go to your GitHub repository: `https://github.com/YOUR_USERNAME/task-management-system`
2. You should see all your files and the README
3. The README will be displayed on the homepage

### Option 3: Use GitHub Desktop (GUI)

1. Download [GitHub Desktop](https://desktop.github.com/)
2. Open GitHub Desktop
3. File → Add Local Repository
4. Select: `C:\Users\devke\source\task-management-system`
5. Click "Publish repository"
6. Choose name, description, and visibility
7. Click "Publish Repository"

## 🔧 Installing Required Tools

### For Docker Development (Recommended)

You only need **Docker Desktop**:

#### Windows:
```powershell
# Download and install Docker Desktop:
# https://desktop.docker.com/win/main/amd64/Docker%20Desktop%20Installer.exe

# After installation:
docker --version
docker-compose --version
```

#### Mac:
```bash
# Download and install Docker Desktop:
# https://desktop.docker.com/mac/main/amd64/Docker.dmg

# Verify:
docker --version
docker-compose --version
```

#### Linux:
```bash
# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Verify
docker --version
docker-compose --version

# Add your user to docker group
sudo usermod -aG docker $USER
# Log out and log back in
```

### For Local Development (Without Docker)

If you want to run services locally without Docker:

#### Install .NET 8 SDK
```powershell
# Windows (using winget):
winget install Microsoft.DotNet.SDK.8

# Or download from:
# https://dotnet.microsoft.com/download/dotnet/8.0

# Verify:
dotnet --version
```

#### Install Node.js 20+
```powershell
# Windows (using winget):
winget install OpenJS.NodeJS.LTS

# Or download from:
# https://nodejs.org/

# Verify:
node --version
npm --version
```

#### Install MongoDB 7+
```powershell
# Windows (using Chocolatey):
choco install mongodb

# Or download from:
# https://www.mongodb.com/try/download/community

# Start MongoDB:
net start MongoDB

# Verify:
mongosh --version
```

#### Install PostgreSQL 16+
```powershell
# Windows (using Chocolatey):
choco install postgresql16

# Or download from:
# https://www.postgresql.org/download/windows/

# Remember the password you set during installation!

# Verify:
psql --version
```

## 🚀 Running the Application

### Using Docker (Easiest)

```powershell
# Navigate to project
cd C:\Users\devke\source\task-management-system

# Start all services (first time will download images)
docker-compose up -d

# Wait about 30 seconds, then check status
docker-compose ps

# View logs
docker-compose logs -f

# Access the application:
# - Web UI: http://localhost:3000
# - Tasks API: http://localhost:5001/swagger
# - Users API: http://localhost:5002/swagger
```

**Demo Login:**
- Email: `admin@taskmanagement.com`
- Password: `Password123!`

### Stopping Services

```powershell
# Stop all services
docker-compose down

# Stop and remove all data (WARNING: deletes databases)
docker-compose down -v
```

### Using Local Development

See the detailed guide in [docs/SETUP.md](docs/SETUP.md)

## 📊 Verify Everything Works

### 1. Check Docker Services
```powershell
docker-compose ps
```
All services should show as "Up" or "healthy"

### 2. Test APIs
```powershell
# Test Users API health
curl http://localhost:5002/health

# Test Tasks API health
curl http://localhost:5001/health
```

### 3. Open Web UI
Open browser: http://localhost:3000

### 4. Login
Use demo credentials:
- Email: `admin@taskmanagement.com`
- Password: `Password123!`

## 🐛 Troubleshooting

### Ports Already in Use
```powershell
# Check what's using ports 3000, 5001, 5002, 27017, 5432
netstat -ano | findstr ":5001"

# Either stop the conflicting service or change ports in docker-compose.yml
```

### Docker Errors
```powershell
# Restart Docker Desktop
# Then clean and restart:
docker-compose down -v
docker-compose up -d --build
```

### Database Connection Issues
```powershell
# Check if databases are running
docker-compose logs mongodb
docker-compose logs postgres

# Restart databases
docker-compose restart mongodb postgres
```

## 📚 Next Steps

1. ✅ **Push to GitHub** (see instructions above)
2. 📖 **Read Documentation**:
   - [USER_STORY.md](docs/USER_STORY.md) - Project requirements
   - [ARCHITECTURE.md](docs/ARCHITECTURE.md) - Architecture decisions
   - [API_DOCUMENTATION.md](docs/API_DOCUMENTATION.md) - API usage
3. 🧪 **Start Development**:
   - Run tests: `docker-compose exec tasks-api dotnet test`
   - Make changes to code
   - See changes reflected in running containers
4. 🎨 **Customize**:
   - Update environment variables in `docker-compose.yml`
   - Modify frontend styling in `apps/web/src/`
   - Add new features following Clean Architecture

## 🎯 Project Structure Quick Reference

```
task-management-system/
├── apps/
│   ├── tasks-api/      ← .NET API for tasks (MongoDB)
│   ├── users-api/      ← .NET API for users (PostgreSQL)
│   └── web/            ← Vue.js frontend
├── infrastructure/
│   ├── docker/         ← Dockerfiles
│   └── database/       ← DB init scripts
├── docs/               ← Documentation
├── docker-compose.yml  ← Start here!
└── README.md           ← Main documentation
```

## 🤝 Getting Help

- Check logs: `docker-compose logs -f [service-name]`
- View service status: `docker-compose ps`
- Reset everything: `docker-compose down -v && docker-compose up -d`
- Read docs: Check the `docs/` folder

## ✅ Success Checklist

- [ ] Docker Desktop installed and running
- [ ] Git repository created locally
- [ ] GitHub repository created online
- [ ] Code pushed to GitHub
- [ ] `docker-compose up -d` runs successfully
- [ ] All 5 services are running (mongodb, postgres, tasks-api, users-api, web)
- [ ] Can access http://localhost:3000
- [ ] Can login with demo credentials
- [ ] Can see Swagger docs at http://localhost:5001/swagger

---

**Questions or issues?** Check [SETUP.md](docs/SETUP.md) for detailed troubleshooting.

🎉 **Congratulations!** Your Task Management System is ready for development!
