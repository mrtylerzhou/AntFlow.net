---
name: "fs-to-dapper-migration"
description: "Migrates AntFlowCore repository from FreeSql to Dapper ORM. Invoke when user asks to refactor/migrate a FreeSql repository, convert FreeSql API to Dapper, or continue the ORM migration."
---

# FreeSql to Dapper Repository Migration Skill

## Overview

Migrate AntFlowCore Persist layer repository implementations from FreeSql ORM to Dapper. Each migration replaces a `FsXxxRepository` (FreeSql-based) with a `DpXxxRepository` (Dapper-based), creates a corresponding SQL static class, and updates the interface and service callers.

**Key Principle: SQL must be compatible with MySQL, PostgreSQL, SQL Server, and Oracle simultaneously.**

## Architecture Context

```
AntFlowCore.Persist.api       → Interface layer (IXxxRepository interfaces)
AntFlowCore.Persist           → Implementation layer (DpXxxRepository + XxxSql)
AntFlowCore.Persist/sql/      → SQL static classes (one per repository)
AntFlowCore.Abstraction.Orm   → ORM base layer (RepositoryBase, AntFlowOrmContext, IBaseRepository, EntityMetadata)
AntFlowCore.Base              → Entity definitions, VO, DTO, utilities
AntFlowCore.Business          → Service layer (XxxService implementations)
```

Key base types:
- `IBaseRepository<TEntity>` — ORM-agnostic repository interface (no `IQueryable`, no `Expression` for complex queries)
- `RepositoryBase<TEntity>` — Dapper repository base implementation
  - Constructor: `RepositoryBase(AntFlowOrmContext ormContext)`
  - Provides: `GetConnection()`, `GetTransaction()`, `DbType`, basic CRUD via Dapper
- `AntFlowOrmContext` — Context wrapper exposing `IDbConnectionFactory` + `DatabaseType`
  - Constructor: `AntFlowOrmContext(IDbConnectionFactory connectionFactory, DatabaseType databaseType)`
  - Properties: `DatabaseType`, `CurrentTransaction`
  - Methods: `GetConnection()`
- `EntityMetadata` — Entity-to-table/column mapping (replaces FreeSql FluentConfiguration)
- `EntityMetadataTypeMap` — Custom Dapper type map for column→property resolution (handles Oracle uppercase columns)
- `DatabaseType` enum — `MySQL`, `PostgreSQL`, `SqlServer`, `Oracle`

## Naming Convention

- **Repository class**: `Dp` prefix + entity name + `Repository` (e.g., `DpAFTaskRepository`, `DpBpmnConfRepository`)
- **Repository without Fs prefix**: If the original repository does NOT have `Fs` prefix (e.g., `DictDataRepository`), keep the same name (no `Dp` prefix)
- **SQL static class**: Entity name + `Sql` (e.g., `AFTaskSql`, `BpmnConfSql`, `DictDataSql`)
- **SQL static class location**: `src/AntFlowCore.Persist/sql/`
- **Repository interface**: No prefix, just `I` + entity name + `Repository` (unchanged)

## Multi-Database SQL Compatibility

### Core Principle

Every SQL statement must work across MySQL, PostgreSQL, SQL Server, and Oracle. Use the `DatabaseType dbType` parameter in SQL static class methods to return database-specific SQL when syntax differs.

### Database Differences Reference

| Feature | MySQL | PostgreSQL | SQL Server | Oracle |
|---------|-------|-----------|------------|--------|
| Parameter prefix | `@` | `@` | `@` | `:` (Dapper auto-converts) |
| Pagination | `LIMIT n OFFSET m` | `LIMIT n OFFSET m` | `OFFSET m ROWS FETCH NEXT n ROWS ONLY` | `OFFSET m ROWS FETCH NEXT n ROWS ONLY` |
| Top 1 | `LIMIT 1` | `LIMIT 1` | `TOP 1` | `WHERE ROWNUM <= 1` |
| Auto-increment return | `SELECT LAST_INSERT_ID()` | `RETURNING id` | `SELECT SCOPE_IDENTITY()` | `SELECT seq.CURRVAL FROM DUAL` |
| String concat | `CONCAT(a,b)` | `a \|\| b` | `CONCAT(a,b)` | `a \|\| b` |
| Boolean type | `TINYINT(1)` | native `BOOLEAN` | `BIT` | `NUMBER(1)` |
| Column name case | lowercase | lowercase | case-insensitive | UPPERCASE (unquoted) |

### Dapper Parameter Handling

Dapper automatically handles parameter prefix differences:
- In C# code, always use `@ParamName` in anonymous objects
- Dapper converts `@` to `:` for Oracle automatically
- **Do NOT use `:` in SQL strings or parameter names in C# code**

### Pagination Pattern

