---
name: "fs-to-ef-migration"
description: "Migrates AntFlowCore repository from FreeSql to EF Core 10. Invoke when user asks to refactor/migrate a Fs repository, convert FreeSql API to EF Core, or continue the ORM migration."
---

# FreeSql to EF Core Repository Migration Skill

## Overview

Migrate AntFlowCore Persist layer repository implementations from FreeSql ORM to Entity Framework Core 10. Each migration replaces a `FsXxxRepository` (FreeSql-based) with an `EfXxxRepository` (EF Core-based), and updates the corresponding interface and service callers.

## Reference Projects

- **Current project (migrating)**: `D:\projects\antflowcore`
- **EF reference project (completed EF version, not modularized)**: `D:\data\antflowcoreef`
  - EF repository implementations: `D:\data\antflowcoreef\antflowcore\conf\ef\`
  - EF service layer (contains mixed EF code not yet extracted to repository): `D:\data\antflowcoreef\antflowcore\service\`

## Architecture Context

```
AntFlowCore.Persist.api  → Interface layer (IXxxRepository + IXxxService interfaces)
AntFlowCore.Persist      → Implementation layer (EfXxxRepository implementations)
AntFlowCore.Abstraction.Orm → ORM base layer (RepositoryBase, AntFlowDbContext, AntFlowOrmContext)
AntFlowCore.Business     → Service layer (XxxService implementations)
```

Key base types:
- `IBaseRepository<TEntity>` — ORM-agnostic repository interface (in `AntFlowCore.Abstraction.Orm.repository`)
- `RepositoryBase<TEntity>` — EF Core repository base implementation (in `AntFlowCore.Abstraction.Orm.repository`, namespace `antflowcore.conf.ef`)
  - Constructor: `RepositoryBase(AntFlowDbContext dbContext)` — injects `AntFlowDbContext`
  - Provides: `DbContext`, `DbSet` protected fields, and standard CRUD methods
- `AntFlowDbContext` — EF Core DbContext (in `AntFlowCore.Abstraction.Orm.context`)
- `AntFlowOrmContext` — Context wrapper exposing `DbContext` property (in `AntFlowCore.Abstraction.Orm.repository`)
  - Constructor: `AntFlowOrmContext(AntFlowDbContext dbContext)`
  - **Keep this class** — it wraps DbContext for backward compatibility
- `IAntFlowRepositoryMix<TEntity, TRepo>` — Mixin interface exposing `_repository` property (in `AntFlowCore.Persist.api.interf.repository`)
  - **Keep `_repository` exposure** during this migration phase

## Naming Convention

- **Repository class**: `Ef` prefix + entity name + `Repository` (e.g., `EfAFTaskRepository`, `EfDictDataRepository`, `EfBpmnConfRepository`)
- **Repository file**: Same as class name (e.g., `EfAFTaskRepository.cs`)
- **Repository interface**: No prefix, just `I` + entity name + `Repository` (e.g., `IAFTaskRepository`, `IDictDataRepository`)
- Old FreeSql repositories used `Fs` prefix (e.g., `FsAFTaskRepository`), new EF Core ones use `Ef` prefix

## Migration Workflow

For each repository to migrate, follow these steps in order:

### Step 1: Analyze the Fs Repository

Read the current `FsXxxRepository` file in `src/AntFlowCore.Persist/repository/`:
- Identify all custom methods beyond the base CRUD
- Identify all FreeSql API usage patterns (see API Mapping below)
- Note the constructor: `FsXxxRepository(AntFlowOrmContext ormContext) : base(ormContext)`

### Step 2: Check EF Reference Project

Check if the EF reference project has a corresponding `EfXxxRepository` in `D:\data\antflowcoreef\antflowcore\conf\ef\`:
- **If exists**: Read it and use its EF Core LINQ implementation as reference. Copy the method implementations but adjust namespaces.
- **If not exists**: Check the EF reference project's service layer `D:\data\antflowcoreef\antflowcore\service\` for EF Core code that should be extracted into the repository. The EF version has many EF operations still mixed in service classes.

**IMPORTANT**: Always reference the EF version's existing LINQ implementations. Do NOT write EF queries from scratch when the EF version already has working code.

### Step 3: Create the Ef Repository Implementation

Create a new `EfXxxRepository` file to replace the old `FsXxxRepository`:

**Before (FreeSql):**
```csharp
using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.repositorysitory; // note: typo in original

