# AntFlowCore + FreeSql 多数据库配置完全指南

## 前言

AntFlowCore .NET 版选择 **FreeSql** 作为 ORM 框架，最重要的原因之一就是 FreeSql 对多数据库的优秀支持。FreeSql 原生支持十几种数据库，AntFlowCore 基于 FreeSql 也天然继承了这一优势。

本文详细介绍如何将 AntFlowCore 从默认的 MySQL 切换到其他数据库。

## 支持的数据库

AntFlowCore 通过 FreeSql 支持以下数据库：

| 数据库 | 驱动包 | 是否支持 |
|--------|--------|---------|
| MySQL | FreeSql.Provider.MySqlConnector | ✅ 默认支持 |
| PostgreSQL | FreeSql.Provider.Npgsql | ✅ 完整支持 |
| SQL Server | FreeSql.Provider.SqlServer | ✅ 完整支持 |
| SQLite | FreeSql.Provider.Sqlite | ✅ 完整支持 |
| Oracle | FreeSql.Provider.Oracle | ✅ 完整支持 |
| 达梦 | FreeSql.Provider.Dameng | ✅ 完整支持 |
| 人大金仓 | FreeSql.Provider.Kingbase | ✅ 完整支持 |
| 南大通用 GBase | FreeSql.Provider.GBase | ✅ 完整支持 |
| 华为高斯 | FreeSql.Provider.OpenGauss | ✅ 完整支持 |
| OceanBase | FreeSql.Provider.OceanBase | ✅ 完整支持 |

## 修改步骤总览

切换数据库只需要三个步骤：

1. 添加对应数据库驱动 NuGet 包
2. 修改连接字符串
3. 执行对应数据库的初始化脚本
4. 启动项目

下面针对每种数据库给出详细步骤。

## 默认 MySQL 配置

默认配置已经包含 MySQL 支持，`antflowcore.csproj` 中已引用：

```xml
<PackageReference Include="FreeSql.Provider.MySqlConnector"  />
<PackageReference Include="FreeSql.Repository"  />
```

连接字符串示例：

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=127.0.0.1;Port=3306;Database=antflow;User=root;Password=123456;SslMode=None;AllowUserVariables=True;CharSet=utf8mb4;"
  }
}
```

初始化脚本位置：`antflownet/scripts/mysql/`

## PostgreSQL 配置

### 1. 添加 NuGet 包

修改 `antflowcore/antflowcore.csproj`，替换 MySQL 包为 PostgreSQL：

```xml
<!-- 注释掉或删除 MySQL -->
<!-- <PackageReference Include="FreeSql.Provider.MySqlConnector"  /> -->
<!-- 添加 PostgreSQL -->
<PackageReference Include="FreeSql.Provider.Npgsql"  />
```

然后还原：

```bash
dotnet restore
```

### 2. 修改连接字符串

修改 `appsettings.json`：

```json
{
  "ConnectionStrings": {
    "Default": "Host=127.0.0.1;Port=5432;Database=antflow;Username=postgres;Password=123456;"
  }
}
```

### 3. 初始化数据库

创建数据库：

```sql
CREATE DATABASE antflow;
```

导入初始化脚本：

PostgreSQL 脚本位于 `antflownet/scripts/postgresql/`

```bash
psql -h 127.0.0.1 -U postgres -d antflow -f antflownet/scripts/postgresql/antflowcore.sql
psql -h 127.0.0.1 -U postgres -d antflow -f antflownet/scripts/postgresql/antflowcore_data.sql
# 可选导入测试数据
psql -h 127.0.0.1 -U postgres -d antflow -f antflownet/scripts/postgresql/antflow_test_data.sql
```

### 4. 启动

```bash
dotnet run --project antflownet
```

## SQL Server 配置

### 1. 添加 NuGet 包

```xml
<!-- <PackageReference Include="FreeSql.Provider.MySqlConnector"  /> -->
<PackageReference Include="FreeSql.Provider.SqlServer"  />
```

```bash
dotnet restore
```

### 2. 修改连接字符串

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=localhost\\SQLEXPRESS;Initial Catalog=antflow;User ID=sa;Password=123456;TrustServerCertificate=True;"
  }
}
```

