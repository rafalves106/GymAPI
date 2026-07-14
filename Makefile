.PHONY: help build up down restart logs test clean

help: ## Show this help
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'

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

dev: ## Run API locally (requires PostgreSQL running)
	cd src/Api && dotnet run

test: ## Run all tests
	dotnet test --verbosity normal

db-migrate: ## Run EF Core migrations
	dotnet ef migrations add $(name) --project src/Infrastructure --startup-project src/Api

db-shell: ## Open psql shell
	docker compose exec postgres psql -U postgres -d gymtracker_dev

clean: ## Clean build artifacts
	dotnet clean
	rm -rf coverage/ app/ **/bin/ **/obj/

prune: ## Remove unused Docker images
	docker system prune -f

status: ## Show container status
	docker compose ps

health: ## Check API health
	curl -s http://localhost:5200/health | python3 -m json.tool