public class FsXxxRepository : RepositoryBase<XxxEntity>, IXxxRepository
{
    public FsXxxRepository(AntFlowOrmContext ormContext) : base(ormContext) { }

    public List<XxxEntity> QueryByExpression(Expression<Func<XxxEntity, bool>> expression, PagingInfo pagingInfo)
    {
        BasePagingInfo basePagingInfo = pagingInfo.ToBasePagingInfo();
        List<XxxEntity> list = _ormContext.FreeSql.GetRepository<XxxEntity>()
            .Where(expression)
            .Page(basePagingInfo)
            .OrderByDescending(c => c.CreateTime)
            .ToList();
        pagingInfo.Count = list.Count;
        return list;
    }
}
```

**After (EF Core):**
```csharp
using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.context;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using antflowcore.conf.ef;
using Microsoft.EntityFrameworkCore;

namespace AntFlowCore.Persist.repository;

public class EfXxxRepository : RepositoryBase<XxxEntity>, IXxxRepository
{
    public EfXxxRepository(AntFlowDbContext dbContext) : base(dbContext) { }

    public List<XxxEntity> QueryByExpression(Expression<Func<XxxEntity, bool>> expression, PagingInfo pagingInfo)
    {
        var query = DbSet.Where(expression).OrderByDescending(c => c.CreateTime);
        pagingInfo.Count = query.Count();
        return query
            .Skip((pagingInfo.PageNumber - 1) * pagingInfo.PageSize)
            .Take(pagingInfo.PageSize)
            .ToList();
    }
}
```

Key changes:
- Class name: `FsXxxRepository` → `EfXxxRepository`
- Constructor: `AntFlowOrmContext ormContext` → `AntFlowDbContext dbContext`
- Namespace: `AntFlowCore.Persist.repositorysitory` → `AntFlowCore.Persist.repository`
- Remove all `using FreeSql.*` and `using AntFlowCore.Abstraction.Orm.ext` (FreeSql's BasePagingInfo)
- Add `using AntFlowCore.Abstraction.Orm.context;` and `using Microsoft.EntityFrameworkCore;`
- Data access: `_ormContext.FreeSql.Select<T>()` → `DbSet` / `DbContext.Set<T>()`
- Pagination: `.Page(basePagingInfo)` → `.Skip((pageNumber-1)*pageSize).Take(pageSize)`

### Step 4: Update Repository Interface

Modify `IXxxRepository` in `src/AntFlowCore.Persist.api/interf/repository/`:

- Remove `using FreeSql.Internal.Model;`
- Remove `BasePagingInfo` parameters
- For **join query methods** that use `Expression<Func<T1, T2, bool>>` (FreeSql dual-parameter expression), change the method signature to accept separate filter expressions for each entity type:

**Before:**
```csharp
List<XxxEntity> QueryByExpression(Expression<Func<XxxEntity, BpmnConf, bool>> expression, PagingInfo pagingInfo);
```

**After:**
```csharp
List<XxxEntity> QueryWithBpmnConf(
    Expression<Func<XxxEntity, bool>>? xxxFilter,
    Expression<Func<BpmnConf, bool>>? bpmnConfFilter,
    PagingInfo pagingInfo);
```

This is necessary because EF Core cannot translate dual-parameter expressions to SQL joins. Instead, use LINQ query syntax in the repository implementation:

```csharp
var query = from a in DbSet.Where(xxxFilter ?? (_ => true))
            join b in DbContext.Set<BpmnConf>().Where(bpmnConfFilter ?? (_ => true)) on a.Value equals b.FormCode
            select a;