### 3. 初始化数据库

使用 sqlcmd 或 SSMS 执行脚本，脚本位于 `antflownet/scripts/sqlserver/`：

```bash
sqlcmd -S localhost\SQLEXPRESS -U sa -P 123456 -i antflownet/scripts/sqlserver/antflowcore.sql
sqlcmd -S localhost\SQLEXPRESS -U sa -P 123456 -i antflownet/scripts/sqlserver/antflowcore_data.sql
```

## SQLite 配置

SQLite 是文件型数据库，非常适合开发测试和小型应用。

### 1. 添加 NuGet 包

```xml
<!-- <PackageReference Include="FreeSql.Provider.MySqlConnector"  /> -->
<PackageReference Include="FreeSql.Provider.Sqlite"  />
```

```bash
dotnet restore
```

### 2. 修改连接字符串

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=antflow.db;"
  }
}
```

FreeSql 会自动创建数据库文件，不需要手动创建。

### 3. 初始化

AntFlowCore 会自动同步结构（如果表不存在），但初始数据（默认管理员、菜单等）需要执行脚本：

脚本位置：`antflownet/scripts/sqlite/antflowcore_data.sql`

使用 SQLite 工具执行即可。

## Oracle 配置

### 1. 添加 NuGet 包

```xml
<!-- <PackageReference Include="FreeSql.Provider.MySqlConnector"  /> -->
<PackageReference Include="FreeSql.Provider.Oracle"  />
```

```bash
dotnet restore
```

### 2. 修改连接字符串

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=//localhost:1521/orcl;User Id=system;Password=123456;"
  }
}
```

### 3. 初始化脚本

脚本位置：`antflownet/scripts/oracle/`

## 国产数据库支持

AntFlowCore 对国产数据库提供了良好的支持，以下是几种常见国产数据库的配置说明。

### 达梦数据库

#### 1. 添加 NuGet 包

```xml
<!-- <PackageReference Include="FreeSql.Provider.MySqlConnector"  /> -->
<PackageReference Include="FreeSql.Provider.Dameng"  />
```

#### 2. 连接字符串

```json
{
  "ConnectionStrings": {
    "Default": "Server=127.0.0.1:5236;User Id=SYSDBA;Password=SYSDBA;Database=ANTFLOW;"
  }
}
```

#### 3. 脚本位置

`antflownet/scripts/dameng/`

达梦支持三种模式：Oracle 模式、MySQL 模式、PostgreSQL 模式、SQL Server 模式，请根据你的实际模式选择对应脚本。

### 人大金仓 (Kingbase)

#### 1. 添加 NuGet 包

```xml
<!-- <PackageReference Include="FreeSql.Provider.MySqlConnector"  /> -->
<PackageReference Include="FreeSql.Provider.Kingbase"  />
```

#### 2. 连接字符串

```json
{
  "ConnectionStrings": {
    "Default": "Server=127.0.0.1;Port=54321;Database=antflow;User Id=system;Password=123456;"
  }
}
```

#### 3. 脚本位置

`antflownet/scripts/kingbase/`

同样根据你的兼容模式选择对应脚本（Oracle 模式 / MySQL 模式 / PostgreSQL 模式）。

### 南大通用 GBase

#### 1. 添加 NuGet 包

```xml
<!-- <PackageReference Include="FreeSql.Provider.MySqlConnector"  /> -->
<PackageReference Include="FreeSql.Provider.GBase"  />
```

