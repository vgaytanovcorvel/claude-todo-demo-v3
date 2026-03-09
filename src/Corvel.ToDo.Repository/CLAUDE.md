# Corvel.ToDo.Repository

## Rules

@../../rules/common/coding-style.md
@../../rules/common/patterns.md
@../../rules/common/security.md
@../../rules/csharp/coding-style.md
@../../rules/csharp/persistence.md
@../../rules/csharp/security.md

## Module Purpose

Implements the data access layer using Entity Framework Core. Contains DbContext, ORM entity classes, repository implementations, migrations, and DI registration.

## Key Contents

- `ApplicationDbContext` (DbContext)
- ORM entity classes (e.g., `ToDoItemEntity` — with navigation properties, EF attributes)
- Repository implementations (e.g., `ToDoRepository : RepositoryBase<ApplicationDbContext>, IToDoRepository`)
- Entity type configurations (`IEntityTypeConfiguration<T>`)
- EF Core migrations
- DI registration extension (`AddPersistence`)

## Dependency Constraints

- **Allowed**: Corvel.ToDo.Abstractions, Corvel.ToDo.Common, EF Core packages
- **Forbidden**: Must NOT reference Implementation, Web.Core, Web.Server, or any Web.* project