Use `SqlHelper.Paginate()` for all paginated queries:

```csharp
var baseSql = "SELECT * FROM table /**where**/ ORDER BY create_time DESC";
var countSql = SqlHelper.CountSql(baseSql);
pagingInfo.Count = conn.ExecuteScalar<long>(countSql, template.Parameters, GetTransaction());
var pagedSql = SqlHelper.Paginate(baseSql, pagingInfo.PageNumber, pagingInfo.PageSize, DbType);
return conn.Query<TEntity>(pagedSql, template.Parameters, GetTransaction()).ToList();
```

### Top 1 Pattern

Use `SqlHelper.TopOne()` for single-record queries:

```csharp
public static string SelectByFormCode(DatabaseType dbType) => dbType switch
{
    DatabaseType.MySQL or DatabaseType.PostgreSQL =>
        "SELECT * FROM t_bpmn_conf WHERE form_code = @FormCode AND effective_status = 1 LIMIT 1",
    DatabaseType.SqlServer =>
        "SELECT TOP 1 * FROM t_bpmn_conf WHERE form_code = @FormCode AND effective_status = 1",
    DatabaseType.Oracle =>
        "SELECT * FROM t_bpmn_conf WHERE form_code = @FormCode AND effective_status = 1 AND ROWNUM <= 1",
    _ => throw new NotSupportedException()
};
```

### INSERT with Auto-Increment

```csharp
public static string Insert(DatabaseType dbType) => dbType switch
{
    DatabaseType.MySQL =>
        "INSERT INTO table (col1, col2) VALUES (@Col1, @Col2); SELECT LAST_INSERT_ID();",
    DatabaseType.PostgreSQL =>
        "INSERT INTO table (col1, col2) VALUES (@Col1, @Col2) RETURNING id",
    DatabaseType.SqlServer =>
        "INSERT INTO table (col1, col2) VALUES (@Col1, @Col2); SELECT SCOPE_IDENTITY();",
    DatabaseType.Oracle =>
        "INSERT INTO table (col1, col2) VALUES (@Col1, @Col2); SELECT seq_name.CURRVAL FROM DUAL",
    _ => throw new NotSupportedException()
};
```

### Simple SQL (No Syntax Differences)

When SQL syntax is identical across all databases, the `dbType` parameter can be ignored:

```csharp
public static string UpdateAssignee(DatabaseType dbType)
{
    return "UPDATE bpm_af_task SET assignee = @Assignee, assignee_name = @AssigneeName WHERE id = @TaskId";
}
```

### Dynamic WHERE with Dapper.SqlBuilder

For queries with dynamic conditions, use `Dapper.SqlBuilder` with `/**where**/` placeholders:

```csharp
// SQL static class
public static string DeleteByCondition(DatabaseType dbType)
{
    return "DELETE FROM bpm_af_task /**where**/";
}

// Repository implementation
public int DeleteByCondition(string? whereClause, object? parameters)
{
    var builder = new SqlBuilder();
    var template = builder.AddTemplate(AFTaskSql.DeleteByCondition(DbType));
    if (!string.IsNullOrWhiteSpace(whereClause))
    {
        builder.Where(whereClause, parameters);
    }
    using var conn = GetConnection();
    return conn.Execute(template.RawSql, template.Parameters, GetTransaction());
}
```

**SqlBuilder placeholders:**
- `/**where**/` — replaced by `.Where()` conditions joined with AND
- `/**orderby**/` — replaced by `.OrderBy()` clauses
- `/**select**/` — replaced by `.Select()` clauses
- `/**set**/` — replaced by `.Set()` clauses (for UPDATE)
- `/**groupby**/` — replaced by `.GroupBy()` clauses
- `/**having**/` — replaced by `.Having()` clauses

**Shared conditions across templates** (e.g., count + list):

```csharp
var builder = new SqlBuilder();
var countTemplate = builder.AddTemplate("SELECT COUNT(1) FROM table /**where**/");
var listTemplate = builder.AddTemplate("SELECT * FROM table /**where**/ /**orderby**/");
builder.Where("status = @Status", new { Status = 1 });
if (condition) builder.Where("name LIKE @Name", new { Name = "%keyword%" });
builder.OrderBy("create_time DESC");
var total = conn.ExecuteScalar<int>(countTemplate.RawSql, countTemplate.Parameters);
var list = conn.Query<Entity>(listTemplate.RawSql, listTemplate.Parameters).ToList();
```

## Migration Workflow

For each repository to migrate, follow these steps in order:

### Step 1: Analyze the Fs Repository