```

### Step 5: Update Service Callers

Search for all callers of the modified repository methods and update them:

**Before (FreeSql dual-expression):**
```csharp
Expression<Func<DictData, BpmnConf, bool>> expression = (a, b) => a.DictType == "lowcodeflow";
if (condition) expression = LinqExtensions.And(expression, (a, b) => b.EffectiveStatus == state);
var result = _repository.QueryByExpression(expression, pagingInfo);
```

**After (EF Core separate filters):**
```csharp
Expression<Func<DictData, bool>> dictDataFilter = a => a.DictType == "lowcodeflow";
Expression<Func<BpmnConf, bool>>? bpmnConfFilter = null;
if (condition) { var s = state; bpmnConfFilter = b => b.EffectiveStatus == s; }
var result = _repository.QueryWithBpmnConf(dictDataFilter, bpmnConfFilter, pagingInfo);
```

Note: When capturing variables in lambda expressions, use a local variable to avoid captured variable closure issues:
```csharp
var processState = taskMgmtVO.ProcessState.Value; // capture before lambda
bpmnConfFilter = b => b.EffectiveStatus == processState;
```

### Step 6: Update DI Registration

In `src/AntFlowCore.Engine.Abstraction/conf/di/serviceregistration/ServiceRegistration.cs`:

1. Change the repository registration from `AddSingleton` to `AddScoped`, and update the class name:
```csharp
// Before
services.AddSingleton<IXxxRepository, FsXxxRepository>();
// After
services.AddScoped<IXxxRepository, EfXxxRepository>();
```

2. Ensure `AntFlowDbContext` is registered (already done, verify this exists):
```csharp
services.AddDbContext<AntFlowDbContext>(options =>
{
    var connectionString = configuration.GetConnectionString("DefaultConnection")
                           ?? configuration.GetConnectionString("MySqlConnection");
    options.UseMySQL(connectionString);
});
services.AddScoped<AntFlowOrmContext>();
```

### Step 7: Delete Old Fs Repository File

Delete the old `FsXxxRepository.cs` file from `src/AntFlowCore.Persist/repository/`.

## FreeSql to EF Core API Mapping

### Basic CRUD

| FreeSql | EF Core |
|---------|---------|
| `_ormContext.FreeSql.GetRepository<T>().Where(expr).ToList()` | `DbSet.Where(expr).ToList()` |
| `_ormContext.FreeSql.GetRepository<T>().Where(expr).First()` | `DbSet.FirstOrDefault(expr)` |
| `_ormContext.FreeSql.GetRepository<T>().Insert(entity)` | `DbSet.Add(entity)` |
| `_ormContext.FreeSql.GetRepository<T>().Update(entity)` | `DbContext.Entry(entity).State = EntityState.Modified` |
| `_ormContext.FreeSql.GetRepository<T>().Delete(entity)` | `DbSet.Remove(entity)` |
| `_ormContext.FreeSql.Select<T>().Where(expr).ToOne()` | `DbSet.FirstOrDefault(expr)` |
| `_ormContext.FreeSql.Select<T>().Where(expr).ToList()` | `DbSet.Where(expr).ToList()` |
| `_ormContext.FreeSql.Select<T>().Count()` | `DbSet.Count()` |
| `_ormContext.FreeSql.Select<T>().Max(a => a.Code)` | `DbSet.Max(a => a.Code)` |

### Pagination

| FreeSql | EF Core |
|---------|---------|
| `.Page(basePagingInfo)` | `.Skip((pageNumber - 1) * pageSize).Take(pageSize)` |
| `pagingInfo.Count = list.Count` (from Page result) | `pagingInfo.Count = query.Count()` (before Skip/Take) |

Note: `BasePagingInfo` is a FreeSql type (`FreeSql.Internal.Model`). Do NOT use it. Use `PagingInfo` (project's own type in `AntFlowCore.Base.entity`) directly.

### Join Queries

| FreeSql | EF Core |
|---------|---------|
| `_ormContext.FreeSql.Select<T1, T2>().InnerJoin((a, b) => a.Fk == b.Pk).Where(expr).ToList<T1>((a, b) => a)` | `from a in DbSet join b in DbContext.Set<T2>() on a.Fk equals b.Pk select a` |
| `_ormContext.FreeSql.Select<T1, T2>().LeftJoin(...)` | `from a in DbSet join b in DbContext.Set<T2>() on a.Fk equals b.Pk into gj from b in gj.DefaultIfEmpty() select a` |

### Update Operations

| FreeSql | EF Core |
|---------|---------|
| `_ormContext.FreeSql.Update<T>().SetSource(entity).ExecuteAffrows()` | `DbContext.Entry(entity).State = EntityState.Modified; DbContext.SaveChanges();` |
| `_ormContext.FreeSql.Update<T>().Where(expr).Set(a => new T { Prop = value }).ExecuteAffrows()` | `DbSet.Where(expr).ExecuteUpdate(setter => setter.SetProperty(a => a.Prop, value))` |

### Delete Operations

| FreeSql | EF Core |
|---------|---------|
| `_ormContext.FreeSql.Delete<T>().Where(expr).ExecuteAffrows()` | `DbSet.Where(expr).ExecuteDelete()` |

### Conditional Query Building

| FreeSql | EF Core |
|---------|---------|
| `LinqExtensions.True<T>().And(cond1).And(cond2)` | `IQueryable<T> query = DbSet; if (cond1) query = query.Where(expr1); if (cond2) query = query.Where(expr2);` |
| `expression.WhereIf(condition, expr)` | `if (condition) query = query.Where(expr);` |

For single-entity expressions, `LinqExtensions.And()` from `AntFlowCore.Base.extension` still works with EF Core `IQueryable.Where()`.

## Common Patterns

### Pattern 1: Simple Paginated Query

```csharp
// FreeSql
public List<T> QueryPage(Expression<Func<T, bool>> expr, PagingInfo pi) {
    var list = _ormContext.FreeSql.GetRepository<T>().Where(expr).Page(pi.ToBasePagingInfo()).OrderByDescending(c => c.CreateTime).ToList();
    pi.Count = list.Count;
    return list;
}

