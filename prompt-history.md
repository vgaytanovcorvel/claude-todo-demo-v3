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

## 10. Multi-User Support (Registration, JWT Auth, Profile Management, Todo Isolation)

```
I want to make this app multi-user so that users can register online, will be able to maintain their profiles, change passwords and view their own task while isolated from other users.
```

User chose "Create a seed user (Recommended)" for migration and "Simple JWT only (Recommended)" for token strategy.

```
implement using independent agents loop: using agents. developer agent: plan and implement the module and tests. review agent: VERY CRITICALLY review implementation on matching applicable rules, report issues detected with associated severity (CRITICAL-HIGH-MEDIUM-LOW). If CRITICAL HIGH-MEDIUM issues are found, revert back to developer agent for fixes and continue this loop until only low priority issues remain.
```

Implemented across 6 phases using the developer/reviewer agent loop:

### Phase 1: Common + Abstractions (1 dev round, no review — pure contracts)
- Added `ValidationConstants` (EmailMaxLength, PasswordMinLength/MaxLength, NameMaxLength)
- Added `RouteConstants` (AuthRoute, UserProfileRoute, AuthRateLimitPolicy)
- Created `User` domain model, `AuthToken` record, 4 request records (Register, Login, UpdateProfile, ChangePassword)
- Created interfaces: `IUserRepository`, `IUserService`, `ITokenService`, `IPasswordHasher`, `ICurrentUserAccessor`
- Created exceptions: `DuplicateEmailException`, `AuthenticationFailedException`
- Modified `ToDoItem` (added UserId), `IToDoItemRepository` (userId params, GetAllByUserId)

### Phase 2: Repository (1 dev round, 1 review round)
- Created `UserEntity`, `UserEntityConfiguration`, `UserRepository`
- Modified `ToDoItemEntity` (UserId FK), `ToDoItemEntityConfiguration` (FK, cascade delete)
- Modified `ToDoItemRepository` (userId filtering on all operations, security fix: Update filters by UserId)
- Modified `ToDoDbContext` (added DbSet<UserEntity>)
- Extracted shared `TestDbContextFactory` from test duplicates
- Tests: 20 ToDoItem + 8 User repository tests

**Review findings fixed**: Hardcoded max-lengths → ValidationConstants, duplicate FK config removed, test naming suffixes added, duplicate TestDbContextFactory extracted.

### Phase 3: Implementation (1 dev round, 1 review round)
- Created `UserService`, `TokenService`, `PasswordHasherWrapper` (wraps Identity PasswordHasher<T>)
- Created `JwtOptions` with SectionName constant
- Created 4 validators: RegisterRequest, LoginRequest, UpdateProfile, ChangePassword
- Modified `ToDoItemService` (ICurrentUserAccessor injection, user-scoped operations)
- Tests: 72 total (UserService 11, TokenService 2, PasswordHasher 3, 4 validator files, modified ToDoItemService)

**Review findings fixed**: PasswordHasher null! → SentinelUser, JwtOptions.SectionName added, mock initialization inline, TokenService missing Verifiable/VerifyAll.

### Phase 4: Web.Core (1 dev round, 1 review round)
- Created `AuthController` ([AllowAnonymous], rate limiting, register/login)
- Created `UserProfileController` ([Authorize], GET/PUT profile, PUT password)
- Created `UserProfileResponse` DTO (strips PasswordHash)
- Created `HttpContextCurrentUserAccessor`
- Modified `ToDoItemsController` (added [Authorize])
- Modified `GlobalExceptionHandlerMiddleware` (DuplicateEmailException 409, AuthenticationFailedException 401)
- Tests: 22 total (AuthController 2, UserProfile 3, HttpContextCurrentUserAccessor 3, modified existing)

**Review findings fixed**: Fully qualified namespace → using, missing ProducesResponseType attributes added.