Read the current `FsXxxRepository` file in `src/AntFlowCore.Persist/repository/`:
- Identify all custom methods beyond the base CRUD
- Identify all FreeSql API usage patterns (see API Mapping below)
- Note the constructor: `FsXxxRepository(AntFlowOrmContext ormContext) : base(ormContext)`
- Check for `BasePagingInfo` usage (FreeSql-specific type)
- Identify JOIN queries, UNION queries, and dynamic conditions

### Step 2: Ensure EntityMetadata Registration

Check `src/AntFlowCore.Abstraction.Orm/metadata/EntityMetadata.cs`:
- If the entity is not yet registered, add its mapping based on `FreesqlFluentConfiguration.cs`
- Include ALL column mappings (property name → database column name)
- Mark ignored properties with `.Ignore()`
- **Use lowercase column names** — `EntityMetadataTypeMap` handles Oracle uppercase matching automatically

### Step 3: Create the SQL Static Class

Create `src/AntFlowCore.Persist/sql/XxxSql.cs`:

```csharp
using AntFlowCore.Abstraction.Orm.enums;

namespace AntFlowCore.Persist.sql;

public static class XxxSql
{
    // Simple SQL (identical across databases)
    public static string UpdateSomeField(DatabaseType dbType)
    {
        return "UPDATE table SET field = @Value WHERE id = @Id";
    }

    // SQL with database-specific syntax
    public static string SelectTopOne(DatabaseType dbType) => dbType switch
    {
        DatabaseType.MySQL or DatabaseType.PostgreSQL =>
            "SELECT * FROM table WHERE id = @Id LIMIT 1",
        DatabaseType.SqlServer =>
            "SELECT TOP 1 * FROM table WHERE id = @Id",
        DatabaseType.Oracle =>
            "SELECT * FROM table WHERE id = @Id AND ROWNUM <= 1",
        _ => throw new NotSupportedException()
    };

    // Dynamic WHERE using SqlBuilder placeholder
    public static string DeleteByCondition(DatabaseType dbType)
    {
        return "DELETE FROM table /**where**/";
    }

    // JOIN query
    public static string FindByProcessNumber(DatabaseType dbType)
    {
        return @"
            SELECT a.* FROM table_a a
            INNER JOIN table_b b ON a.fk_id = b.id
            WHERE b.business_number = @ProcessNumber";
    }

    // Paginated query with SqlBuilder
    public static string QueryPageList(DatabaseType dbType)
    {
        return "SELECT * FROM table /**where**/ ORDER BY create_time DESC";
    }
}
```

### Step 4: Create the Dp Repository Implementation

Create `src/AntFlowCore.Persist/repository/DpXxxRepository.cs`:

**Before (FreeSql):**
```csharp
public class FsXxxRepository : RepositoryBase<XxxEntity>, IXxxRepository
{
    public FsXxxRepository(AntFlowOrmContext ormContext) : base(ormContext) { }

    public void DeleteByExpression(Expression<Func<XxxEntity, bool>> predicate)
    {
        _ormContext.FreeSql.Delete<XxxEntity>().Where(predicate).ExecuteAffrows();
    }

    public List<XxxEntity> FindByProcessNumber(string processNumber)
    {
        return _ormContext.FreeSql.Select<XxxEntity, BpmBusinessProcess>()
            .InnerJoin((a, b) => a.ProcInstId == b.ProcInstId)
            .Where((a, b) => b.BusinessNumber == processNumber)
            .ToList();
    }
}
```

**After (Dapper):**
```csharp
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Persist.sql;
using Dapper;

namespace AntFlowCore.Persist.repository;

public class DpXxxRepository : RepositoryBase<XxxEntity>, IXxxRepository
{
    public DpXxxRepository(AntFlowOrmContext ormContext) : base(ormContext) { }

    public int DeleteByCondition(string? whereClause, object? parameters)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(XxxSql.DeleteByCondition(DbType));
        if (!string.IsNullOrWhiteSpace(whereClause))
            builder.Where(whereClause, parameters);
        using var conn = GetConnection();
        return conn.Execute(template.RawSql, template.Parameters, GetTransaction());
    }

    public List<XxxEntity> FindByProcessNumber(string processNumber)
    {
        using var conn = GetConnection();
        return conn.Query<XxxEntity>(XxxSql.FindByProcessNumber(DbType),
            new { ProcessNumber = processNumber }, GetTransaction()).ToList();
    }
}
```

Key changes:
- Class name: `FsXxxRepository` → `DpXxxRepository`
- Constructor: unchanged (`AntFlowOrmContext ormContext`)
- Namespace: `AntFlowCore.Persist.repositorysitory` → `AntFlowCore.Persist.repository`
- Remove all `using FreeSql.*` and `using AntFlowCore.Abstraction.Orm.ext`
- Add `using Dapper;` and `using AntFlowCore.Persist.sql;`
- Data access: `_ormContext.FreeSql.xxx` → `GetConnection()` + Dapper extension methods
- `Expression<Func<T, bool>>` → `string? whereClause, object? parameters` (for dynamic conditions)
- Pagination: `.Page(basePagingInfo)` → `SqlHelper.Paginate()` + `SqlHelper.CountSql()`

