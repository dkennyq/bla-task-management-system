# BLA Task Management - Web Frontend

Vue 3 + TypeScript + Vite + TailwindCSS

## 🚀 Quick Start

### 1. Install Dependencies

```bash
npm install
```

### 2. Setup Environment Variables

**⚠️ IMPORTANT:** You must create `.env` files before running the app. These files are not in Git for security reasons.

#### Option A: Use the Setup Script (Recommended)

```bash
# From repository root
./scripts/setup-env.ps1    # Windows
./scripts/setup-env.sh     # Linux/Mac
```

The script will guide you through creating the necessary files.

#### Option B: Manual Setup

**For Docker (Recommended):**
```bash
cp .env.example .env
# Edit .env - ports are already set to 5001, 5002
```

**For Local Development (APIs in Visual Studio/VS Code):**
```bash
cp .env.example .env.development
# Edit .env.development and change ports to:
# VITE_TASKS_API_URL=http://localhost:5077
# VITE_USERS_API_URL=http://localhost:5034
```

### 3. Run Development Server

```bash
npm run dev
```

The app will be available at `http://localhost:3000`

## 📋 Available Scripts

```bash
npm run dev          # Start development server
npm run build        # Build for production
npm run preview      # Preview production build
npm run test         # Run tests
npm run test:ui      # Run tests with UI
npm run test:coverage # Run tests with coverage
npm run lint         # Lint code
npm run lint:fix     # Lint and fix code
npm run type-check   # TypeScript type checking
```

## 🔧 Environment Configuration

The app uses different API ports depending on the environment:

| Environment | Tasks API | Users API | File |
|-------------|-----------|-----------|------|
| **Docker** | `localhost:5001` | `localhost:5002` | `.env` or `.env.test` |
| **Local Dev** | `localhost:5077` | `localhost:5034` | `.env.development` |

### How Vite Loads Environment Files

```bash
npm run dev              # Loads .env.development (then .env as fallback)
npm run dev -- --mode test  # Loads .env.test (then .env as fallback)
npm run build            # Loads .env.production (then .env as fallback)
```

**Priority order:** `.env.[mode].local` → `.env.[mode]` → `.env.local` → `.env`

📖 For more details, see [ENV_SETUP.md](./ENV_SETUP.md)

## 🐳 Docker Usage

The frontend can run in Docker as part of the full stack:

```bash
# From repository root
docker-compose up -d web
```

Or build the frontend image separately:

```bash
docker build -f infrastructure/docker/web.Dockerfile -t bla-web .
```

## 🧪 Testing

```bash
npm run test              # Run tests in watch mode
npm run test:ui           # Run tests with Vitest UI
npm run test:coverage     # Generate coverage report
```

## 🏗️ Project Structure

```
src/
├── components/       # Reusable Vue components
├── views/           # Page components
├── router/          # Vue Router configuration
├── stores/          # Pinia stores (state management)
├── services/        # API clients and services
├── types/           # TypeScript type definitions
├── utils/           # Utility functions
└── assets/          # Static assets (images, styles)
```

## 🔑 Default Test Credentials

```
Email: admin@taskmanagement.com
Password: Password123!
```

## 🛠️ Tech Stack

- **Framework:** Vue 3 (Composition API with `<script setup>`)
- **Language:** TypeScript
- **Build Tool:** Vite
- **Styling:** TailwindCSS
- **State Management:** Pinia
- **Routing:** Vue Router
- **HTTP Client:** Axios
- **Testing:** Vitest + Happy-DOM
- **Linting:** ESLint + TypeScript ESLint

## 📚 Learn More

- [Vue 3 Documentation](https://vuejs.org/)
- [Vite Documentation](https://vitejs.dev/)
- [TypeScript Documentation](https://www.typescriptlang.org/)
- [TailwindCSS Documentation](https://tailwindcss.com/)
- [Pinia Documentation](https://pinia.vuejs.org/)

## ❓ Troubleshooting

### Problem: "Cannot connect to API"

**Solution:** Verify that:
1. Your `.env` file exists and has the correct API URLs
2. The APIs are running on the expected ports
3. Check browser console for CORS errors

### Problem: "VITE_* variables are undefined"

**Solution:** 
1. Ensure `.env` file exists
2. Restart the dev server (`Ctrl+C` then `npm run dev`)
3. Verify variable names start with `VITE_`

### Problem: "Wrong API ports"

**Solution:** 
- For Docker: Use ports 5001, 5002 in `.env`
- For Local: Use ports 5077, 5034 in `.env.development`

Run the setup script to reconfigure: `./scripts/setup-env.ps1`
