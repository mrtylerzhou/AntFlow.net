---
name: "fs-to-sqlsugar-migration"
description: "Migrates AntFlowCore repository from FreeSql to SqlSugar ORM. Invoke when user asks to refactor/migrate a FreeSql repository, convert FreeSql API to SqlSugar, or continue the ORM migration."
---

# FreeSql to SqlSugar Repository Migration Skill

## Overview

Migrate AntFlowCore Persist layer repository implementations from FreeSql ORM to SqlSugar 5.x. Each migration replaces a `FsXxxRepository` (FreeSql-based) with a `SsXxxRepository` or updated `XxxRepository` (SqlSugar-based), and updates the corresponding infrastructure and service callers.

## Reference Projects

- **Current project (migrating)**: `D:\projects\antflowcore`
- **SqlSugar reference project (completed SqlSugar version, not modularized)**: `D:\data\antflow-sqlsugar-real\antflowcore`

## Architecture Context

```
AntFlowCore.Persist.api       → Interface layer (IXxxRepository interfaces)
AntFlowCore.Persist           → Implementation layer (SsXxxRepository implementations)
AntFlowCore.Abstraction.Orm   → ORM base layer (RepositoryBase, AntFlowOrmContext, IBaseRepository)
AntFlowCore.Base              → Entity definitions, VO, DTO, utilities
AntFlowCore.Business          → Service layer (XxxService implementations)
```

Key base types:
- `IBaseRepository<TEntity>` — ORM-agnostic repository interface (in `AntFlowCore.Abstraction.Orm.repository`)
  - `GetQueryable()` returns `ISugarQueryable<TEntity>` (NOT `IQueryable<TEntity>`)
- `RepositoryBase<TEntity>` — SqlSugar repository base implementation (in `AntFlowCore.Abstraction.Orm.repository`)
  - Constructor: `RepositoryBase(AntFlowOrmContext ormContext)`
  - Provides: `Db` (ISqlSugarClient) and `Query` (ISugarQueryable<TEntity>) properties
- `AntFlowOrmContext` — Context wrapper exposing `SqlSugar` property (ISqlSugarClient)
- `AFBaseCurdRepositoryService<T>` — Alternative base class for repositories that directly inject ISqlSugarClient

## Naming Convention

- **Repository class**: If the original Fs repository has `Fs` prefix (e.g., `FsDictDataRepository`), the new SqlSugar repository gets `Ss` prefix (e.g., `SsDictDataRepository`). If the original does NOT have `Fs` prefix, the new one does NOT get `Ss` prefix either.
- **Repository interface**: No prefix, just `I` + entity name + `Repository` (e.g., `IDictDataRepository`, `IBpmnConfRepository`)
- Old FreeSql repositories use `Fs` prefix (e.g., `FsDictDataRepository`) — delete them after migration
- **Rule**: `FsXxxRepository` → `SsXxxRepository` | `XxxRepository` → `XxxRepository` (no change)

## Migration Workflow

For each repository to migrate, follow these steps in order:

### Step 1: Analyze the Fs Repository

Read the current `FsXxxRepository` file:
- Identify all custom methods beyond the base CRUD
- Identify all FreeSql API usage patterns (see API Mapping below)
- Note the constructor: typically `FsXxxRepository(AntFlowOrmContext ormContext) : base(ormContext)`
- Check for `BasePagingInfo` usage (FreeSql-specific type)

### Step 2: Check SqlSugar Reference Project

Check if the SqlSugar reference project has a corresponding repository in `D:\data\antflow-sqlsugar-real\antflowcore\service\repository\`:
- **If exists**: Read it and use its SqlSugar implementation as reference. Copy the method implementations but adjust namespaces.
- **If not exists**: Write SqlSugar API calls from scratch using the API Mapping below.

### Step 3: Create the Ss Repository Implementation

Create a new repository file or update existing one:

**Before (FreeSql):**
```csharp
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.repositorysitory;

public class DictDataRepository : RepositoryBase<DictData>, IDictDataRepository
{
    public DictDataRepository(AntFlowOrmContext ormContext) : base(ormContext) { }