### Step 5: Update Repository Interface

Modify `IXxxRepository` in `src/AntFlowCore.Persist.api/interf/repository/`:

- Remove `using FreeSql.Internal.Model;`
- Remove `BasePagingInfo` parameters
- Replace `Expression<Func<T, bool>>` with `string? whereClause, object? parameters` for dynamic conditions
- Replace `Expression<Func<T1, T2, bool>>` (dual-parameter) with specific method signatures

**Before:**
```csharp
public interface IXxxRepository : IBaseRepository<XxxEntity>
{
    void DeleteByExpression(Expression<Func<XxxEntity, bool>> predicate);
    List<XxxEntity> QueryByExpression(Expression<Func<XxxEntity, BpmnConf, bool>> expression, PagingInfo pagingInfo);
}
```

**After:**
```csharp
public interface IXxxRepository : IBaseRepository<XxxEntity>
{
    int DeleteByCondition(string? whereClause, object? parameters);
    List<XxxEntity> QueryWithBpmnConf(string? whereClause, object? parameters, PagingInfo pagingInfo);
}
```

### Step 6: Update Service Callers

Search for all callers of the modified repository methods and update them:

**Before (FreeSql Expression):**
```csharp
_repository.DeleteByExpression(a => a.Id == taskId);
_repository.DeleteByExpression(a => a.ProcInstId == processInstanceId);
_repository.DeleteByExpression(a => otherNewTaskIds.Contains(a.Id));
```

**After (Dapper SQL condition):**
```csharp
_repository.DeleteByCondition("id = @Id", new { Id = taskId });
_repository.DeleteByCondition("proc_inst_id = @ProcInstId", new { ProcInstId = processInstanceId });
_repository.DeleteByCondition("id IN @Ids", new { Ids = otherNewTaskIds });
```

**Before (FreeSql dual-expression for JOIN):**
```csharp
Expression<Func<DictData, BpmnConf, bool>> expression = (a, b) => a.DictType == "lowcodeflow";
if (taskMgmtVO.ProcessState != null)
    expression = LinqExtensions.And(expression, (a, b) => b.EffectiveStatus == taskMgmtVO.ProcessState);
var result = _repository.QueryByExpression(expression, pagingInfo);
```

**After (Dapper SQL condition string):**
```csharp
var parameters = new DynamicParameters();
var conditions = new List<string> { "a.dict_type = @DictType" };
parameters.Add("@DictType", "lowcodeflow");
if (taskMgmtVO.ProcessState != null && taskMgmtVO.ProcessState > 0)
{
    conditions.Add("b.effective_status = @ProcessState");
    parameters.Add("@ProcessState", taskMgmtVO.ProcessState);
}
var whereClause = string.Join(" AND ", conditions);
var result = _repository.QueryWithBpmnConf(whereClause, parameters, pagingInfo);
```

### Step 7: Update DI Registration

In `src/AntFlowCore.Engine.Abstraction/conf/di/serviceregistration/ServiceRegistration.cs`:

```csharp
// Before
services.AddSingleton<IXxxRepository, FsXxxRepository>();
// After
services.AddScoped<IXxxRepository, DpXxxRepository>();
```

**IMPORTANT**: Dapper repositories MUST be `AddScoped` (not `AddSingleton`) because `IDbConnection` is not thread-safe.

### Step 8: Delete Old Fs Repository File

Delete the old `FsXxxRepository.cs` file from `src/AntFlowCore.Persist/repository/`.

## FreeSql to Dapper API Mapping

### Basic CRUD

| FreeSql | Dapper |
|---------|--------|
| `_ormContext.FreeSql.GetRepository<T>().Where(expr).ToList()` | `conn.Query<T>(sql, parameters, GetTransaction()).ToList()` |
| `_ormContext.FreeSql.Select<T>(id).First()` | `conn.QueryFirstOrDefault<T>(sql, new { Id = id }, GetTransaction())` |
| `_ormContext.FreeSql.GetRepository<T>().Insert(entity)` | `conn.Execute(SqlHelper.BuildInsertSql(entity))` or custom INSERT SQL |
| `_ormContext.FreeSql.GetRepository<T>().Update(entity)` | `conn.Execute(SqlHelper.BuildUpdateSql(entity))` or custom UPDATE SQL |
| `_ormContext.FreeSql.GetRepository<T>().Delete(entity)` | `conn.Execute(sql, new { Id = idValue }, GetTransaction())` |
| `_ormContext.FreeSql.Select<T>().Where(expr).ToOne()` | `conn.QueryFirstOrDefault<T>(sql, parameters, GetTransaction())` |
| `_ormContext.FreeSql.Select<T>().Where(expr).Count()` | `conn.ExecuteScalar<int>(countSql, parameters, GetTransaction())` |
| `_ormContext.FreeSql.Select<T>().Max(a => a.Code)` | `conn.ExecuteScalar<string>("SELECT MAX(code) FROM table", transaction: GetTransaction())` |