#### 2. 连接字符串

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=127.0.0.1:9008;Database=antflow;User=gbasedbt;Password=GBasedbt123;"
  }
}
```

### 华为 OpenGuass (高斯数据库)

#### 1. 添加 NuGet 包

```xml
<!-- <PackageReference Include="FreeSql.Provider.MySqlConnector"  /> -->
<PackageReference Include="FreeSql.Provider.OpenGauss"  />
```

#### 2. 连接字符串

```json
{
  "ConnectionStrings": {
    "Default": "Host=127.0.0.1;Port=5432;Database=antflow;Username=omm;Password=123456;"
  }
}
```

### OceanBase

#### 1. 添加 NuGet 包

```xml
<!-- <PackageReference Include="FreeSql.Provider.MySqlConnector"  /> -->
<PackageReference Include="FreeSql.Provider.OceanBase"  />
```

#### 2. 连接字符串

根据你的模式（MySQL/Oracle）选择：

**MySQL 模式：**
```json
{
  "ConnectionStrings": {
    "Default": "Data Source=127.0.0.1;Port=2883;Database=antflow;User=root@sys;Password=123456;SslMode=None;"
  }
}
```

## 代码层面需要修改吗？

**不需要。**

AntFlowCore 所有 SQL 都使用 FreeSql 的 Fluent API / CodeFirst 方式编写，不依赖原生 SQL，因此切换数据库不需要修改任何业务代码。这就是使用 FreeSql 的优势。

只有两点需要注意：

1. 添加正确的数据库驱动 NuGet 包
2. 使用对应数据库的初始化脚本创建表结构和初始数据

## 源码解析：FreeSql 是如何集成的

让我们看一下 AntFlowCore 中 FreeSql 的集成代码：

`antflowcore/conf/freesql/FreeSqlExtensions.cs`:

```csharp
public static IServiceCollection FreeSqlSet(this IServiceCollection services, IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("Default");
    var dataBase = FreeSql.FreeSqlBuilder.MapperNameMySql();
    // FreeSql 自动根据连接字符串识别数据库类型
    IFreeSql fsql = new FreeSqlBuilder()
        .UseConnectionString(dataBase, connectionString)
        .UseAutoSyncStructure(false) // 关闭自动同步结构，避免意外修改
        .Build();
    services.AddSingleton<IFreeSql>(fsql);
    return services;
}
```

可以看到，集成非常简洁，FreeSql 帮我们处理了所有数据库差异。

## 常见问题

### 1. 切换数据库后需要重新建库吗？

是的，不同数据库的 SQL 脚本不兼容，需要在新数据库重新执行初始化脚本。

### 2. 可以同时使用多个数据库吗？

AntFlowCore 本身设计只需要一个数据库存储流程数据，业务数据可以使用任意数据库。如果你确实需要 AntFlowCore 本身使用多库，可以基于 FreeSql 的多库特性进行扩展。

### 3. 为什么我的数据库不在列表中？

FreeSql 本身支持更多数据库，如果你的数据库不在列表中，可以：

1. 检查 FreeSql 官方文档是否支持
2. 提交 Issue 告知我们，我们会添加对应的驱动包和初始化脚本

### 4. 连接字符串格式不对怎么办？

连接字符串格式请参考 FreeSql 官方文档：
https://freesql.net/documentation/get-started.html#连接字符串

### 5. 中文乱码问题

不同数据库默认编码不同，建议：

- MySQL：使用 `utf8mb4` 编码
- PostgreSQL：使用 `UTF8` 编码
- SQL Server：使用 `Chinese_PRC_CI_AS` 排序规则

### 6. 启动提示 "该数据库提供程序未安装"

**错误信息：**
```
FreeSql.Exceptions.FreeSqlException: 该数据库提供程序未安装
```

**解决：**
确认你已经添加了对应数据库的 `FreeSql.Provider.XXX` NuGet 包，并且 restore 成功。

## 总结

AntFlowCore 借助 FreeSql 的强大能力，轻松支持十几种数据库，切换过程非常简单，只需要修改三个地方：

1. **NuGet 包** - 添加对应数据库驱动
2. **连接字符串** - 修改为你的数据库连接信息
3. **初始化脚本** - 执行对应数据库的创建脚本

不需要修改任何业务代码，真正做到多数据库无缝切换。

如果你在使用过程中遇到问题，欢迎提交 Issue 反馈。

---

**相关链接：**
- [FreeSql 官方文档](https://freesql.net/documentation/)
- [AntFlowCore 仓库](https://gitee.com/antflow/AntFlowCore)
- [上一篇：环境搭建与运行完全指南](./AntFlowCore.NET-环境搭建与运行完全指南.md)
