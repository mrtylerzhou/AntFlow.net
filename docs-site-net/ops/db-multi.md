# 多数据库配置

## 概述

AntFlowCore 基于 FreeSql ORM 框架，原生支持多种关系型数据库。通过简单的配置切换，即可在不同数据库之间迁移，无需修改业务代码。

## 支持的数据库

| 数据库 | 版本要求 | 驱动包 | 状态 |
|--------|---------|--------|------|
| MySQL | 5.7+ | FreeSql.Provider.MySql | ✅ 完全支持 |
| PostgreSQL | 12+ | FreeSql.Provider.PostgreSQL | ✅ 完全支持 |
| SQL Server | 2019+ | FreeSql.Provider.SqlServer | ✅ 完全支持 |
| Oracle | 12c+ | FreeSql.Provider.Oracle | ⚠️ 理论支持 |
| SQLite | 3.x | FreeSql.Provider.Sqlite | ⚠️ 测试环境 |

## 配置方式

### 基础配置结构

```json
{
  "FreeSql": {
    "DbType": "MySQL",
    "ConnectionString": "Server=localhost;Port=3306;Database=antflowcore;Uid=root;Pwd=password;Charset=utf8mb4;",
    "IsAutoSyncStructure": true,
    "IsAutoSyncStructure": true
  }
}
```

## MySQL 配置

### 连接字符串

```json
{
  "FreeSql": {
    "DbType": "MySQL",
    "ConnectionString": "Server=localhost;Port=3306;Database=antflowcore;Uid=root;Pwd=your_password;Charset=utf8mb4;SslMode=None;",
    "IsAutoSyncStructure": true
  }
}
```

### 高级配置

```json
{
  "FreeSql": {
    "DbType": "MySQL",
    "ConnectionString": "Server=mysql.example.com;Port=3306;Database=antflowcore;Uid=app_user;Pwd=StrongP@ss;Charset=utf8mb4;SslMode=Required;Connection Timeout=30;Command Timeout=30;",
    "IsAutoSyncStructure": false,
    "IsNoneCommandParameter": true,
    "IsGenerateCommandParameterWithOwner": false
  }
}
```

### MySQL 特有参数说明

| 参数 | 说明 | 推荐值 |
|------|------|--------|
| Charset | 字符集 | utf8mb4 |
| SslMode | SSL 模式 | 内网: None / 公网: Required |
| Connection Timeout | 连接超时(秒) | 30 |
| Command Timeout | 命令超时(秒) | 30 |
| AllowUserVariables | 允许用户变量 | True（用于某些复杂查询） |

### MySQL 初始化脚本

```sql
-- 创建数据库
CREATE DATABASE IF NOT EXISTS antflowcore
    DEFAULT CHARACTER SET utf8mb4
    DEFAULT COLLATE utf8mb4_unicode_ci;

-- 创建专用用户（生产环境推荐）
CREATE USER 'antflow'@'%' IDENTIFIED BY 'StrongP@ssw0rd!';
GRANT ALL PRIVILEGES ON antflowcore.* TO 'antflow'@'%';
FLUSH PRIVILEGES;
```

## PostgreSQL 配置

### 连接字符串

```json
{
  "FreeSql": {
    "DbType": "PostgreSQL",
    "ConnectionString": "Host=localhost;Port=5432;Database=antflowcore;Username=postgres;Password=your_password;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=100;",
    "IsAutoSyncStructure": true
  }
}
```

### PostgreSQL 特有参数说明

| 参数 | 说明 | 推荐值 |
|------|------|--------|
| Host | 服务器地址 | localhost |
| Port | 端口 | 5432 |
| Pooling | 连接池 | true |
| Minimum Pool Size | 最小连接数 | 5 |
| Maximum Pool Size | 最大连接数 | 100 |
| SSL Mode | SSL模式 | Prefer |
| SearchPath | 搜索路径 | public |

### PostgreSQL 初始化