### Pagination

| FreeSql | Dapper |
|---------|--------|
| `.Page(basePagingInfo)` | `SqlHelper.Paginate(sql, pageNumber, pageSize, dbType)` |
| `pagingInfo.Count = list.Count` (BUG) | `pagingInfo.Count = conn.ExecuteScalar<long>(SqlHelper.CountSql(sql), parameters)` |

### Join Queries

| FreeSql | Dapper |
|---------|--------|
| `_ormContext.FreeSql.Select<T1, T2>().InnerJoin((a, b) => a.Fk == b.Pk).Where(expr).ToList()` | Write JOIN SQL in XxxSql static class, use `conn.Query<T1>(sql, parameters)` |
| `_ormContext.FreeSql.Select<T1, T2>().LeftJoin(...)` | Write LEFT JOIN SQL in XxxSql static class |
| Dual-parameter `Expression<Func<T1, T2, bool>>` | Replace with `string? whereClause, object? parameters` |

### Update Operations

| FreeSql | Dapper |
|---------|--------|
| `_ormContext.FreeSql.Update<T>().SetSource(entity).ExecuteAffrows()` | `conn.Execute("UPDATE table SET col1=@Col1, col2=@Col2 WHERE id=@Id", entity, GetTransaction())` |
| `_ormContext.FreeSql.Update<T>().Where(expr).Set(a => new T { Prop = value }).ExecuteAffrows()` | `conn.Execute("UPDATE table SET prop = @Value WHERE ...", new { Value = value }, GetTransaction())` |

### Delete Operations

| FreeSql | Dapper |
|---------|--------|
| `_ormContext.FreeSql.Delete<T>().Where(expr).ExecuteAffrows()` | `conn.Execute("DELETE FROM table /**where**/", parameters, GetTransaction())` with SqlBuilder |
| `_ormContext.FreeSql.Delete<T>(entity).ExecuteAffrows()` | `conn.Execute("DELETE FROM table WHERE id = @Id", new { Id = id }, GetTransaction())` |

### Conditional Query Building

| FreeSql | Dapper |
|---------|--------|
| `LinqExtensions.True<T>().And(cond1).And(cond2)` | `List<string> conditions; DynamicParameters parameters; string.Join(" AND ", conditions)` |
| `expression.WhereIf(condition, expr)` | `if (condition) conditions.Add("col = @Param"); parameters.Add("@Param", value);` |
| `Expression<Func<T, bool>> predicate` | `string whereClause + object parameters` |

## Common Patterns

### Pattern 1: Simple Paginated Query

```csharp
// SQL static class
public static string QueryPageList(DatabaseType dbType)
{
    return "SELECT * FROM t_dict_data /**where**/ ORDER BY create_time DESC";
}

// Repository
public List<DictData> QueryDictDataListByExpression(string? whereClause, object? parameters, PagingInfo pagingInfo)
{
    var builder = new SqlBuilder();
    var template = builder.AddTemplate(DictDataSql.QueryPageList(DbType));
    if (!string.IsNullOrWhiteSpace(whereClause))
        builder.Where(whereClause, parameters);

    var countSql = SqlHelper.CountSql(template.RawSql);
    using var conn = GetConnection();
    pagingInfo.Count = conn.ExecuteScalar<long>(countSql, template.Parameters, GetTransaction());

    var pagedSql = SqlHelper.Paginate(template.RawSql, pagingInfo.PageNumber, pagingInfo.PageSize, DbType);
    return conn.Query<DictData>(pagedSql, template.Parameters, GetTransaction()).ToList();
}
```

### Pattern 2: JOIN Query with Pagination

```csharp
// SQL static class
public static string QueryPageListWithBpmnConf(DatabaseType dbType)
{
    return @"
        SELECT a.* FROM t_dict_data a
        INNER JOIN t_bpmn_conf b ON a.dict_value = b.form_code AND b.is_lowcode_flow = 1
        /**where**/
        ORDER BY a.create_time DESC";
}

// Service caller builds conditions
var parameters = new DynamicParameters();
var conditions = new List<string> { "a.dict_type = @DictType" };
parameters.Add("@DictType", "lowcodeflow");
if (processState > 0)
{
    conditions.Add("b.effective_status = @ProcessState");
    parameters.Add("@ProcessState", processState);
}
var whereClause = string.Join(" AND ", conditions);
var result = _repository.QueryDictDataListWithBpmnConf(whereClause, parameters, pagingInfo);
```

