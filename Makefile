.PHONY: help build up down logs test clean reset-db install dev-tasks dev-users dev-web

help: ## Show this help message
	@echo "Available commands:"
	@echo ""
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  \033[36m%-20s\033[0m %s\n", $$1, $$2}'

build: ## Build all Docker images
	docker-compose build

up: ## Start all services
	docker-compose up -d
	@echo "✅ All services started!"
	@echo "📝 Tasks API: http://localhost:5001/swagger"
	@echo "👤 Users API: http://localhost:5002/swagger"
	@echo "🌐 Web UI: http://localhost:3000"

down: ## Stop all services
	docker-compose down

logs: ## Show logs from all services
	docker-compose logs -f

logs-tasks: ## Show logs from tasks-api
	docker-compose logs -f tasks-api

logs-users: ## Show logs from users-api
	docker-compose logs -f users-api

logs-web: ## Show logs from web
	docker-compose logs -f web

logs-db: ## Show database logs
	docker-compose logs -f mongodb postgres

test: ## Run all tests
	@echo "Running tasks-api tests..."
	dotnet test apps/tasks-api/tests/TasksApi.Domain.Tests/
	dotnet test apps/tasks-api/tests/TasksApi.Application.Tests/
	@echo "Running users-api tests..."
	dotnet test apps/users-api/tests/UsersApi.Domain.Tests/
	dotnet test apps/users-api/tests/UsersApi.Application.Tests/
	@echo "Running web tests..."
	cd apps/web && npm run test

test-tasks: ## Run tasks-api tests only
	dotnet test apps/tasks-api/tests/

test-users: ## Run users-api tests only
	dotnet test apps/users-api/tests/

test-web: ## Run web tests only
	cd apps/web && npm run test

clean: ## Clean all containers, volumes, and images
	docker-compose down -v
	docker system prune -f

reset-db: ## Reset databases (WARNING: deletes all data)
	docker-compose down -v
	docker-compose up -d mongodb postgres
	@echo "⏳ Waiting for databases to initialize..."
	@sleep 10
	@echo "✅ Databases reset!"

install: ## Install all dependencies
	@echo "Installing .NET dependencies..."
	dotnet restore BlaTaskManagement.sln
	@echo "Installing Node.js dependencies..."
	cd apps/web && npm install
	@echo "✅ All dependencies installed!"

dev-tasks: ## Run tasks-api locally (without Docker)
	cd apps/tasks-api/src/TasksApi.WebApi && dotnet run

dev-users: ## Run users-api locally (without Docker)
	cd apps/users-api/src/UsersApi.WebApi && dotnet run

dev-web: ## Run web frontend locally (without Docker)
	cd apps/web && npm run dev

status: ## Show status of all services
	docker-compose ps