// EF Core
public List<T> QueryPage(Expression<Func<T, bool>> expr, PagingInfo pi) {
    var query = DbSet.Where(expr).OrderByDescending(c => c.CreateTime);
    pi.Count = query.Count();
    return query.Skip((pi.PageNumber - 1) * pi.PageSize).Take(pi.PageSize).ToList();
}
```

### Pattern 2: Join Query with Pagination

```csharp
// FreeSql
public List<A> QueryWithJoin(Expression<Func<A, B, bool>> expr, PagingInfo pi) {
    return _ormContext.FreeSql.Select<A, B>()
        .InnerJoin((a, b) => a.Fk == b.Pk)
        .Where(expr)
        .OrderByDescending((a, b) => a.CreateTime)
        .Page(pi.ToBasePagingInfo())
        .ToList<A>((a, b) => a);
}

// EF Core
public List<A> QueryWithJoin(Expression<Func<A, bool>>? filterA, Expression<Func<B, bool>>? filterB, PagingInfo pi) {
    var queryA = filterA != null ? DbSet.Where(filterA) : DbSet.AsQueryable();
    var queryB = filterB != null ? DbContext.Set<B>().Where(filterB) : DbContext.Set<B>().AsQueryable();
    var query = from a in queryA
                join b in queryB on a.Fk equals b.Pk
                select a;
    var ordered = query.OrderByDescending(a => a.CreateTime);
    pi.Count = ordered.Count();
    return ordered.Skip((pi.PageNumber - 1) * pi.PageSize).Take(pi.PageSize).ToList();
}
```

### Pattern 3: Dynamic Where with Expression.And

```csharp
// Single-entity dynamic filter - LinqExtensions.And still works
Expression<Func<T, bool>> expr = a => a.Type == "someType";
if (condition) expr = expr.And(a => a.Status == 1);
var result = DbSet.Where(expr).ToList();

