# Complete Testing Setup & Execution Guide

## Overview

The testing architecture for `GymAssistant_API` provides three dedicated test projects with comprehensive case coverage, test isolation, zero impact on production, and dedicated Docker database orchestration.

---

## 1. Test Projects Hierarchy

1. **`GymAssistant.TestCommon`** (Shared Infrastructure & Safety Layer)
   - **`docker-compose.test.yml`**: Docker Compose definition running Microsoft SQL Server 2022 on port `14333` with isolated volume and health check.
   - **`ProductionProtectionGuard`**: Inspects connection strings before registration and aborts execution if production patterns (`databaseasp.net`, `db27400`, `fitrixapp`, etc.) appear.
   - **`SqlDatabaseFixture`**: Prioritizes `docker-compose.test.yml` on port `14333`, seamlessly falls back to dynamic Testcontainers or safe local test databases if Docker is unavailable.
   - **`CustomWebApplicationFactory`**: Custom `WebApplicationFactory<Program>` that overrides DB context configuration with the test container, redirects file uploads to a temporary test directory, and swaps out external services with `FakeEmailService` and `FakePushNotificationService`.
   - **`TestDbContextFactory`**: Provides isolated in-memory EF Core instances for unit tests.

2. **`GymAssistant.UnitTests`**
   - **Localization Tests** (`LocalizationTests.cs`): Verifies resource parity across English (`SharedResources.resx`) and Arabic (`SharedResources.ar.resx`), `Localize()` formatting with dynamic arguments, missing key fallbacks, and production safety guard assertions.
   - **Mapping Tests** (`PredefinedMappingTests.cs`): Verifies entity-to-DTO mappings and localization projections for `Section` and `Exercise` models.
   - **Handler Unit Tests**:
     - `RegisterHandlerTests.cs`: Registration success, duplicate conflict handling.
     - `WorkoutHandlerTests.cs`: Session creation, completion, and error cases.
     - `ExerciseHandlerTests.cs`: Section and exercise retrieval and not-found handling.
     - `CustomExerciseHandlerTests.cs`: Custom exercise creation, image handling, and deletion.
     - `ProgressHandlerTests.cs`: Overview statistics, workout frequencies, and exercise progress.
     - `RecordsHandlerTests.cs`: Personal records and milestones querying.
     - `TrainerHandlerTests.cs`: Trainee retrieval and not-found scenarios.
     - `ProfileHandlerTests.cs`: User profile and body measurement creation.

3. **`GymAssistant.IntegrationTests`**
   - `AuthControllerTests.cs`: Registration validation errors, bad requests, problem details, provider listing.
   - `ExercisesControllerTests.cs`: Section listing, invalid GUID validation, unauthenticated access.
   - `WorkoutsControllerTests.cs`: Securing session creation and workout history.
   - `PredefinedControllerTests.cs`: Predefined routines and exercise queries.
   - `TrainerControllerTests.cs`: Trainer directory and validation checks.
   - `NotificationControllerTests.cs`: Unauthenticated access protection for notifications.

4. **`GymAssistant.E2ETests`**
   - `AuthLifecycleJourneyTests.cs`: Complete user registration, login, and forgot password via `FakeEmailService`.
   - `WorkoutCreationJourneyTests.cs`: Exercise discovery and secured workout execution journey.
   - `TrainerClientJourneyTests.cs`: Trainer lookup, client request creation, and pending request security flow.

---

## 2. Docker DB Commands

### Starting the Test DB Container
```powershell
docker compose -f docker-compose.test.yml up -d
```

### Stopping the Test DB Container
```powershell
docker compose -f docker-compose.test.yml down
```

---

## 3. Running All Tests

```powershell
# Run Unit Tests
dotnet test tests/GymAssistant.UnitTests/GymAssistant.UnitTests.csproj

# Run Integration Tests
dotnet test tests/GymAssistant.IntegrationTests/GymAssistant.IntegrationTests.csproj

# Run E2E Tests
dotnet test tests/GymAssistant.E2ETests/GymAssistant.E2ETests.csproj
```
