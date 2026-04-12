# AntFlowCore .NET 版环境搭建与运行完全指南

## 前言

AntFlow 是一款开源的流程引擎，分为 Java 版（基于 Activiti 深度定制）和 .NET 版（完全原生 .NET 开发，采用 FreeSql + Natasha 技术栈）。

本文详细介绍如何从零开始搭建 AntFlowCore .NET 版的开发环境，并成功运行项目。

## 技术栈概览

| 技术组件     | 版本要求  | 作用                         |
| ------------ | --------- | ---------------------------- |
| .NET         | .NET 10  | 运行时框架                   |
| FreeSql      | 最新版    | ORM 框架，支持多种数据库     |
| Natasha      | 最新版    | 动态编译，用于低代码条件计算 |
| ASP.NET Core | .NET 9    | Web 框架                     |
| Node.js      | 16+       | 前端构建                     |
| MySQL        | 5.7+      | 默认数据库                   |

## 环境准备

### 1. 安装 .NET 10 SDK

AntFlowCore 基于 .NET 10 开发，请先安装 .NET 10 SDK。

**下载地址：** https://dotnet.microsoft.com/download/dotnet/10.0

安装完成后验证：

```bash
dotnet --version
# 应输出类似 10.0.xxx
```

### 2. 安装 Node.js

前端项目需要 Node.js 进行构建：

**下载地址：** https://nodejs.org/

安装完成后验证：

```bash
node --version
npm --version
```

### 3. 安装 MySQL 数据库

AntFlowCore 默认使用 MySQL 数据库，版本要求 5.7 或以上。

如果你已经有 MySQL 环境可以直接使用，也可以使用 Docker 快速启动：

```bash
docker run -d \
  --name antflow-mysql \
  -p 3306:3306 \
  -e MYSQL_ROOT_PASSWORD=123456 \
  -e MYSQL_DATABASE=antflow \
  mysql:8.0
```

> antflow也支持mysql以外其它数据库,其中mysql和sql server脚本完全开源,其它脚本目前捐赠10元以上任意金额即可获取


## 获取源码

AntFlowCore 官方仓库：

- Gitee：https://gitee.com/antswarm/antflowcore
- GitHub：https://github.com/mrtylerzhou/AntFlow.net

克隆源码：

```bash
git clone https://github.com/mrtylerzhou/AntFlow.net.git
cd AntFlowCore
```

## 数据库初始化

### 1. 创建数据库

```sql
CREATE DATABASE IF NOT EXISTS antflow DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
```

### 2. 导入初始化脚本

初始化脚本位于 `antflownet/scripts/` 目录：

```bash
# 执行以下脚本：
bpm_init_db_mysql.sql
```

## 后端配置

### 1. 修改连接字符串

打开 `antflownet/appsettings.json`，修改数据库连接字符串：

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=127.0.0.1;Port=3306;Database=antflow;User=root;Password=123456;SslMode=None;AllowUserVariables=True;CharSet=utf8mb4;"
  }
}
```

根据你的实际情况修改：

- `Data Source`：数据库地址
- `Port`：端口
- `Database`：数据库名称
- `User`：用户名
- `Password`：密码

### 2. 还原 NuGet 包

在解决方案根目录执行：

```bash
dotnet restore
```

> 如果用户使用的是ide,加载或者构建的时候会自动还原

### 3. 编译项目

```bash
dotnet build
```

> 如果用户使用了ide,启动则会自动构建

如果编译过程中出现包还原问题，可以尝试清除缓存重新还原：

```bash
dotnet nuget locals all --clear
dotnet restore
```

## 前端构建

AntFlowCore 复用了 AntFlow Java 版的前端代码，请进入[java仓库](https://gitee.com/tylerzhou/Antflow) 下载后只需要antflow-vue目录下内容即为项目的前端

```bash
cd antflow.abp/antflow-vue
```

### 1. 安装依赖

```bash
npm install
# 如果网络慢，可以使用淘宝镜像：
# npm install --registry=https://registry.npmmirror.com
```

### 2. 修改后端接口地址

开发环境修改 `.env.development`：

```
VITE_APP_BASE_API=http://localhost:8001
```

生产环境修改 `.env.production`：

```
VITE_APP_BASE_API=https://your-api-domain.com
```

### 3. 构建前端

```bash
# 开发环境
npm run dev

