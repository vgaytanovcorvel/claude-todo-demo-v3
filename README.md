# Corvel.ToDo

A full-stack ToDo application built with clean architecture principles. .NET 9 backend with a React (Vite + TypeScript) frontend.

This project was built entirely using [Claude Code](https://claude.com/claude-code) with a developer/reviewer agent loop to enforce coding rules at every step.

## Architecture

```
┌─────────────────────────────────┐
│   corvel.todo.client (React)    │  ← React 19 SPA (Vite + TypeScript)
│   http://localhost:5173         │
└──────────────┬──────────────────┘
               │ HTTP (proxied via Vite)
┌──────────────┴──────────────────┐
│   Web.Server (ASP.NET Core)     │  ← Host, middleware pipeline, CORS
│   https://localhost:5001        │
├─────────────────────────────────┤
│   Web.Core                      │  ← Controllers, global exception handler
├─────────────────────────────────┤
│   Implementation                │  ← Services, FluentValidation, business logic
├─────────────────────────────────┤
│   Repository                    │  ← EF Core, entities, data access
├─────────────────────────────────┤
│   Abstractions                  │  ← Domain models, interfaces, contracts
├─────────────────────────────────┤
│   Common                        │  ← Shared DTOs, enums, constants
└─────────────────────────────────┘
```

**Dependency flow**: Common ← Abstractions ← Implementation/Repository ← Web.Core ← Web.Server

## Features

- Full CRUD for ToDo items (create, read, update, delete)
- Priority levels: Low, Medium, High, Critical
- Status tracking: Pending, In Progress, Completed, Cancelled
- Due date tracking with overdue indicators
- Client-side filtering by status and priority
- Form validation matching backend rules
- API response envelope (`ApiResponse<T>`) with Zod validation on the client
- Global exception handling middleware
- 104 backend unit/integration tests

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | React 19, TypeScript, Vite, Zod |
| Backend | .NET 9, ASP.NET Core, C# 13 |
| Validation | FluentValidation (server), Zod (client) |
| Data Access | Entity Framework Core (SQL Server) |
| Testing | MSTest, Moq, FluentAssertions, FakeTimeProvider |

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/) (for the React frontend)
- SQL Server (or SQL Server LocalDB for development)

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/vgaytanovcorvel/claude-todo-demo-v3.git
cd claude-todo-demo-v3
```

### 2. Configure the database connection

Edit `src/Corvel.ToDo.Web.Server/appsettings.json` (or use user secrets):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CorvelToDo;Trusted_Connection=True;"
  },
  "AllowedOrigins": ["http://localhost:5173"]
}
```

Or use .NET user secrets:

```bash
cd src/Corvel.ToDo.Web.Server
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\mssqllocaldb;Database=CorvelToDo;Trusted_Connection=True;"
```

### 3. Apply database migrations

```bash
dotnet ef database update --project src/Corvel.ToDo.Repository --startup-project src/Corvel.ToDo.Web.Server
```

### 4. Run the backend

```bash
dotnet run --project src/Corvel.ToDo.Web.Server
```

The API will be available at `https://localhost:5001`.

### 5. Run the frontend

```bash
cd src/corvel.todo.client
npm install
npm run dev
```

The app will be available at `http://localhost:5173`. API calls are proxied to the backend automatically.

## Build & Test

```bash
# Build the .NET solution
dotnet build Corvel.ToDo.slnx

# Run all backend tests (104 tests)
dotnet test Corvel.ToDo.slnx

# Build the React frontend for production
cd src/corvel.todo.client
npm run build
```

## API Endpoints

| Method | Endpoint | Description | Response |
|--------|----------|-------------|----------|
| GET | `/api/todo-items` | List all todos | `ApiResponse<ToDoItem[]>` |
| GET | `/api/todo-items/{id}` | Get todo by ID | `ApiResponse<ToDoItem>` |
| POST | `/api/todo-items` | Create a todo | `ApiResponse<ToDoItem>` |
| PUT | `/api/todo-items/{id}` | Update a todo | `ApiResponse<ToDoItem>` |
| DELETE | `/api/todo-items/{id}` | Delete a todo | 204 No Content |

All responses use a consistent envelope:

```json
{
  "success": true,
  "data": { ... },
  "error": null,
  "statusCode": 200
}
```

## Project Structure

```
src/
├── Corvel.ToDo.Common           ← Shared DTOs, enums, constants
├── Corvel.ToDo.Abstractions     ← Domain models, interfaces, exceptions
├── Corvel.ToDo.Implementation   ← Service layer, validators, business logic
├── Corvel.ToDo.Repository       ← EF Core DbContext, entities, repositories
├── Corvel.ToDo.Web.Core         ← Controllers, middleware
├── Corvel.ToDo.Web.Server       ← ASP.NET Core host (Program.cs)
└── corvel.todo.client/          ← React SPA
    └── src/
        ├── types/               ← TypeScript interfaces & enums
        ├── constants/           ← Validation constants
        ├── services/            ← API client (fetch + Zod)
        ├── hooks/               ← useTodos custom hook
        └── components/          ← TodoList, TodoItem, TodoForm, TodoFilters, ConfirmDialog
tests/
├── Corvel.ToDo.Common.Tests
├── Corvel.ToDo.Abstractions.Tests
├── Corvel.ToDo.Implementation.Tests
├── Corvel.ToDo.Repository.Tests
├── Corvel.ToDo.Web.Core.Tests
└── Corvel.ToDo.Web.Server.Tests
```

## Coding Rules

The project enforces coding standards via rule files in `rules/`:

- **Common rules** — Immutability, DRY/KISS/YAGNI, error handling, testing (80% coverage, AAA pattern, strict mocks)
- **C# rules** — Records, FluentValidation, primary constructors, virtual methods, `TimeProvider`, EF Core patterns
- **TypeScript rules** — Immutability via spread, Zod validation at boundaries, no `console.log`

## How It Was Built

This project was scaffolded and implemented entirely using Claude Code with a dev/review agent loop:

1. **Rules installed** — Clean architecture coding rules deployed to `rules/`
2. **Solution bootstrapped** — Project skeletons and CLAUDE.md files generated
3. **Backend implemented** — 4 modules built through iterative dev/review cycles (Implementation → Repository → Web.Core → Web.Server), each going through 2 review rounds until only LOW-severity issues remained
4. **Frontend implemented** — React SPA built through 1 dev round + 1 fix round, reviewed twice until GO verdict

See [prompt-history.md](prompt-history.md) for the full chronological record of prompts used.
