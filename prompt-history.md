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

## 7. Implement Modules with Dev/Review Loop

```
implement scafflded projects one by one using agents. developer agent: plan and implement the module and tests. review agentt: VERY CRICITCALLY review implementation on matching applicable rules, report issues detected with associated severity (CRITICAL-HIGH-MEDIUM-LOW). If CRICIAL HIGH-MEDIUM issues are found, revert back to develor agent for fixes and continue this loop until only low priority issues remain.
```

Implemented all 4 remaining modules (Common and Abstractions were already scaffolded with code) using a developer/reviewer agent loop:

### Module 1: Implementation (2 dev rounds, 2 review rounds)
- **Dev Round 1**: Created `ToDoItemService`, `CreateToDoItemRequestValidator`, `UpdateToDoItemRequestValidator`, DI extension, and 29 tests.
- **Review Round 1**: NO-GO — 1 HIGH (redundant mock re-initialization), 3 MEDIUM (DI extension in wrong folder, validator test method naming, validator tests missing Mock\<SUT\> pattern).
- **Dev Round 2**: Fixed all 4 issues.
- **Review Round 2**: GO — only 1 MEDIUM (style) + 1 LOW remaining.

### Module 2: Repository (2 dev rounds, 1 review round)
- **Dev Round 1**: Created `ToDoItemEntity`, `ToDoDbContext`, `ToDoItemEntityConfiguration`, `RepositoryBase`, `ToDoItemRepository`, DI extension, and 11 tests.
- **Review Round 1**: NO-GO — 3 MEDIUM (DI method naming `AddRepositoryServices` should be `AddPersistence`, `TestDbContextFactory` missing async override, missing `TestCleanup`).
- **Dev Round 2**: Fixed all 3 issues. GO.

### Module 3: Web.Core (2 dev rounds, 2 review rounds)
- **Dev Round 1**: Created `ToDoItemsController`, `GlobalExceptionHandlerMiddleware`, DI extension, and 11 tests.
- **Review Round 1**: NO-GO — 3 HIGH (missing `[ProducesResponseType]`, null-forgiving operator in Delete, controller mock missing `MockBehavior.Strict`), 3 MEDIUM (Delete returns 204 with body, middleware registered as Transient, mock fields not re-initialized in Setup).
- **Dev Round 2**: Fixed all 6 issues.
- **Review Round 2**: GO — only 3 LOW remaining.

### Module 4: Web.Server (2 dev rounds, 2 review rounds)
- **Dev Round 1**: Created `Program.cs` with full middleware pipeline, `appsettings.json`, `ToDoWebApplicationFactory`, and 8 integration tests.
- **Review Round 1**: NO-GO — 1 HIGH (CORS before Routing in pipeline), 4 MEDIUM (missing HTTPS redirection, null-forgiving operator in tests, missing Update 400/404 and GetAll-with-data tests).
- **Dev Round 2**: Fixed all issues, added 3 new tests (total 11).
- **Review Round 2**: GO — only 4 LOW remaining.

**Final result**: 104 tests passing across 6 test projects, 0 warnings, 0 errors.

## 8. Save Agent Prompts to Memory

```
update prompts with used prompts
```

Saved developer/reviewer agent prompt templates to memory (`dev-review-prompts.md`).

## 9. Implement Frontend with Dev/Review Loop

```
did we implement front end? implement scafflded projects one by one using agents. developer agent: plan and implement the module and tests. review agentt: VERY CRICITCALLY review implementation on matching applicable rules, report issues detected with associated severity (CRITICAL-HIGH-MEDIUM-LOW). If CRICIAL HIGH-MEDIUM issues are found, revert back to develor agent for fixes and continue this loop until only low priority issues remain.
```

Implemented the React frontend (`corvel.todo.client`) from scaffold using the dev/review agent loop. Backend was already 100% complete (104 tests passing).

### corvel.todo.client (1 dev round, 1 fix round, 2 review rounds)

**Dev Round 1**: Created 17 files:
- `types/todo.ts` — TypeScript interfaces, const enums (`Priority`, `ToDoItemStatus`), helper label functions
- `constants/validation.ts` — `TITLE_MAX_LENGTH=200`, `DESCRIPTION_MAX_LENGTH=2000`
- `services/todoApiService.ts` — HTTP client using native `fetch` with `ApiResponse<T>` envelope unwrapping
- `hooks/useTodos.ts` — Custom hook for CRUD state management with immutable updates
- `components/TodoList.tsx` — Main list with table layout, loading/error/empty states
- `components/TodoItem.tsx` — Single row with color-coded priority/status badges, overdue indicator
- `components/TodoForm.tsx` — Modal form for create/edit with client-side validation
- `components/TodoFilters.tsx` — Client-side filtering by status and priority
- `components/ConfirmDialog.tsx` — Delete confirmation modal with keyboard (Escape) support
- `App.tsx` (rewritten), `App.css`, plus CSS for each component
- TypeScript compilation (`tsc --noEmit`) and production build (`npm run build`) both pass.

**Review Round 1**: NO-GO — 6 MEDIUM issues:
1. DRY violation — duplicate `ALL_PRIORITIES`/`ALL_STATUSES` in TodoForm and TodoFilters
2. Missing Zod validation on API responses at system boundary
3. Unsafe type assertions (`undefined as T`, `body.data as T`)
4. Unhandled promise rejection in `handleDeleteConfirm`
5. Hardcoded "Delete" text in generic `ConfirmDialog`
6. Raw error messages from API shown directly to users
7. Unsafe `as` type casts in `handleFormSubmit`

**Fix Round**: Fixed all 7 MEDIUM issues:
- Extracted shared constants to `types/todo.ts`
- Installed `zod`, added `apiResponseSchema` validation on all API responses
- Removed unsafe casts; `deleteTodo` handles 204 directly, added null guard on `body.data`
- Added try/catch with user-friendly error in `handleDeleteConfirm`
- Added `confirmLabel` prop to `ConfirmDialog`
- Changed error messages to friendly "Failed to save/delete todo. Please try again."
- Used `'status' in request` for proper type narrowing instead of forced casts

**Review Round 2**: GO — all 7 MEDIUM issues resolved, only 4 LOW remaining (enum casts on controlled selects, `err.message` leaking in fetch path, missing `aria-live` for dynamic errors, non-JSON response handling in delete).

**Final result**: 17 frontend files, TypeScript strict compilation passing, production build passing (206 KB JS, 5.4 KB CSS).