### Phase 5: Web.Server (1 dev round, 1 review round)
- Configured JWT Bearer authentication with TokenValidationParameters
- Added rate limiting on auth endpoints
- Configured middleware pipeline order
- Tests: 11 integration tests with TestAuthHandler

**Review findings fixed (3 CRITICAL, 4 HIGH, 5 MEDIUM)**:
- CRITICAL: Removed null-forgiving operator on JWT key → proper null check with fail-fast
- CRITICAL: Added startup validation for JWT key presence and minimum 32-byte length
- CRITICAL: Moved JWT signing key from appsettings.Development.json to dotnet user-secrets
- HIGH: Set ClockSkew explicitly to 30 seconds (was default 5 minutes)
- HIGH: Rate limiter returns 429 with ApiResponse envelope (was bare 503)
- HIGH: CORS environment-differentiated (AllowAny only in Development)
- HIGH: Replaced magic string "auth" with RouteConstants.AuthRateLimitPolicy
- MEDIUM: TestAuthHandler nested as private class (one-class-per-file rule)
- MEDIUM: Pinned JwtBearer package version 9.0.13 (was floating 9.0.*)
- MEDIUM: Test factory provides JWT key via AddInMemoryCollection

### Phase 6: Frontend (1 dev round, 1 fix round, 2 review rounds)
- Created auth types, apiClient (centralized fetch with Bearer token), authApiService
- Created AuthContext/AuthProvider, useAuth hook
- Created LoginForm, RegisterForm, AuthPage, ProfilePage (later split into ProfileForm + ChangePasswordForm)
- Modified todoApiService (uses shared apiFetch), App.tsx (auth-gated UI), main.tsx (AuthProvider wrapper)

**Review Round 1 findings (1 CRITICAL, 3 HIGH, 10 MEDIUM)**:
- HIGH: Extracted duplicate apiResponseSchema + handleApiResponse into shared apiClient.ts (DRY)
- MEDIUM: Content-Type only set for methods with body (POST/PUT/PATCH)
- MEDIUM: 401 always throws Error (prevents caller race condition)
- MEDIUM: AuthContext refreshProfile has error handling, login/register handle getProfile failure
- MEDIUM: Extracted zodErrorsToMap utility (4 duplicates → 1 shared function)
- MEDIUM: ProfilePage split into ProfileForm + ChangePasswordForm components

**Review Round 2**: GO — 0 CRITICAL, 0 HIGH, 2 MEDIUM (borderline), 7 LOW. Fixed remaining 2 MEDIUMs:
- setOnUnauthorized returns cleanup function (used in useEffect)
- 401 always throws regardless of callback registration

### Phase 7: Final Verification
- `dotnet build Corvel.ToDo.slnx` — 0 errors, 0 warnings
- `dotnet test Corvel.ToDo.slnx` — **167 tests passing** across 6 test projects
- `npx tsc --noEmit` — TypeScript compiles cleanly

**Final result**: Full multi-user support with JWT auth, per-user todo isolation, profile management, password changes. 167 backend tests, 0 warnings.

## 11. Cross-Module Rules Compliance Review and Fixes

```
start parallel review agents to VERY CRITICALLY review Implementation, Web.Core, Web.Server modules for compliance with RULES. then fix all issues using developer agents, re-run reviewer agents to verify fixes. run one implementing agent at a time with very critical reviewer agent to follow in a loop on main.
```

Launched parallel reviewer agents for Implementation, Web.Core, and Web.Server. Initial reviews found **53 total violations** (5 Critical, 24 Major, 24 Minor). Fixed iteratively using dev/review agent loop.

### Round 1: Initial Reviews (3 parallel reviewer agents)

**Implementation** — 18 violations (1 CRITICAL, 7 MAJOR, 10 MINOR):
- Domain models (`User`, `ToDoItem`) used mutable classes instead of records
- Missing `IValidateOptions<JwtOptions>` startup validation
- Email enumeration vulnerability in UserService
- No shared validation rules (DRY violations across 6 validators)
- Missing `MaximumLength` on LoginRequest/ChangePassword validators
- `new[]` instead of C# 12 collection expressions in TokenService
- Empty primary constructor parentheses on PasswordHasherWrapper