    public List<DictData> QueryDictDataListByExpression(Expression<Func<DictData, bool>> expression, PagingInfo pagingInfo)
    {
        BasePagingInfo basePagingInfo = pagingInfo.ToBasePagingInfo();
        List<DictData> dictDatas = _ormContext.FreeSql.GetRepository<DictData>()
            .Where(expression)
            .Page(basePagingInfo)
            .OrderByDescending(c => c.CreateTime)
            .ToList();
        pagingInfo.Count = dictDatas.Count;
        return dictDatas;
    }
}
```

**After (SqlSugar):**
```csharp
using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class DictDataRepository : RepositoryBase<DictData>, IDictDataRepository
{
    public DictDataRepository(AntFlowOrmContext ormContext) : base(ormContext) { }

    public List<DictData> QueryDictDataListByExpression(Expression<Func<DictData, bool>> expression, PagingInfo pagingInfo)
    {
        int totalCount = 0;
        List<DictData> dictDatas = Db.Queryable<DictData>()
            .Where(expression)
            .OrderByDescending(c => c.CreateTime)
            .ToPageList(pagingInfo.PageNumber, pagingInfo.PageSize, ref totalCount);
        pagingInfo.Count = totalCount;
        return dictDatas;
    }
}
```

Key changes:
- Remove all `using FreeSql.*` and `using AntFlowCore.Abstraction.Orm.ext`
- Add `using SqlSugar;`
- `_ormContext.FreeSql.GetRepository<T>()` → `Db.Queryable<T>()`
- `_ormContext.FreeSql.Select<T>()` → `Db.Queryable<T>()`
- `.Page(basePagingInfo)` → `.ToPageList(pageNumber, pageSize, ref totalCount)`
- `pagingInfo.Count = list.Count` → `pagingInfo.Count = totalCount` (ref parameter)

## FreeSql to SqlSugar API Mapping

### Basic CRUD

| FreeSql | SqlSugar |
|---------|----------|
| `_ormContext.FreeSql.GetRepository<T>().Where(expr).ToList()` | `Db.Queryable<T>().Where(expr).ToList()` |
| `_ormContext.FreeSql.Select<T>().Where(expr).First()` | `Db.Queryable<T>().Where(expr).First()` |
| `_ormContext.FreeSql.GetRepository<T>().Insert(entity)` | `Db.Insertable(entity).ExecuteCommand()` |
| `_ormContext.FreeSql.GetRepository<T>().Update(entity)` | `Db.Updateable(entity).ExecuteCommand()` |
| `_ormContext.FreeSql.GetRepository<T>().Delete(entity)` | `Db.Deleteable(entity).ExecuteCommand()` |
| `_ormContext.FreeSql.Select<T>().Where(expr).ToOne()` | `Db.Queryable<T>().Where(expr).First()` |
| `_ormContext.FreeSql.Select<T>().Where(expr).Count()` | `Db.Queryable<T>().Where(expr).Count()` |
| `_ormContext.FreeSql.Select<T>().Max(a => a.Code)` | `Db.Queryable<T>().Max(a => a.Code)` |
| `_ormContext.FreeSql.Select<T>(id).First()` | `Db.Queryable<T>().InSingle(id)` |
| `GetRepository<T>().Select.AsQueryable()` | `Db.Queryable<T>()` |

### Pagination

| FreeSql | SqlSugar |
|---------|----------|
| `.Page(basePagingInfo)` | `.ToPageList(pageNumber, pageSize, ref totalCount)` |
| `pagingInfo.Count = list.Count` (after Page) | `pagingInfo.Count = totalCount` (ref parameter, set BEFORE data returned) |

**IMPORTANT**: SqlSugar's `ToPageList` automatically sets the `ref totalCount` parameter with the TOTAL count of matching records. FreeSql's `.Page()` returns only the current page's items, and the original code incorrectly set `pagingInfo.Count = list.Count` (page size, not total). SqlSugar fixes this bug.

### Join Queries

| FreeSql | SqlSugar |
|---------|----------|
| `_ormContext.FreeSql.Select<T1, T2>().InnerJoin((a, b) => a.Fk == b.Pk).Where(expr).ToList<T1>((a, b) => a)` | `Db.Queryable<T1>().InnerJoin<T2>((a, b) => a.Fk == b.Pk).Where(expr).ToList()` |
| `_ormContext.FreeSql.Select<T1, T2>().LeftJoin(...)` | `Db.Queryable<T1>().LeftJoin<T2>((a, b) => a.Fk == b.Pk)` |
| Dual-parameter `Expression<Func<T1, T2, bool>>` | SqlSugar supports dual-parameter expressions natively |

### Update Operations

| FreeSql | SqlSugar |
|---------|----------|
| `_ormContext.FreeSql.Update<T>().SetSource(entity).ExecuteAffrows()` | `Db.Updateable(entity).ExecuteCommand()` |
| `_ormContext.FreeSql.Update<T>().Where(expr).Set(a => new T { Prop = value }).ExecuteAffrows()` | `Db.Updateable<T>().SetColumns(a => a.Prop == value).Where(expr).ExecuteCommand()` |

### Delete Operations

| FreeSql | SqlSugar |
|---------|----------|
| `_ormContext.FreeSql.Delete<T>().Where(expr).ExecuteAffrows()` | `Db.Deleteable<T>().Where(expr).ExecuteCommand()` |
| `_ormContext.FreeSql.Delete<T>(entity).ExecuteAffrows()` | `Db.Deleteable(entity).ExecuteCommand()` |

### Conditional Query Building

| FreeSql | SqlSugar |
|---------|----------|
| `LinqExtensions.True<T>().And(cond1).And(cond2)` | `SqlSugar.LinqExtensions.True<T>().And(cond1).And(cond2)` (same API) |
| `expression.WhereIf(condition, expr)` | `SqlSugar.LambdaExtensions.WhereIf(expression, condition, expr)` or `if (condition) query = query.Where(expr)` |
| `LinqExtensions.And(expr1, expr2)` | Same in SqlSugar: `LinqExtensions.And(expr1, expr2)` |

### Async Methods

| FreeSql | SqlSugar |
|---------|----------|
| `.ToListAsync(cancellationToken)` | `.ToListAsync(cancellationToken)` |
| `.FirstAsync(cancellationToken)` | `.FirstAsync(cancellationToken)` |
| `.InsertAsync(entity, cancellationToken)` | `.Insertable(entity).ExecuteCommandAsync(cancellationToken)` |
| `.DeleteAsync(predicate, cancellationToken)` | `.Deleteable<T>().Where(predicate).ExecuteCommandAsync(cancellationToken)` |

## Namespace Mapping

| Current Version (FreeSql) | SqlSugar Reference Version | Target (After Migration) |
|---|---|---|
| `AntFlowCore.Persist.repositorysitory` (typo) | `antflowcore.service.repository` | `AntFlowCore.Persist.repository` |
| `AntFlowCore.Abstraction.Orm.ext` (BasePagingInfoExtensions) | N/A | Remove or replace with SqlSugar pagination |
| `FreeSql.Internal.Model` | N/A | Remove entirely |
| `AntFlowCore.Abstraction.Orm.repository` (RepositoryBase) | `antflowcore.service.repository` | `AntFlowCore.Abstraction.Orm.repository` (no change) |

## Infrastructure Files Already Migrated

The following core infrastructure files have been migrated from FreeSql to SqlSugar:

1. **`AntFlowOrmContext.cs`**: `IFreeSql FreeSql` → `ISqlSugarClient SqlSugar`
2. **`RepositoryBase<TEntity>.cs`**: All FreeSql API → SqlSugar API
3. **`AFBaseCurdRepositoryService<T>.cs`**: FreeSql → SqlSugar
4. **`IBaseRepository<TEntity>.cs`**: `GetQueryable()` returns `ISugarQueryable<TEntity>` (NOT `IQueryable<TEntity>`)
5. **`BasePagingInfoExtensions.cs`**: Replaced FreeSql's `BasePagingInfo` conversion with SqlSugar pagination helper
6. **`SqlSugarSetUp.cs`**: SqlSugarScope registration as Singleton
7. **`SqlSugarFluentConfiguration.cs`**: Entity mapping configuration (migrated from FreeSql reference project)
8. **`TransactionalAttribute.cs`**: FreeSql UoW → SqlSugar `Ado.BeginTran/CommitTran/RollbackTran`
9. **`Program.cs`**: `FreeSqlSet` → `SqlSugarSet`

## Common Patterns

### Pattern 1: Simple Paginated Query

```csharp
// FreeSql
public List<T> QueryPage(Expression<Func<T, bool>> expr, PagingInfo pi) {
    BasePagingInfo basePagingInfo = pi.ToBasePagingInfo();
    var list = _ormContext.FreeSql.GetRepository<T>().Where(expr).Page(basePagingInfo).OrderByDescending(c => c.CreateTime).ToList();
    pi.Count = list.Count;  // BUG: sets page size, not total count
    return list;
}