### Pattern 3: Dynamic Delete

```csharp
// SQL static class
public static string DeleteByCondition(DatabaseType dbType)
{
    return "DELETE FROM bpm_af_task /**where**/";
}

// Repository
public int DeleteByCondition(string? whereClause, object? parameters)
{
    var builder = new SqlBuilder();
    var template = builder.AddTemplate(AFTaskSql.DeleteByCondition(DbType));
    if (!string.IsNullOrWhiteSpace(whereClause))
        builder.Where(whereClause, parameters);
    using var conn = GetConnection();
    return conn.Execute(template.RawSql, template.Parameters, GetTransaction());
}

// Service callers
_repository.DeleteByCondition("id = @Id", new { Id = taskId });
_repository.DeleteByCondition("proc_inst_id = @ProcInstId", new { ProcInstId = processInstanceId });
_repository.DeleteByCondition("id IN @Ids", new { Ids = idList });
_repository.DeleteByCondition("task_def_key = @TaskDefKey AND proc_inst_id = @ProcInstId", new { TaskDefKey = key, ProcInstId = procInstId });
```

### Pattern 4: Partial Field Update

```csharp
// SQL static class
public static string UpdateAssignee(DatabaseType dbType)
{
    return "UPDATE bpm_af_task SET assignee = @Assignee, assignee_name = @AssigneeName WHERE id = @TaskId";
}

// Repository
public int UpdateAssignee(string taskId, string assignee, string assigneeName)
{
    using var conn = GetConnection();
    return conn.Execute(AFTaskSql.UpdateAssignee(DbType),
        new { TaskId = taskId, Assignee = assignee, AssigneeName = assigneeName }, GetTransaction());
}
```

### Pattern 5: UNION ALL Query

```csharp
// SQL static class
public static string GetNodeIdsByElementId(DatabaseType dbType)
{
    return @"
        SELECT b.node_id FROM t_bpm_variable a
        INNER JOIN t_bpm_variable_single b ON a.id = b.variable_id
        WHERE a.process_num = @ProcessNumber AND b.element_id = @ElementId AND a.is_del = 0
        UNION ALL
        SELECT b.node_id FROM t_bpm_variable a
        INNER JOIN t_bpm_variable_multiplayer b ON a.id = b.variable_id
        WHERE a.process_num = @ProcessNumber AND b.element_id = @ElementId AND a.is_del = 0";
}
```

### Pattern 6: Multi-table JOIN with Projection

```csharp
// SQL static class — project to VO columns directly
public static string SelectPageList(DatabaseType dbType)
{
    return @"
        SELECT a.id, a.bpmn_code, a.form_code, c.dict_label AS form_code_display_name,
               a.deduplication_type, a.effective_status, a.business_party_id,
               a.update_time, a.is_out_side_process, a.is_lowcode_flow, a.remark
        FROM t_bpmn_conf a
        LEFT JOIN t_out_side_bpm_business_party b ON a.business_party_id = b.id
        LEFT JOIN t_dict_data c ON a.form_code = c.dict_value AND a.is_lowcode_flow = 1
        WHERE a.is_del = 0
        /**where**/
        ORDER BY a.create_time DESC";
}

// Repository — map to VO type
public List<BpmnConfVo> SelectPageList(PagingInfo pagingInfo, BpmnConfVo vo)
{
    var builder = new SqlBuilder();
    var template = builder.AddTemplate(BpmnConfSql.SelectPageList(DbType));
    // Build dynamic conditions from vo properties
    if (vo.EffectiveStatus > 0)
        builder.Where("a.effective_status = @EffectiveStatus", new { vo.EffectiveStatus });
    if (!string.IsNullOrEmpty(vo.Search))
        builder.Where("(a.bpmn_name LIKE @Search OR a.form_code LIKE @Search OR a.bpmn_code LIKE @Search)",
            new { Search = "%" + vo.Search + "%" });
    // ... more conditions

    var countSql = SqlHelper.CountSql(template.RawSql);
    using var conn = GetConnection();
    pagingInfo.Count = conn.ExecuteScalar<long>(countSql, template.Parameters, GetTransaction());
    var pagedSql = SqlHelper.Paginate(template.RawSql, pagingInfo.PageNumber, pagingInfo.PageSize, DbType);
    return conn.Query<BpmnConfVo>(pagedSql, template.Parameters, GetTransaction()).ToList();
}
```

### Pattern 7: Multi-step Business Logic (EffectiveBpmnConf pattern)

When FreeSql code performs multiple DB operations in sequence (query → conditional update → update), convert each step to a separate Dapper call with its own SQL:

