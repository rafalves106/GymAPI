# GymAPI — Current State

## What It Is
REST API for gym workout management. .NET 10, PostgreSQL, JWT auth, Docker ready.

## Architecture
Clean Architecture: Domain → Application → Infrastructure → Api

## Entities
- **User**: username (unique), email (unique), passwordHash, isMaster, createdAt, lastLoginAt
- **Workout**: name, userId, scheduledDay (DayOfWeek?), exercises (collection)
- **Exercise**: workoutId, name, targetSets, targetReps, restSeconds, order
- **WorkoutSession**: userId, workoutId, status (Running/Paused/Completed/Cancelled), startedAt, finishedAt, progress
- **ExerciseProgress**: sessionId, exerciseId, completedSets

## Endpoints
| Method | Route | Auth |
|--------|-------|------|
| POST | /api/auth/register | No |
| POST | /api/auth/login | No |
| GET | /api/workouts | Yes |
| GET | /api/workouts/today?day= | Yes |
| GET | /api/workouts/{id} | Yes |
| POST | /api/workouts | Yes |
| PUT | /api/workouts/{id} | Yes |
| DELETE | /api/workouts/{id} | Yes |
| PUT | /api/workouts/{id}/day | Yes |
| POST | /api/sessions/start/{workoutId} | Yes |
| POST | /api/sessions/{id}/pause | Yes |
| POST | /api/sessions/{id}/resume | Yes |
| POST | /api/sessions/{id}/stop | Yes |
| POST | /api/sessions/{id}/cancel | Yes |
| POST | /api/sessions/{id}/exercise/{exId}/increment | Yes |
| POST | /api/sessions/{id}/exercise/{exId}/decrement | Yes |
| GET | /health | No |

## Config
- Connection string key: `ConnectionStrings:Default`
- JWT config: `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` (7-day expiry)
- CORS: `Cors:Origin`
- Master user: `MASTER_USERNAME`, `MASTER_PASSWORD`, `MASTER_EMAIL` env vars
- Auto-migration on startup + master user seed

## Docker
- Multi-stage build (SDK → Debian slim)
- Non-root `gymapi` user
- Health check at `/health`
- PostgreSQL on port 5433 external
- API on port 5200

## Flutter App
- `/Users/falves/Dev/GymAPP` — iOS-only Flutter app
- Roadmap at `docs/init/Instructions.md`

## Build
```
dotnet build          # builds all 4 projects
dotnet run --project src/Api  # runs locally
make up               # Docker compose up
```
