# Prompt History

Chronological record of prompts used to build the Corvel.ToDo project.

## 1. Install Clean Architecture Rules

```
/install-clean-arch-rules
```

Deployed coding rule files to `rules/` (common, csharp, typescript).

## 2. Bootstrap Solution

```
Bootstrap Corvel.ToDo clean architecture solution, confirm plan before development.
```

Triggered `/bootstrap-clean-arch Corvel.ToDo`. Claude presented a plan with all available modules and asked which to include.

## 3. Select Modules and Constraints

```
we won't need client. Use React instead of Angular. Limit to scaffolding only and bootstrapping claude.md rules
```

Narrowed scope to: Common, Abstractions, Implementation, Repository, Web.Core, Web.Server, and a React (Vite + TypeScript) frontend. No starter code — project skeletons and CLAUDE.md files only.

## 4. Execute Plan

```
go
```

Scaffolded all projects, created `Corvel.ToDo.slnx`, added test projects, generated 14 CLAUDE.md files, and verified the build passes.

## 5. Add .gitignore

```
add .gitignore
```

Created `.gitignore` covering .NET, Node/React, IDE files, test coverage, and secrets.

## 6. Capture Prompt History

```
create prompt-history.md and capture all prompts we have used to build project
```

Created this file.