```csharp
// SQL static class — separate SQL for each step
public static string SelectById(DatabaseType dbType) => "SELECT * FROM t_bpmn_conf WHERE id = @Id";
public static string SelectEffectiveByFormCode(DatabaseType dbType) => dbType switch { /* Top 1 per db */ };
public static string UpdateEffectiveStatusToZero(DatabaseType dbType) => "UPDATE t_bpmn_conf SET effective_status = 0 WHERE id = @Id";
public static string EffectiveBpmnConf(DatabaseType dbType) => "UPDATE t_bpmn_conf SET app_id = @AppId, bpmn_type = @BpmnType, is_all = @IsAll, effective_status = 1 WHERE id = @Id";

// Repository — multi-step logic
public void EffectiveBpmnConf(int id)
{
    using var conn = GetConnection();
    BpmnConf bpmnConf = conn.QueryFirstOrDefault<BpmnConf>(BpmnConfSql.SelectById(DbType), new { Id = id }, GetTransaction());
    if (bpmnConf == null) throw new Exception($"Bpmn conf with id {id} not found");

    BpmnConf alreadyEffectiveConf = conn.QueryFirstOrDefault<BpmnConf>(
        BpmnConfSql.SelectEffectiveByFormCode(DbType), new { FormCode = bpmnConf.FormCode }, GetTransaction());

    if (alreadyEffectiveConf != null)
    {
        conn.Execute(BpmnConfSql.UpdateEffectiveStatusToZero(DbType), new { Id = alreadyEffectiveConf.Id }, GetTransaction());
        // merge values from alreadyEffectiveConf and bpmnConf
    }

    conn.Execute(BpmnConfSql.EffectiveBpmnConf(DbType), new { Id = id, AppId = appId, BpmnType = bpmnType, IsAll = isAll }, GetTransaction());
}
```

## Column Name Reference

