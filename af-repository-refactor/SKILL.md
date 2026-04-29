---
name: af-repository-refactor
description: This skill should be used when refactoring AntFlowCore service classes that directly extend AFBaseCurdRepositoryService to use the IAntFlowRepositoryMix pattern. This decouples services from FreeSql-specific base classes, making them ORM-agnostic. Trigger when the user asks to refactor a service that inherits AFBaseCurdRepositoryService or wants to migrate a service to the repository pattern.
---

# AF Repository Refactor Skill

## Overview

Refactor AntFlowCore service classes from directly extending `AFBaseCurdRepositoryService<TEntity>` (which tightly couples to IFreeSql) to using the `IAntFlowRepositoryMix<TEntity, TRepo>` pattern. This introduces a repository abstraction layer, making the codebase ORM-agnostic and easier to migrate to other ORMs.

## Architecture Context

The project follows this layered structure:

```
AntFlowCore.Persist.api  → Interface layer (service & repository interfaces)
AntFlowCore.Persist      → Implementation layer (service & repository implementations)
AntFlowCore.AspNetCore   → DI registration (ServiceRegistration.cs)
```

Key base types:
- `IBaseRepository<TEntity>` — ORM-agnostic repository interface (in `AntFlowCore.Abstraction.Orm.repository`)
- `RepositoryBase<TEntity>` — FreeSql repository base implementation (in `AntFlowCore.Abstraction.Orm.repository`)
- `AntFlowOrmContext` — FreeSql context wrapper, injected into `RepositoryBase` constructors
- `IAntFlowRepositoryMix<TEntity, TRepo>` — Mixin interface exposing `_repository` property (in `AntFlowCore.Persist.api.interf.repository`)
- `AFBaseCurdRepositoryService<TEntity>` — Legacy base class tightly coupled to IFreeSql (to be eliminated)

## Refactoring Workflow

For each service to refactor, follow these steps in order:

### Step 1: Create Repository Interface

In `src/AntFlowCore.Persist.api/interf/repository/`, create `I{EntityName}Repository.cs`:

```csharp
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;  // entity namespace may vary

namespace AntFlowCore.Persist.api.interf.repository;

public interface I{EntityName}Repository : IBaseRepository<{EntityName}>
{
}
```

Check if the repository interface already exists before creating it.

### Step 2: Create Repository Implementation

In `src/AntFlowCore.Persist/repo/`, create `Fs{EntityName}Repository.cs`:

```csharp
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace antflowcore.conf.ef;

public class Fs{EntityName}Repository : RepositoryBase<{EntityName}>, I{EntityName}Repository
{
    public Fs{EntityName}Repository(AntFlowOrmContext context) : base(context)
    {
    }
}
```

Check if the repository implementation already exists before creating it.

### Step 3: Refactor Service Interface

Modify `I{EntityName}Service` in `src/AntFlowCore.Persist.api/interf/repository/`:

**Before:**
```csharp
public interface I{EntityName}Service : IBaseRepositoryService<{EntityName}>
```

**After:**
```csharp
using antflowcore.service.interf.repository;

public interface I{EntityName}Service : IAntFlowRepositoryMix<{EntityName}, I{EntityName}Repository>
```

Remove any `using FreeSql;` if no longer needed.

### Step 4: Refactor Service Implementation

Modify `{EntityName}Service` in `src/AntFlowCore.Persist/repository/`:

**Before:**
```csharp
public class {EntityName}Service : AFBaseCurdRepositoryService<{EntityName}>, I{EntityName}Service
{
    public {EntityName}Service(IFreeSql freeSql) : base(freeSql) { }
    
    // uses this.baseRepo.xxx
}
```

**After:**
```csharp
public class {EntityName}Service : I{EntityName}Service
{
    public {EntityName}Service(I{EntityName}Repository repository)
    {
        _repository = repository;
    }

    public I{EntityName}Repository _repository { get; }
    
    // uses _repository.xxx
}
```

### Step 5: Fix Callers (baseRepo → _repository migration)

Search the entire codebase for references to the old service's `baseRepo` property and convert them:

| Old Pattern | New Pattern |
|---|---|
| `service.baseRepo.Where(predicate).ToList()` | `service._repository.Find(predicate)` |
| `service.baseRepo.Where(predicate).First()` | `service._repository.FirstOrDefault(predicate)` |
| `service.baseRepo.Insert(entity)` | `service._repository.Add(entity)` |
| `service.baseRepo.Update(entity)` | `service._repository.Update(entity)` |
| `service.baseRepo.Delete(entity)` | `service._repository.Remove(entity)` |
| `service.baseRepo.Select.ToList()` | `service._repository.GetAll()` |
| `service._repository.GetQueryable().Where(predicate).ToList()` | `service._repository.Find(predicate)` |
| `service._repository.GetQueryable().Where(predicate).First()` | `service._repository.FirstOrDefault(predicate)` |
| `service._repository.GetQueryable().Count()` | `service._repository.Count()` |

Also check for callers that reference `Frsql` (IFreeSql property) on the service if it was exposed.

### Step 6: Register Repository in DI

In `src/AntFlowCore.AspNetCore/conf/di/serviceregistration/ServiceRegistration.cs`, add the repository registration near the existing repository registrations (around the line with `//=========`):

```csharp
services.AddSingleton<I{EntityName}Repository, Fs{EntityName}Repository>();
```

Insert it after the last existing repository registration, before the `//=================================` line.

## Validation Checklist

After completing all steps, verify:

1. No compilation errors in the refactored service
2. No remaining references to `.baseRepo` on the service type
3. Repository is registered in `ServiceRegistration.cs`
4. The service no longer depends on `AFBaseCurdRepositoryService` or `IFreeSql` directly
5. All callers updated to use `_repository` instead of `baseRepo`

## Common Pitfalls

- Some services expose `IFreeSql Frsql { get; }` — this also needs to be removed or replaced
- Some callers use `ServiceProviderUtils.GetService<XxxService>()` to get the service and then access `baseRepo` — these also need fixing
- The `IAntFlowRepositoryMix` interface is in namespace `antflowcore.service.interf.repository` (lowercase) — ensure correct using
- Repository implementations go in namespace `antflowcore.conf.ef` (lowercase) — match existing convention