**Web.Core** — 19 violations (2 CRITICAL, 9 MAJOR, 8 MINOR):
- `IMiddleware` pattern instead of convention-based middleware
- Exception messages leaking internal details (DuplicateEmailException, AuthenticationFailedException)
- Domain models exposed directly in controller responses (no DTOs)
- Missing FluentValidation auto-validation wiring
- DI method named `AddWebCoreServices` instead of `AddWebCore`
- Middleware registered as scoped service (should use `UseMiddleware<T>`)
- Two classes in one file (middleware + extensions)
- `InvalidOperationException` instead of `AuthenticationFailedException` in HttpContextCurrentUserAccessor

**Web.Server** — 16 violations (2 CRITICAL, 8 MAJOR, 6 MINOR):
- JWT auth and rate limiting config inline in Program.cs instead of extension methods
- No Central Package Management (Directory.Packages.props)
- Missing HSTS, Swagger, health checks, `AllowedHosts`
- Forbidden direct references to Common/Implementation namespaces
- Middleware pipeline order issues

### Round 2: Fixes (sequential dev agents on main branch)

**Cross-Cutting Agent**: Created `Directory.Packages.props` with 21 centralized package versions. Converted `User` and `ToDoItem` from classes to records with `{ get; init; }`. Updated all test assignments for record compatibility.

**Implementation Agent**: Created `JwtOptionsValidator` (IValidateOptions), `SharedValidationRules` (DRY extension methods). Fixed email enumeration message, collection expressions, empty constructor parens. Refactored all 6 validators to use shared rules. Added `MaximumLength` constraints.

**Web.Core Agent**: Converted middleware to convention-based pattern. Sanitized exception messages. Created `ToDoItemResponse` and `AuthTokenResponse` DTOs. Added `MapToResponse` methods in controllers. Added `AddFluentValidationAutoValidation()`. Renamed DI method to `AddWebCore()`. Fixed HttpContextCurrentUserAccessor exception type.

**Web.Server Agent**: Extracted `AddJwtAuthentication()` and `AddApiRateLimiting()` extension methods. Slimmed Program.cs from ~116 to ~71 lines. Added HSTS, Swagger, health checks, `AllowedHosts`. Removed forbidden namespace usings.

### Round 3: Re-Reviews (3 parallel reviewer agents)

Found **12 remaining violations** across all modules. Fixed 4 actionable items:
1. Middleware pipeline order — moved `UseGlobalExceptionHandler()` to first position
2. Added `UseStaticFiles()` for React SPA serving
3. Consolidated duplicate environment checks into if/else
4. Split two-classes-in-one-file (middleware extensions to separate file)

### Round 4: Final Re-Reviews (3 parallel reviewer agents)

**8 remaining findings** (1 CRITICAL, 5 MEDIUM, 2 LOW) — accepted as sufficient:

| Module | Severity | Finding |
|--------|----------|---------|
| Web.Server | CRITICAL | Empty `AllowedOrigins` in appsettings.json (deployment config, not code) |
| Web.Server | MEDIUM | `AddHealthChecks()` has no actual checks configured |
| Web.Core | MEDIUM | Hardcoded rate limit values (window=1min, limit=10) |
| Web.Core | MEDIUM | Hardcoded JWT key min length `32` |
| Web.Core | MEDIUM | Redundant `LogError` in exception handler (telemetry captures) |
| Implementation | MEDIUM | `PasswordHasherWrapper` uses traditional field instead of primary constructor |
| Implementation | LOW | Unused `ApplyDueDateRules` / duplicated inline due date validation |
| Implementation | LOW | Unused `ApplyEnumRules` in SharedValidationRules |

**Final result**: 53 violations → 8 remaining (all MEDIUM/LOW except 1 deployment config item). 167 tests passing, 0 build errors.