// SqlSugar
public List<T> QueryPage(Expression<Func<T, bool>> expr, PagingInfo pi) {
    int totalCount = 0;
    var list = Db.Queryable<T>().Where(expr).OrderByDescending(c => c.CreateTime)
        .ToPageList(pi.PageNumber, pi.PageSize, ref totalCount);
    pi.Count = totalCount;  // CORRECT: sets total count
    return list;
}
```

### Pattern 2: Join Query with Pagination

```csharp
// FreeSql
public List<A> QueryWithJoin(Expression<Func<A, B, bool>> expr, PagingInfo pi) {
    BasePagingInfo basePagingInfo = pi.ToBasePagingInfo();
    return _ormContext.FreeSql.Select<A, B>()
        .InnerJoin((a, b) => a.Fk == b.Pk)
        .Where(expr)
        .OrderByDescending((a, b) => a.CreateTime)
        .Page(basePagingInfo)
        .ToList<A>((a, b) => a);
}

// SqlSugar
public List<A> QueryWithJoin(Expression<Func<A, B, bool>> expr, PagingInfo pi) {
    int totalCount = 0;
    return Db.Queryable<A>()
        .InnerJoin<B>((a, b) => a.Fk == b.Pk)
        .Where(expr)
        .OrderByDescending(a => a.CreateTime)
        .ToPageList(pi.PageNumber, pi.PageSize, ref totalCount);
}
```

### Pattern 3: Bulk Update with SetColumns

```csharp
// FreeSql
int affrows = _ormContext.FreeSql.Update<BpmnConf>()
    .Set(a => a.AppId, appId)
    .Set(a => a.EffectiveStatus, 1)
    .Where(a => a.Id == id)
    .ExecuteAffrows();

