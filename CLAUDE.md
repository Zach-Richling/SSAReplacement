# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the solution
dotnet build

# Run the API (http://localhost:5192, https://localhost:7102)
dotnet run --project SSAReplacement.Api

# Run the Blazor frontend (http://localhost:5041, https://localhost:7094)
dotnet run --project SSAReplacement.Wasm

# Database migrations run automatically on API startup via EF Core
```

There are no test projects currently in the solution.

## Architecture

**SSAReplacement** is a job scheduling and execution management system — users upload DLL executables, define schedules, and create multi-step jobs that run those executables on a schedule or on-demand.

### Projects

- **SSAReplacement.Api** — ASP.NET Core 10 Minimal API backend
- **SSAReplacement.Wasm** — Blazor WebAssembly frontend

### Backend (Vertical Slice Architecture)

The API is organized by feature under `Features/`, each with `Handlers/` (request/response logic) and `Infrastructure/` (services):

- `Features/Auth/` — Windows/Kerberos authentication bridge that issues JWTs and manages refresh tokens
- `Features/Dashboard/` — Real-time monitoring: job stats summary, upcoming runs, failure spotlight, hourly run history
- `Features/Executables/` — Upload and manage executable DLLs, versioned per executable
- `Features/Jobs/` — CRUD for jobs, step management (ordered `JobStep`s each targeting a specific executable version), schedule assignment, trigger with optional `startAtStep` for resume-on-failure
- `Features/JobRuns/` — Tracking individual job executions; stop a running job; SSE log streaming
- `Features/Schedules/` — Cron-based schedule management synced to Hangfire

**Key services:**
- `JobRunnerService` — Executes jobs step-by-step sequentially. For each step, runs `dotnet <dll>` as a child process, captures stdout/stderr, creates a `JobRunStep`, writes logs via `JobLogWriterBackgroundService`. Passes `JobStepParameter`s as environment variables. Supports `startAtStep` for mid-workflow resume. Stops on first non-zero exit code. Status tracking: Running → Success/Failed/Stopped.
- `JobCancellationService` — `CancellationTokenSource` registry; the `StopJobRun` handler cancels a running job via this service.
- `ScheduleHangfireSyncService` — Keeps Hangfire recurring jobs in sync with the database schedules (create, update, enable/disable, delete).
- `ScheduleRunnerService` — Called by Hangfire to trigger a scheduled job run.
- `JobLogWriterBackgroundService` — Queue-based background worker that batches log writes to the database.
- `FileSystemExecutableStorage` — Stores uploaded DLL versions on disk, organized by executable ID and version number.
- `TokenService` — Generates JWTs (HMAC-SHA256) and manages refresh tokens in the database.
- `AuditService` — Intercepts `SaveChangesAsync` on `IAuditable` entities and writes `AuditEntry` records.
- `ICurrentUserService` / `HttpContextCurrentUserService` — Extracts the current user from the `uid` JWT claim.

**Infrastructure:**
- `AppDbContext.cs` — EF Core DbContext; migrations run on startup
- Negotiate (Kerberos) + JWT Bearer dual authentication configured in `Program.cs`. `POST /auth/token` requires Negotiate; all other protected endpoints require JWT Bearer.
- Hangfire uses in-memory storage (development); configured in `Program.cs`
- API docs available via Scalar UI at `/scalar`

**Configuration (environment variables / appsettings.json):**
- `ConnectionStrings:DefaultConnection` — SQL Server connection string
- `Executables:Path` — File system path for storing uploaded executables
- `Jwt:Secret` — Signing key for JWT tokens
- `Jwt:Issuer`, `Jwt:Audience` — Token issuer/audience (default: `SSAReplacement`)
- `Jwt:ExpirationMinutes` — Access token lifetime (default: 15)
- `Jwt:RefreshTokenExpirationDays` — Refresh token lifetime (default: 7)

### Frontend (Blazor WebAssembly)

Pages under `Pages/` mirror the backend features (Executables, Jobs, Schedules). The `Client/` directory contains typed HTTP client wrappers over `SsaApiClient`.

Real-time log streaming uses Server-Sent Events (`EventSource`). The JS interop lives in `wwwroot/js/event-source.js`, and the Blazor side in `Components/EventConsumer/`.

UI is built with **BlazorBlueprint** components and a custom dark theme (`wwwroot/css/theme.css`). Human-readable cron expressions use `CronExpressionDescriptor`; relative time formatting uses `Humanizer`.

Use the BlazorBlueprint MCP server to learn about Blazor Blueprint.

**Auth components:**
- `JwtAuthenticationStateProvider` — Blazor `AuthenticationStateProvider` that parses JWTs
- `TokenStorageService` — Persists JWT + refresh token in browser session/local storage
- `AuthTokenHandler` — `DelegatingHandler` that injects the JWT `Authorization` header into all API requests

**Pages:**
- `Home.razor` — Dashboard (summary cards, run history chart, upcoming runs, failure spotlight)

**UI conventions:**
- All buttons must include a Lucide icon via the `<Icon>` render fragment (e.g. `<LucideIcon Name="play" Class="h-4 w-4" />`)
- All buttons must use Size="ButtonSize.Small"
- All user-triggered actions must produce a toast notification on success (`Toasts.Success(...)`) and failure (`Toasts.Error(...)`)

### Database Models

```
Job (IAuditable) → JobStep (IAuditable, ordered) → ExecutableVersion
                 → JobStepParameter (IAuditable, env vars per step)
                 → JobSchedule → Schedule (cron expression + Hangfire job key)
                 → JobRun (IAuditable) → JobRunStep → JobLog (stdout/stderr lines)
Executable (IAuditable) → ExecutableVersion → ExecutableParameter (parameter definitions)
User — Windows SID + username, first/last seen timestamps
AuditEntry — records all IAuditable changes (EntityName, EntityId, Action, UserId, OccurredAt)
RefreshToken — JWT refresh tokens with revocation support
```

`IAuditable` entities track `CreatedByUserId`; `AppDbContext.SaveChangesAsync` automatically writes `AuditEntry` records for all changes. All IDs are `long`.