// Multi-entity dynamic filter - split into separate expressions
Expression<Func<A, bool>> filterA = a => a.Type == "someType";
Expression<Func<B, bool>>? filterB = null;
if (condition) { var val = someValue; filterB = b => b.Status == val; }
```

### Pattern 4: Bulk Update with ExecuteUpdate

```csharp
// FreeSql
int affrows = _ormContext.FreeSql.Update<BpmAfTask>()
    .Set(a => a.Assignee, assignee)
    .Set(a => a.AssigneeName, assigneeName)
    .Where(a => a.Id == taskId)
    .ExecuteAffrows();

// EF Core
int affrows = DbSet
    .Where(a => a.Id == taskId)
    .ExecuteUpdate(s => s
        .SetProperty(a => a.Assignee, assignee)
        .SetProperty(a => a.AssigneeName, assigneeName));
```

### Pattern 5: Bulk Delete with ExecuteDelete

```csharp
// FreeSql
_ormContext.FreeSql.Delete<BpmAfTask>()
    .Where(predicate)
    .ExecuteAffrows();

// EF Core
DbSet.Where(predicate).ExecuteDelete();
```

## Namespace Mapping

| Current Version (FreeSql) | EF Reference Version | Target (After Migration) |
|---|---|---|
| `AntFlowCore.Persist.repositorysitory` (typo) | `antflowcore.conf.ef` | `AntFlowCore.Persist.repository` |
| `AntFlowCore.Persist.api.interf.repository` | `antflowcore.service.interf.repository` | `AntFlowCore.Persist.api.interf.repository` (no change) |
| `AntFlowCore.Base.entity` | `antflowcore.entity` / `AntFlowCore.Entity` | `AntFlowCore.Base.entity` (no change) |
| `AntFlowCore.Abstraction.Orm.repository` | `antflowcore.conf.ef` | `antflowcore.conf.ef` (for RepositoryBase) |
| `AntFlowCore.Abstraction.Orm.ext` | N/A | Remove (FreeSql's BasePagingInfo) |
| `FreeSql.Internal.Model` | N/A | Remove entirely |

## Files to Clean Up (Already Done)

- `AFBaseCurdRepositoryService.cs` — FreeSql legacy base class, **DELETED**
- `BasePagingInfoExtensions.cs` — FreeSql's BasePagingInfo converter, **DELETED**

## Pitfalls and Lessons Learned

### 1. FreeSql dual-parameter expressions don't work in EF Core
FreeSql's `Expression<Func<T1, T2, bool>>` for join queries cannot be translated by EF Core to SQL. Must split into separate `Expression<Func<T1, bool>>` and `Expression<Func<T2, bool>>` filters, then use LINQ query syntax `from a in ... join b in ... on ... where ...` in the repository.

### 2. BasePagingInfo is a FreeSql type — don't use it
`BasePagingInfo` comes from `FreeSql.Internal.Model`. Use the project's own `PagingInfo` class (in `AntFlowCore.Base.entity`) directly. The `ToBasePagingInfo()` extension method was in the deleted `BasePagingInfoExtensions.cs`.

### 3. Pagination count differs between FreeSql and EF Core
- FreeSql: `.Page(basePagingInfo)` returns the paged list, and `pagingInfo.Count` is set from `list.Count` (which is the page size, NOT the total count). This was actually a bug in the original code.
- EF Core: Call `query.Count()` BEFORE `.Skip().Take()` to get the total count for pagination.

### 4. NuGet package version mismatch
`Microsoft.EntityFrameworkCore 10.0.1` requires `Microsoft.Extensions.Logging` and `Microsoft.Extensions.Options` version 10.0.1 (not 10.0.0). If you see NU1605 downgrade errors, update these package versions in `AntFlowCore.Base.csproj`.

### 5. DbContext must be registered as Scoped
EF Core `DbContext` is not thread-safe and must be `Scoped` lifetime. All repositories depending on it must also be `AddScoped`, not `AddSingleton`. The `AntFlowOrmContext` wrapper must also be `AddScoped`.

### 6. AntFlowDbContext was not initially registered
The `AntFlowDbContext` was not registered in DI — it was only wrapped by `AntFlowOrmContext`. For EF Core repositories to work, `AddDbContext<AntFlowDbContext>()` must be called. This has been added to `ServiceRegistration.cs`.

### 7. Old namespace typo
Many Fs repositories used `namespace AntFlowCore.Persist.repositorysitory` (with typo "repositorysitory"). The new Ef repositories should use `namespace AntFlowCore.Persist.repository` (correct spelling).

### 8. EF version may have repository methods inlined in service
The EF reference project (`D:\data\antflowcoreef`) often inlines repository methods (like `DeleteByExpression`, `UpdateAssignee`, `FindTasksByProcessNumber`) directly in service classes using `_dbContext` or `_repository.GetQueryable()`. In the current modularized project, these should remain as dedicated repository methods. Convert the EF service code back into repository methods.

### 9. Variable capture in lambda closures
When building dynamic `Expression<Func<T, bool>>` filters that capture external variables (like `taskMgmtVO.ProcessState`), always assign to a local variable first:
```csharp
// WRONG - may capture reference, not value
bpmnConfFilter = b => b.EffectiveStatus == taskMgmtVO.ProcessState;