// SqlSugar
int affrows = Db.Updateable<BpmnConf>()
    .SetColumns(a => a.AppId == appId)
    .SetColumns(a => a.EffectiveStatus == 1)
    .Where(a => a.Id == id)
    .ExecuteCommand();
```

### Pattern 4: Dynamic Where with LinqExtensions

```csharp
// FreeSql
Expression<Func<T, bool>> expr = a => a.Type == "someType";
if (condition) expr = LinqExtensions.And(expr, a => a.Status == 1);
var result = _ormContext.FreeSql.GetRepository<T>().Where(expr).ToList();

// SqlSugar (same pattern)
Expression<Func<T, bool>> expr = a => a.Type == "someType";
if (condition) expr = LinqExtensions.And(expr, a => a.Status == 1);
var result = Db.Queryable<T>().Where(expr).ToList();
```

### Pattern 5: Get by ID

```csharp
// FreeSql
var entity = _ormContext.FreeSql.Select<TEntity>(id).First();

// SqlSugar
var entity = Db.Queryable<TEntity>().InSingle(id);
```

## Pitfalls and Lessons Learned

### 1. `ISugarQueryable<T>` does NOT implement `System.Linq.IQueryable<T>`
SqlSugar 5.x uses an independent query model. `ISugarQueryable<T>` has its own `.Where()`, `.ToList()`, `.FirstOrDefault()` methods but does NOT inherit from `System.Linq.IQueryable<T>`. Therefore:
- `IBaseRepository<TEntity>.GetQueryable()` must return `ISugarQueryable<TEntity>`, NOT `IQueryable<TEntity>`
- Do NOT call `.AsQueryable()` on `ISugarQueryable<T>` — it doesn't exist and will cause compilation errors

### 2. **CRITICAL: SqlSugar does NOT use `BasePagingInfo` — use `PagingInfo` directly**

**`BasePagingInfo` is a FreeSql-specific type from `FreeSql.Internal.Model`. SqlSugar does NOT need it and does NOT have it.**

**WRONG (FreeSql pattern — do NOT use):**
```csharp
// ❌ WRONG — BasePagingInfo is FreeSql-specific, ToBasePagingInfo() extension doesn't exist in SqlSugar
BasePagingInfo basePagingInfo = pagingInfo.ToBasePagingInfo();
var list = Db.Queryable<T>().ToPageList(basePagingInfo.PageNumber, basePagingInfo.PageSize, ref totalCount);
pagingInfo.Count = totalCount;
pagingInfo.PageSize = basePagingInfo.PageSize;  // ❌ Unnecessary
pagingInfo.PageNumber = basePagingInfo.PageNumber;  // ❌ Unnecessary
```

**CORRECT (SqlSugar pattern — use PagingInfo directly):**
```csharp
// ✅ CORRECT — Use PagingInfo's PageNumber and PageSize directly
int totalCount = 0;
var list = Db.Queryable<T>()
    .ToPageList(pagingInfo.PageNumber, pagingInfo.PageSize, ref totalCount);
