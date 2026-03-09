# Corvel.ToDo

Clean architecture ToDo application with a .NET 9 backend and React (Vite + TypeScript) frontend.

## Solution Structure

```
Corvel.ToDo.slnx
src/
├── Corvel.ToDo.Common           ← Shared DTOs, constants, utilities (client + server)
├── Corvel.ToDo.Abstractions     ← Domain models, interfaces, contracts, exceptions
├── Corvel.ToDo.Implementation   ← Service implementations, business logic, validation
├── Corvel.ToDo.Repository       ← EF Core DbContext, entities, repositories, migrations
├── Corvel.ToDo.Web.Core         ← Controllers, middleware, filters
├── Corvel.ToDo.Web.Server       ← ASP.NET Core host (Program.cs, middleware pipeline)
└── corvel.todo.client           ← React SPA (Vite + TypeScript)
tests/
├── Corvel.ToDo.Common.Tests
├── Corvel.ToDo.Abstractions.Tests
├── Corvel.ToDo.Implementation.Tests
├── Corvel.ToDo.Repository.Tests
├── Corvel.ToDo.Web.Core.Tests
└── Corvel.ToDo.Web.Server.Tests
```

## Dependency Flow

```
Common ← (no dependencies)
Abstractions ← Common
Implementation ← Abstractions, Common
Repository ← Abstractions, Common
Web.Core ← Abstractions, Implementation, Common
Web.Server ← Web.Core, Implementation, Repository
corvel.todo.client ← (independent — communicates via HTTP)
```

## Rules Architecture

Coding rules are deployed in `rules/` and referenced by each project's CLAUDE.md:

- `rules/common/` — Language-agnostic conventions (coding style, logging, patterns, security, testing)
- `rules/csharp/` — C#-specific rules (coding style, domain, services, persistence, presentation, hosting, security, testing, modularization)
- `rules/typescript/` — TypeScript-specific rules (coding style, patterns, security, testing)

## Build & Test

```bash
# Build .NET solution
dotnet build Corvel.ToDo.slnx

# Run tests
dotnet test Corvel.ToDo.slnx

# React client (requires npm install first)
cd src/corvel.todo.client && npm install && npm run dev
```
