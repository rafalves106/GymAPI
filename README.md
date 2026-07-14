# GymAPI

REST API for gym exercise management built with C#, .NET 10, PostgreSQL and Docker.

## Architecture

Hexagonal Architecture (Ports & Adapters) + Domain-Driven Design.

```
src/
├── Domain/           # Entities, Value Objects, Repository interfaces, Exceptions
│   └── Entities/     # Exercise (aggregate), Training (aggregate), TrainingExercise
├── Application/      # DTOs, Use Cases interfaces, Services
│   └── DTOs/         # Request/Response contracts
├── Infrastructure/   # Database implementations, External services
│   └── Persistence/  # EF Core, Repositories, Unit of Work
│       ├── Context/  # GymDbContext (PostgreSQL)
│       ├── Entities/ # EF Core entity models
│       └── Repositories/ # Repository implementations
└── Api/              # Controllers, Middleware, JWT config
    └── Controllers/  # Exercises, Trainings, Auth
```

**Flow**: Request → Controller → Use Case (Service) → Domain → Repository → PostgreSQL

**Design principles**:
- Domain has zero external dependencies
- Application layer orchestrates workflows
- Infrastructure implements repository interfaces
- API layer handles HTTP concerns only

## Entities

- **Exercise**: Muscle group, name, description, difficulty, instructions, tips, ExternalApiId (nullable for frontend integration)
- **Training**: Owner (UserId), name (1-100 chars), up to 10 exercises (via TrainingExercise)
- **TrainingExercise**: Join entity with rest (0-120s), reps (1-30), series (1-15), supports duplicate exercises via OrderIndex

## Endpoints

| Method | Route                        | Auth      | Description                          |
|--------|------------------------------|-----------|--------------------------------------|
| POST   | `/api/auth/register`         | Public    | Register new user                    |
| POST   | `/api/auth/login`            | Public    | Login and receive JWT token          |
| GET    | `/api/exercises`             | Optional  | List exercises (filter: MuscleGroup, Difficulty, Search) |
| GET    | `/api/exercises/{id}`        | Optional  | Get exercise by ID                   |
| POST   | `/api/exercises`             | Required  | Create exercise                      |
| PUT    | `/api/exercises/{id}`        | Required  | Update exercise                      |
| DELETE | `/api/exercises/{id}`        | Required  | Delete exercise                      |
| GET    | `/api/trainings`             | Required  | List trainings (filter: Name)        |
| GET    | `/api/trainings/{id}`        | Required  | Get training by ID                   |
| POST   | `/api/trainings`             | Required  | Create training with exercises       |
| PUT    | `/api/trainings/{id}`        | Required  | Update training                      |
| DELETE | `/api/trainings/{id}`        | Required  | Delete training                      |
| GET    | `/health`                    | Public    | Health check                         |

## Infrastructure

- **Runtime**: .NET 10
- **Database**: PostgreSQL 16 (port 5433 external, 5432 internal)
- **Authentication**: JWT Bearer tokens
- **Container**: Debian slim (multi-stage build), non-root user
- **Tests**: xUnit + Moq + FluentAssertions (96.3% line coverage, 119 tests)

## Running

### Docker Compose (recommended)

```bash
cp .env.example .env
make up
# or: docker compose up -d
```

API available at `http://localhost:5200`

### Local development

```bash
# Run PostgreSQL
docker run -d --name gym-postgres \
  -e POSTGRES_DB=gym_exercises_dev \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 postgres:16-alpine

# Update connection string in src/Api/appsettings.json if needed

dotnet run --project src/Api
```

### Useful commands

```bash
make up        # Start services
make down      # Stop services
make logs      # View logs
make test      # Run all tests
make test-coverage  # Run tests with coverage
make build     # Build solution
make clean     # Clean build artifacts
make restore   # Restore dependencies
make publish   # Publish for production
```

## Configuration

Environment variables (override `appsettings.json`):

| Variable | Default | Description |
|----------|---------|-------------|
| `POSTGRES_DB` | `gym_exercises_dev` | Database name |
| `POSTGRES_USER` | `postgres` | Database user |
| `POSTGRES_PASSWORD` | `postgres` | Database password |
| `POSTGRES_PORT` | `5433` | External database port |
| `API_PORT` | `5200` | API port |
| `JWT_SECRET_KEY` | *(must be set)* | JWT signing key (min 32 chars) |
| `JWT_ISSUER` | `GymAPI` | JWT issuer |
| `JWT_AUDIENCE` | `GymAPI` | JWT audience |
| `JWT_EXPIRATION` | `60` | Token expiration in minutes |

## Development

```bash
dotnet test                              # Run unit tests
dotnet test --filter "Category=Integration"  # Run integration tests
dotnet test --collect:"XPlat Code Coverage"   # Coverage report
```
