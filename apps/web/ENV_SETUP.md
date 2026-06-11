# 🎯 Environment Configuration - Quick Start

## Available Files

### ✅ `.env` - Default Configuration (Docker)
```env
VITE_TASKS_API_URL=http://localhost:5001
VITE_USERS_API_URL=http://localhost:5002
```
**Use when:** Running everything in Docker

### ✅ `.env.development` - Local Development
```env
VITE_TASKS_API_URL=http://localhost:5077
VITE_USERS_API_URL=http://localhost:5034
```
**Use when:** Running APIs from Visual Studio/VS Code

### ✅ `.env.test` - Docker Testing
```env
VITE_TASKS_API_URL=http://localhost:5001
VITE_USERS_API_URL=http://localhost:5002
```
**Use when:** Running tests with Docker

## 🚀 Quick Usage

### Option 1: Everything in Docker (Recommended)
```bash
# The default .env file is already configured
npm run dev
```

### Option 2: Local Development (APIs in Visual Studio)
```bash
# Uses .env.development automatically
npm run dev
```

### Option 3: Manually change environment
```bash
# Force test mode
npm run dev -- --mode test

# Force development mode
npm run dev -- --mode development
```

## 📊 Ports Table

| Environment | Tasks API | Users API |
|-------------|-----------|-----------|
| **Docker** | 5001 | 5002 |
| **Local (VS)** | 5077 | 5034 |

## ✅ Verification

```bash
# See which .env file is being used
cat .env

# Test the app
npm run dev
```

Complete documentation: See `env-configuration-guide.md` in the session folder.
