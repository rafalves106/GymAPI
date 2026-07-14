.PHONY: help build up down restart logs test clean

# Default target
help: ## Show this help
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

# ============================================
# Docker Commands
# ============================================

build: ## Build Docker images
	docker compose build

up: ## Start all services (API + PostgreSQL)
	docker compose up -d

down: ## Stop all services
	docker compose down

down-volumes: ## Stop all services and remove volumes
	docker compose down -v

restart: ## Restart all services
	docker compose restart

logs: ## View logs
	docker compose logs -f

logs-api: ## View API logs only
	docker compose logs -f api

logs-db: ## View PostgreSQL logs only
	docker compose logs -f postgres

# ============================================
# Development Commands
# ============================================

dev: ## Run API locally (requires PostgreSQL running)
	cd src/Api && dotnet run

test: ## Run all tests
	dotnet test tests/UnitTests --verbosity quiet
	dotnet test tests/IntegrationTests --verbosity quiet

test-unit: ## Run unit tests only
	dotnet test tests/UnitTests --verbosity normal

test-integration: ## Run integration tests only
	dotnet test tests/IntegrationTests --verbosity normal

test-coverage: ## Run tests with coverage
	dotnet test tests/UnitTests --collect:"XPlat Code Coverage" --results-directory ./coverage

# ============================================
# Database Commands
# ============================================

db-migrate: ## Run EF Core migrations
	dotnet ef migrations add $(name) --project src/Infrastructure --startup-project src/Api

db-update: ## Apply migrations to database
	dotnet ef database update --project src/Infrastructure --startup-project src/Api

db-shell: ## Open psql shell
	docker compose exec postgres psql -U postgres -d gym_exercises_dev

# ============================================
# Cleanup Commands
# ============================================

clean: ## Clean build artifacts
	dotnet clean
	rm -rf coverage/
	rm -rf app/
	rm -rf **/bin/ **/obj/

prune: ## Remove unused Docker images
	docker system prune -f

# ============================================
# Production Commands
# ============================================

prod-build: ## Build production image
	docker compose -f docker-compose.yml build

prod-up: ## Start in production mode
	ASPNETCORE_ENVIRONMENT=Production docker compose up -d

status: ## Show container status
	docker compose ps

health: ## Check API health
	curl -s http://localhost:5200/health | python3 -m json.tool