// CORRECT - capture value
var processState = taskMgmtVO.ProcessState.Value;
bpmnConfFilter = b => b.EffectiveStatus == processState;
```

### 10. File encoding must be UTF-8
When writing files using the Write tool, ensure the output is UTF-8 encoded. Non-UTF-8 encoding causes IDE errors when opening files. Verify encoding after writing.

## Validation Checklist

After completing each repository migration, verify:

1. No FreeSql API references (`_ormContext.FreeSql`, `baseRepo`, `IFreeSql`, `BasePagingInfo`) in the new repository file
2. No `using FreeSql.*` in the new repository file
3. Class name uses `Ef` prefix (e.g., `EfAFTaskRepository`, not `AFTaskRepository`)
4. Constructor accepts `AntFlowDbContext dbContext` (not `AntFlowOrmContext`)
5. Namespace is `AntFlowCore.Persist.repository` (not `repositorysitory`)
6. Repository interface has no FreeSql dependencies
7. Service callers updated to use new method signatures
8. DI registration uses `AddScoped` and references `EfXxxRepository` class name
9. Old `FsXxxRepository.cs` file deleted
10. File encoding is UTF-8

## Important Notes

1. **Always reference the EF version first**: Before writing any EF Core query, check `D:\data\antflowcoreef\antflowcore\conf\ef\` for existing implementations. If not found there, check `D:\data\antflowcoreef\antflowcore\service\` for EF code mixed in service classes.
2. **Do NOT write EF queries from scratch** when the EF version already has working code. Copy and adjust namespaces only.
3. **Keep `AntFlowOrmContext`** — it wraps DbContext and is still used by other parts of the system.
4. **Keep `IAntFlowRepositoryMix._repository` exposure** — do not remove during this migration phase.
5. **DbContext lifetime is Scoped** — repository registrations must also be `AddScoped`, not `AddSingleton`.
6. **Variable capture in lambdas**: When capturing loop variables or mutable values in lambda expressions, assign to a local variable first to avoid closure issues.
7. **The project currently cannot fully compile** due to FreeSql removal in progress. Focus on syntax correctness, not full build.
8. **UTF-8 encoding**: When writing files, ensure the output is UTF-8 encoded (with BOM or without BOM, but must be UTF-8). Non-UTF-8 encoding causes IDE errors when opening files. If using the Write tool, verify the file encoding afterward.
9. **Ef prefix naming**: All new EF Core repository implementations must use `Ef` prefix (e.g., `EfAFTaskRepository`), matching the convention of the EF reference project.