When writing SQL, use the **database column names** (not C# property names). Refer to `EntityMetadata` for mappings.

Common column name patterns:
- `Id` → `id`
- `BpmnCode` → `bpmn_code`
- `EffectiveStatus` → `effective_status`
- `IsDel` → `is_del`
- `CreateTime` → `create_time`
- `Assignee` → `assignee`
- `ProcInstId` → `proc_inst_id` (in most tables) or `PROC_INST_ID_` (in BpmBusinessProcess)

**Always check EntityMetadata for exact column names** — some have non-standard mappings like `Sort` → `dict_sort`, `Label` → `dict_label`, `Value` → `dict_value`.

## DI Registration

In `src/AntFlowCore.Engine.Abstraction/conf/di/serviceregistration/ServiceRegistration.cs`:

```csharp
// ALL Dapper repositories must be AddScoped (not AddSingleton)
services.AddScoped<IXxxRepository, DpXxxRepository>();
```

**Why Scoped**: `IDbConnection` is not thread-safe. Unlike FreeSql's `IFreeSql` (Singleton-safe) or SqlSugar's `SqlSugarScope` (thread-safe), Dapper connections must be created per-request.

## Transaction Management

`TransactionalAttribute` uses `IDbConnection` + `IDbTransaction` via Rougamo AOP:

```csharp
[Transactional]
public void SomeMethod() { ... }
```

Inside repository methods, use `GetTransaction()` to participate in the current transaction:

```csharp
using var conn = GetConnection();
return conn.Execute(sql, parameters, GetTransaction());
```

**Important**: When `GetTransaction()` returns non-null, the connection is already opened by the transaction attribute. When it returns null, `GetConnection()` opens a new connection that is disposed via `using`.

## Pitfalls and Lessons Learned

### 1. Oracle column names are UPPERCASE
Oracle returns column names in uppercase when not double-quoted. `EntityMetadataTypeMap` handles this via case-insensitive matching. **Always register column names in lowercase in EntityMetadata** — the type map will match regardless of database.

### 2. Dapper parameter prefix
Always use `@ParamName` in C# code. Dapper auto-converts to `:ParamName` for Oracle. **Never use `:` in SQL strings.**

### 3. Pagination syntax differs significantly
Always use `SqlHelper.Paginate()` for pagination. Never write `LIMIT/OFFSET` directly in SQL static classes for paginated queries — the SqlHelper generates the correct syntax per database.

### 4. `DeleteByExpression` → `DeleteByCondition`
FreeSql's `Expression<Func<T, bool>>` for deletes must be converted to SQL WHERE clause strings. Common patterns:
- `a => a.Id == taskId` → `"id = @Id", new { Id = taskId }`
- `a => a.ProcInstId == procInstId` → `"proc_inst_id = @ProcInstId", new { ProcInstId = procInstId }`
- `a => ids.Contains(a.Id)` → `"id IN @Ids", new { Ids = ids }`
- `a => a.Key == key && a.ProcInstId == procInstId` → `"task_def_key = @TaskDefKey AND proc_inst_id = @ProcInstId", new { TaskDefKey = key, ProcInstId = procInstId }`

### 5. Dual-parameter Expressions don't work in Dapper
FreeSql's `Expression<Func<T1, T2, bool>>` for JOIN queries cannot be used with Dapper. Replace with:
- Specific method signatures accepting `string? whereClause, object? parameters`
- Service callers build SQL conditions manually using `DynamicParameters` and `List<string>`

### 6. BasePagingInfo is FreeSql-specific
Remove all references to `BasePagingInfo` and `ToBasePagingInfo()`. Use `PagingInfo.PageNumber` and `PagingInfo.PageSize` directly.

### 7. Pagination count fix
The original FreeSql code had a bug: `pagingInfo.Count = list.Count` (which is page size, not total count). Dapper version correctly uses `conn.ExecuteScalar<long>(SqlHelper.CountSql(sql))` to get total count.

### 8. DI lifetime must be Scoped
All Dapper repositories MUST be `AddScoped`. FreeSql used `AddSingleton` because `IFreeSql` is thread-safe, but `IDbConnection` is not.

### 9. Namespace typo fix
Many Fs repositories used `namespace AntFlowCore.Persist.repositorysitory` (typo). New Dapper repositories use `namespace AntFlowCore.Persist.repository` (correct).

### 10. Always delete old Fs files after migration
After creating the Dp repository and updating all references, delete the old `FsXxxRepository.cs` file.

### 11. SQL keywords in column names
Some columns use Oracle-incompatible names. For example, `VERSION` is a reserved word in some databases. When in doubt, use column aliases or quote identifiers.

### 12. Connection management in RepositoryBase
`GetConnection()` opens the connection if not already open. Always use `using var conn = GetConnection()` in repository methods. When inside a transaction (`GetTransaction()` returns non-null), the connection is shared and NOT disposed by `using` (the transaction attribute manages its lifecycle).

### 13. `Page<T>` → `PagingInfo` conversion chain
When a repository method uses `Page<T>` as parameter (e.g., `SelectPageList(Page<BpmnConfVo> page, BpmnConfVo vo)`), it must be changed to `PagingInfo`. This change propagates through the entire interface chain:
- `IBpmnConfRepository.SelectPageList(Page<BpmnConfVo>, BpmnConfVo)` → `SelectPageList(PagingInfo, BpmnConfVo)`
- `IBpmnConfService.SelectPageList(Page<BpmnConfVo>, BpmnConfVo)` → `SelectPageList(PagingInfo, BpmnConfVo)`
- `BpmnConfService.SelectPageList(Page<BpmnConfVo>, BpmnConfVo)` → `SelectPageList(PagingInfo, BpmnConfVo)`
- `BpmnConfBizService.SelectPage(pageDto, vo)` — caller must convert: `page.ToPagingInfo()` → call repo → `page.Of(results, pagingInfo)`

The BizService conversion pattern:
```csharp
Page<BpmnConfVo> page = PageUtils.GetPageByPageDto<BpmnConfVo>(pageDto);
PagingInfo pagingInfo = page.ToPagingInfo();
List<BpmnConfVo> results = _service.SelectPageList(pagingInfo, vo);
page.Of(results, pagingInfo); // writes back Total from pagingInfo.Count
```

### 14. Verify entity property types before writing Dapper parameters
FreeSql's `.Set(a => a.AppId, value)` doesn't expose the actual C# type at the call site. When converting to Dapper SQL parameters, always check the entity class for the exact property type. For example, `BpmnConf.AppId` is `int?` (not `string?`), and `BpmnConf.BpmnType` is `int?` (not `string?`). Declaring the wrong type in local variables will cause compilation errors.

## Validation Checklist

After completing each repository migration, verify:

1. No FreeSql API references (`_ormContext.FreeSql`, `IFreeSql`, `BasePagingInfo`) in the new repository file
2. No `using FreeSql.*` in the new repository file
3. Class name uses `Dp` prefix (or same name if original had no `Fs` prefix)
4. Constructor accepts `AntFlowOrmContext ormContext` (unchanged)
5. Namespace is `AntFlowCore.Persist.repository` (not `repositorysitory`)
6. Repository interface has no FreeSql dependencies
7. Service callers updated to use new method signatures
8. DI registration uses `AddScoped` and references `DpXxxRepository` class name
9. Old `FsXxxRepository.cs` file deleted
10. SQL static class created in `src/AntFlowCore.Persist/sql/`
11. SQL statements are compatible with all 4 databases (MySQL, PostgreSQL, SQL Server, Oracle)
12. EntityMetadata has the entity registered with correct column mappings
13. File encoding is UTF-8