pagingInfo.Count = totalCount;
// ✅ No need to set PageSize or PageNumber — they're already in pagingInfo
```

**Key rules:**
- Remove ALL references to `BasePagingInfo`
- Remove ALL calls to `ToBasePagingInfo()`
- Remove ALL imports of `FreeSql.Internal.Model`
- Remove ALL imports of `AntFlowCore.Abstraction.Orm.ext` (contains the extension method)
- Use `pagingInfo.PageNumber` and `pagingInfo.PageSize` directly from `PagingInfo`
- Only need to set `pagingInfo.Count = totalCount` after `ToPageList`

### 3. Pagination count differs between FreeSql and SqlSugar
- FreeSql: `.Page(basePagingInfo)` returns the paged list, and the original code set `pagingInfo.Count = list.Count` (which is the page size, NOT the total count — this was a bug).
- SqlSugar: `.ToPageList(pageNumber, pageSize, ref totalCount)` automatically sets `totalCount` to the total number of matching records. Use `pagingInfo.Count = totalCount`.

### 4. SqlSugar transaction management is simpler
- FreeSql: Used `UnitOfWorkManager` with `Propagation` and `IsolationLevel`
- SqlSugar: Uses `ISqlSugarClient.Ado.BeginTran/CommitTran/RollbackTran` directly
- SqlSugar does NOT support propagation behavior (Required, RequiresNew, etc.)
- `TransactionalAttribute` must be updated to use SqlSugar's transaction API

### 5. All services are registered as Singleton
SqlSugar's `SqlSugarScope` is thread-safe and internally manages per-thread contexts. Unlike EF Core where DbContext must be Scoped, SqlSugar services can remain Singleton (matching the original FreeSql pattern).

### 6. Entity mapping uses `MappingTables` and `MappingColumns`
- FreeSql: `CodeFirst.ConfigEntity<T>(a => a.Name("table_name").Property(b => b.Prop).Name("column_name"))`
- SqlSugar: `db.MappingTables.Add(nameof(Entity), "table_name")` and `db.MappingColumns.Add(nameof(Entity.Prop), "column_name", nameof(Entity))`
- SqlSugar also uses `db.IgnoreColumns.Add(nameof(Entity.Prop), nameof(Entity))` for ignored properties

### 7. SqlSugar uses `ExecuteCommand()` instead of `ExecuteAffrows()`
FreeSql's `.ExecuteAffrows()` returns affected rows count. SqlSugar's equivalent is `.ExecuteCommand()` which also returns affected rows.

### 8. SqlSugar Join 语法 — 推荐链式 `InnerJoin`，清晰易读

**两表 Join**:
```csharp
// FreeSql
_ormContext.FreeSql.Select<T1, T2>()
    .InnerJoin((a, b) => a.Fk == b.Pk)
    .Where(expr)
    .ToList<T1>((a, b) => a);

// SqlSugar（推荐写法）
Db.Queryable<T1>()
    .InnerJoin<T2>((a, b) => a.Fk == b.Pk)
    .Where(expr)
    .Select((a, b) => a)
    .ToList();
```

**三表及以上 Join**（推荐从第一个表开始链式调用）:
```csharp
// FreeSql — 多参数 Select + InnerJoin
_ormContext.FreeSql.Select<T1, T2, T3>()
    .InnerJoin((a, b, c) => a.Fk1 == b.Pk)
    .InnerJoin((a, b, c) => b.Fk2 == c.Pk)
    .Where((a, b, c) => a.Id == id)
    .ToList<T1>((a, b, c) => a);

// SqlSugar — 从第一个表开始链式 InnerJoin（推荐 ✅）
Db.Queryable<T1>()
    .InnerJoin<T2>((a, b) => a.Fk1 == b.Pk)
    .InnerJoin<T3>((a, b, c) => b.Fk2 == c.Pk)
    .Where((a, b, c) => a.Id == id)
    .Select((a, b, c) => a)
    .ToList();