```sql
-- 创建数据库
CREATE DATABASE antflowcore
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'zh_CN.UTF-8'
    LC_CTYPE = 'zh_CN.UTF-8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- 创建 schema（可选）
CREATE SCHEMA IF NOT EXISTS antflow;
SET search_path TO antflow, public;
```

## SQL Server 配置

### 连接字符串

```json
{
  "FreeSql": {
    "DbType": "SqlServer",
    "ConnectionString": "Data Source=localhost;Initial Catalog=antflowcore;User Id=sa;Password=your_password;TrustServerCertificate=True;MultipleActiveResultSets=True;",
    "IsAutoSyncStructure": true
  }
}
```

### SQL Server 特有参数说明

| 参数 | 说明 | 推荐值 |
|------|------|--------|
| Data Source | 服务器地址 | localhost 或 IP |
| Initial Catalog | 数据库名 | antflowcore |
| TrustServerCertificate | 信任证书 | True（开发环境） |
| MultipleActiveResultSets | 多活动结果集 | True |
| Integrated Security | Windows 集成认证 | True/False |

### SQL Server 初始化

```sql
-- 创建数据库
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'antflowcore')
BEGIN
    CREATE DATABASE antflowcore;
END
GO

USE antflowcore;
GO

-- 创建登录名和用户（可选）
CREATE LOGIN antflow WITH PASSWORD = 'StrongP@ssw0rd!';
CREATE USER antflow FOR LOGIN antflow;
ALTER ROLE db_owner ADD MEMBER antflow;
GO
```

## 多数据库代码注册

### Program.cs 中的配置

```csharp
using FreeSql;

var builder = WebApplication.CreateBuilder(args);

// 方式一：通过配置文件
builder.Services.AddFreeSql(builder.Configuration);

// 方式二：手动配置（适合多库场景）
builder.Services.AddFreeSql("main", config =>
{
    var dbType = builder.Configuration["FreeSql:DbType"];
    var connStr = builder.Configuration["FreeSql:ConnectionString"];
    
    var fsql = new FreeSqlBuilder()
        .UseConnectionString(Enum.Parse<DataType>(dbType), connStr)
        .UseAutoSyncStructure(true)  // CodeFirst 自动建表
        .UseMonitorCommand(cmd => 
        {
            // SQL 监控日志
            logger.LogDebug(cmd.CommandText);
        })
        .Build();
    
    return fsql;
});
```

### FreeSql 扩展注册

```csharp
// FreeSqlServiceCollectionExtensions.cs
public static class FreeSqlServiceCollectionExtensions
{
    public static IServiceCollection AddFreeSql(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var dbType = configuration["FreeSql:DbType"] ?? "MySQL";
        var connectionString = configuration["FreeSql:ConnectionString"];
        var autoSync = bool.TryParse(
            configuration["FreeSql:IsAutoSyncStructure"], 
            out var sync) && sync;

        var freeSqlBuilder = new FreeSqlBuilder()
            .UseConnectionString(GetDataType(dbType), connectionString)
            .UseAutoSyncStructure(autoSync)
            .UseMonitorCommand(cmd =>
            {
                var sql = cmd.CommandText;
                // 记录慢查询
                if (cmd.ElapsedMilliseconds > 1000)
                {
                    Log.Warning("Slow SQL ({Elapsed}ms): {Sql}", 
                        cmd.ElapsedMilliseconds, sql);
                }
            });

        IFreeSql fsql = freeSqlBuilder.Build();
        
        services.AddSingleton(fsql);
        services.AddScoped<UnitOfWorkManager>();
        
        return services;
    }

    private static DataType GetDataType(string dbType)
    {
        return dbType.ToUpper() switch
        {
            "MYSQL" => DataType.MySql,
            "POSTGRESQL" or "PG" => DataType.PostgreSQL,
            "SQLSERVER" or "MSSQL" => DataType.SqlServer,
            "SQLITE" => DataType.Sqlite,
            "ORACLE" => DataType.Oracle,
            _ => throw new NotSupportedException(
                $"Unsupported database type: {dbType}")
        };
    }
}
```