# 生产环境构建
npm run build
```

## 运行项目

### 1. 运行后端

回到解决方案根目录：

```bash
cd ../../
dotnet run --project antflownet
```

启动成功后，你会看到类似输出：

```
Now listening on: http://[::]:8001
Application started. Press Ctrl+C to shut down.
```

### 2. 访问 Swagger

打开浏览器访问：`http://localhost:5000/swagger`

你应该能看到所有 API 接口文档：

![Swagger 文档](./images/swagger-preview.png)

### 3. 运行前端开发服务器

如果刚才没有启动前端开发服务器，现在启动：

```bash
cd antflow.abp/antflow-vue
npm run dev
```

前端开发服务器默认启动在 `http://localhost:5173`，打开浏览器访问即可。

## 默认账号密码

系统初始化默认管理员账号：

| 用户名 | 密码   | 角色       |
| ------ | ------ | ---------- |
| admin  | 123456 | 系统管理员 |

## 常见问题排查

### 1. 编译错误：找不到 .NET 10 SDK

**问题：**

```
error MSB1415: 项目文件需要 SDK "Microsoft.NET.Sdk"，但未安装该 SDK。
```

**解决：**
确认已正确安装 .NET 10 SDK，并且 PATH 环境变量已配置。重新打开终端窗口尝试。

### 2. NuGet 包还原失败

**问题：**

```
error: 无法找到包 XXXX
```

**解决：**
检查 NuGet 源配置，添加 nuget.org 源：

```bash
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
```

### 3. 启动时报数据库连接错误

**问题：**

```
MySqlConnector.MySqlException: Unable to connect to any of the specified MySQL hosts.
```

**解决：**

- 检查 MySQL 服务是否启动
- 检查 `appsettings.json` 中的连接字符串是否正确
- 检查防火墙是否允许连接
- 如果使用 Docker，检查端口映射是否正确

### 4. 前端 npm install 很慢

**解决：**
使用淘宝镜像加速：

```bash
npm config set registry https://registry.npmmirror.com
npm install
```

### 5. 前端访问后端 API 跨域错误

**问题：**
开发环境下前端运行在 5173 端口，后端运行在 5000 端口，会产生跨域。

**解决：**
项目已经默认配置了 CORS 允许任意源，如果你修改过配置，请确认 `Program.cs` 中的 CORS 配置：

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", bd => bd
        .SetIsOriginAllowed((host) => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
```

并且在中间件中正确启用：

```csharp
app.UseCors("CorsPolicy");
```

### 6. 登录提示用户名或密码错误

**问题：** 能打开登录页面，但登录失败。

**解决：**

- 确认初始化脚本已经正确执行
- 检查 `antflowcore_data.sql` 是否导入，该脚本包含默认管理员账号
- 检查数据库表 `af_user` 中是否有数据

```sql
SELECT * FROM af_user WHERE username = 'admin';
```

## 下一步

成功运行之后，你可以继续阅读：

- [如何对接自己系统的用户和角色](./如何对接自己系统的用户和角色.md)
- [定制一个自己的审批规则](./高级&问题篇/定制一个自己的审批规则.md)
- [antflow事件系统与接入模式介绍](./系统集成与扩展篇/antflow事件系统与接入模式介绍.md)

## 总结

本文介绍了 AntFlowCore .NET 版从环境准备到成功运行的完整步骤。只要按照本文步骤操作，应该可以顺利启动项目。

如果遇到其他问题，欢迎在仓库提交 Issue 反馈。

---