// 五表 Join 同理（参考 BpmnNodeConditionsConfService）:
Db.Queryable<BpmnNodeConditionsParamConf>()
    .InnerJoin<BpmnNodeConditionsConf>((a, b) => a.BpmnNodeConditionsId == b.Id)
    .InnerJoin<BpmnNode>((a, b, c) => b.BpmnNodeId == c.Id && c.NodeType == 3)
    .InnerJoin<BpmnConf>((a, b, c, d) => c.ConfId == d.Id)
    .InnerJoin<BpmBusinessProcess>((a, b, c, d, e) => e.Version == d.BpmnCode)
    .Where((a, b, c, d, e) => e.BusinessNumber == processNumber)
    .Select((a, b, c, d, e) => a.ConditionParamName)
    .ToList();
```

**要点**:
- FreeSql 用 `Select<T1, T2, ...>()` 多参数构造函数
- SqlSugar 用 `Queryable<T1>()` 单参数起始，然后链式 `.InnerJoin<T2>().InnerJoin<T3>()`
- 每增加一个表，lambda 参数自动递增（两个表用 (a,b)，三个表用 (a,b,c)，以此类推）
- SqlSugar 不需要 FreeSql 最后的投影参数 `ToList<T1>((a,b,c) => a)`，改为 `.Select((a,b,c) => a).ToList()`

### 9. File encoding must be UTF-8
When writing files, ensure the output is UTF-8 encoded. Non-UTF-8 encoding causes IDE errors when opening files.

### 10. Repository namespace typo
Many Fs repositories used `namespace AntFlowCore.Persist.repositorysitory` (typo "repositorysitory"). The new SqlSugar repositories should use `namespace AntFlowCore.Persist.repository` (correct spelling).

## DI Registration

In `src/AntFlowCore.Engine.Abstraction/conf/di/serviceregistration/ServiceRegistration.cs`:

```csharp
// Keep Singleton (SqlSugarScope is thread-safe)
services.AddSingleton<IXxxRepository, SsXxxRepository>();

// SqlSugar client is already registered via SqlSugarSetUp:
// services.AddSingleton<SqlSugarScope>(sqlSugar);
// services.AddSingleton<ISqlSugarClient>(sqlSugar);
```

## Validation Checklist

After completing each repository migration, verify:

1. No FreeSql API references (`_ormContext.FreeSql`, `IFreeSql`, `BasePagingInfo`) in the new repository file
2. No `using FreeSql.*` in the new repository file
3. Class name: `FsXxxRepository` → `SsXxxRepository` (if original has `Fs` prefix); `XxxRepository` → `XxxRepository` (if original has no prefix)
4. Constructor accepts `AntFlowOrmContext ormContext` (unchanged)
5. Namespace is `AntFlowCore.Persist.repository` (not `repositorysitory`)
6. Repository interface has no FreeSql dependencies
7. Service callers work with new implementation (no changes needed if interface is unchanged)
8. Old `FsXxxRepository.cs` file is still present (will be deleted in later phases)
9. File encoding is UTF-8
10. `GetQueryable()` returns `ISugarQueryable<TEntity>` (NOT `IQueryable<TEntity>`)

## Important Notes

1. **Always reference the SqlSugar version first**: Before writing any SqlSugar query, check `D:\data\antflow-sqlsugar-real\antflowcore\service\repository\` for existing implementations.
2. **Do NOT write SqlSugar queries from scratch** when the reference project already has working code. Copy and adjust namespaces only.
3. **Keep `AntFlowOrmContext`** — it wraps ISqlSugarClient and is still used by other parts of the system.
4. **`ISugarQueryable<T>` is NOT `IQueryable<T>`**: SqlSugar's query type is independent. Adjust interface return types accordingly.
5. **Singleton is fine**: Unlike EF Core, SqlSugar's `SqlSugarScope` is thread-safe, so repositories can remain `AddSingleton`.
6. **Pagination fix**: The original FreeSql code had a bug where `pagingInfo.Count` was set to page size instead of total count. SqlSugar's `ToPageList` with `ref totalCount` fixes this.
7. **BasePagingInfo removal**: Remove all references to `BasePagingInfo` and `ToBasePagingInfo()`. Use `PagingInfo.PageNumber` and `PagingInfo.PageSize` directly.
8. **Naming convention**: `FsXxxRepository` → `SsXxxRepository`; `XxxRepository` → `XxxRepository` (no prefix if original has none). Old Fs files must be deleted after migration.