## 读写分离配置

FreeSql 支持读写分离，适用于高并发场景：

```json
{
  "FreeSql": {
    "DbType": "MySQL",
    "ConnectionString": "Server=master.db;Port=3306;Database=antflowcore;Uid=root;Pwd=password;",
    "SlaveConnections": [
      "Server=slave1.db;Port=3306;Database=antflowcore;Uid=readonly;Pwd=password;",
      "Server=slave2.db;Port=3306;Database=antflowcore;Uid=readonly;Pwd=password;"
    ]
  }
}
```

```csharp
// 读写分离配置
var fsql = new FreeSqlBuilder()
    .UseConnectionString(DataType.MySql, masterConnectionString)
    .UseSlaveConnectionString(slaveConnectionStrings) // 从库连接
    .UseAutoSyncStructure(true)
    .Build();
```

## 数据库迁移策略

### CodeFirst 模式（推荐用于开发环境）

```json
{
  "FreeSql": {
    "IsAutoSyncStructure": true
  }
}
```

启用后，每次启动时会自动对比实体类与数据库表结构的差异，自动执行 ALTER TABLE。

### 迁移脚本模式（推荐用于生产环境）

1. 开发环境启用 CodeFirst 生成变更
2. 记录生成的 SQL 脚本
3. 在生产环境手动执行脚本

```bash
# 生成迁移脚本（示例）
dotnet run --project src/AntFlowCore.Api -- generate-sql
```

## 不同数据库的注意事项

### MySQL 注意事项

1. **字符集**：务必使用 utf8mb4，避免 emoji 存储异常
2. **排序规则**：推荐 utf8mb4_unicode_ci
3. **时间类型**：DateTime 精度默认为秒，如需毫秒使用 DateTime(3)
4. **索引长度**：单索引最大长度 3072 字节，utf8mb4 下 VARCHAR(758)

### PostgreSQL 注意事项

1. **大小写敏感**：表名和列名默认小写，引用时需加双引号
2. **JSON 类型**：支持原生 JSONB，查询效率高于 MySQL
3. **数组类型**：支持数组列，可替代某些关联表
4. **事务隔离**：默认 Read Committed

### SQL Server 注意事项

1. **Schema**：默认使用 dbo schema
2. **BIT 类型**：对应 C# bool，但数据库中存储 0/1
3. **NVARCHAR**：支持中文，避免使用 VARCHAR 存储中文
4. **时间类型**：DateTime2 精度更高，推荐使用

## 性能调优

### 连接池配置

```json
{
  "ConnectionString": "Server=localhost;Port=3306;Database=antflowcore;Uid=root;Pwd=password;Pooling=true;Min Pool Size=5;Max Pool Size=200;Connection Life Time=300;"
}
```

### 索引建议

```sql
-- 高频查询字段添加索引
CREATE INDEX idx_bpmn_conf_formcode ON bpmn_conf(form_code);
CREATE INDEX idx_task_status ON bpm_af_task(status);
CREATE INDEX idx_task_assignee ON bpm_af_task(assignee);
CREATE INDEX idx_execution_proc_instance ON bpm_af_execution(proc_instance_id);

-- 复合索引（最左前缀原则）
CREATE INDEX idx_task_assignee_status ON bpm_af_task(assignee, status);
```

## 故障排除

| 问题 | 可能原因 | 解决方案 |
|------|---------|---------|
| 表未自动创建 | 未启用 AutoSyncStructure | 检查 IsAutoSyncStructure 配置 |
| 中文乱码 | 字符集非 utf8mb4 | 修改连接字符串 Charset=utf8mb4 |
| 连接超时 | 连接池耗尽 | 增大 Max Pool Size，检查连接是否正确释放 |
| 慢查询 | 缺少索引 | 分析慢查询日志，添加适当索引 |
| 事务死锁 | 并发冲突 | 优化事务范围，减少锁持有时间 |
