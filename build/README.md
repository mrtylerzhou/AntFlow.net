# AntFlow.NET 构建脚本说明

本目录包含了 AntFlow.NET 项目的各种构建和部署脚本，帮助开发者快速进行项目构建、发布和部署。

## 📁 脚本文件说明

### 🔨 构建脚本

#### `build.ps1` - 项目构建脚本
用于构建整个解决方案，支持多种构建选项。

**基本用法：**
```powershell
# 默认 Release 构建
.\build\build.ps1

# Debug 构建
.\build\build.ps1 -Configuration Debug

# 清理、构建、测试、打包
.\build\build.ps1 -Clean -Test -Pack
```

**参数说明：**
- `-Configuration`: 构建配置（Release/Debug，默认：Release）
- `-OutputPath`: 输出路径（默认：./artifacts）
- `-Clean`: 构建前清理项目
- `-Restore`: 还原 NuGet 包（默认：true）
- `-Test`: 运行单元测试
- `-Pack`: 创建 NuGet 包

#### `publish.ps1` - 应用发布脚本
用于发布应用程序到不同的运行时环境。

**基本用法：**
```powershell
# 默认发布（Windows x64）
.\build\publish.ps1

# Linux x64 发布
.\build\publish.ps1 -Runtime linux-x64

# 自包含单文件发布
.\build\publish.ps1 -SelfContained -SingleFile
```

**参数说明：**
- `-Configuration`: 构建配置（默认：Release）
- `-Runtime`: 目标运行时（默认：win-x64）
- `-OutputPath`: 输出路径（默认：./publish）
- `-Project`: 项目文件路径
- `-SelfContained`: 自包含部署
- `-SingleFile`: 单文件发布
- `-Trimmed`: 启用裁剪
- `-Clean`: 清理输出目录

**支持的运行时：**
- `win-x64`: Windows 64位
- `win-x86`: Windows 32位
- `linux-x64`: Linux 64位
- `linux-arm64`: Linux ARM64
- `osx-x64`: macOS Intel
- `osx-arm64`: macOS Apple Silicon

### 🐳 Docker 脚本

#### `docker-build.ps1` - Docker 镜像构建脚本
用于构建 Docker 镜像，支持多种构建选项。

**基本用法：**
```powershell
# 默认构建
.\build\docker-build.ps1

# 指定标签和推送
.\build\docker-build.ps1 -Tag v1.0.0 -Push -Registry myregistry.com

# 无缓存详细构建
.\build\docker-build.ps1 -NoBuildCache -Verbose
```

**参数说明：**
- `-ImageName`: 镜像名称（默认：antflow-net）
- `-Tag`: 镜像标签（默认：latest）
- `-DockerfilePath`: Dockerfile 路径（默认：./Dockerfile）
- `-Context`: 构建上下文（默认：.）
- `-Push`: 推送镜像到仓库
- `-Registry`: 镜像仓库地址
- `-NoBuildCache`: 不使用构建缓存
- `-Verbose`: 详细输出

### ⚙️ 开发环境脚本

#### `setup-dev.ps1` - 开发环境设置脚本
用于快速设置开发环境，适合新开发者使用。

**基本用法：**
```powershell
# 基本设置
.\build\setup-dev.ps1

# 完整设置
.\build\setup-dev.ps1 -All

# 仅设置数据库
.\build\setup-dev.ps1 -SetupDatabase
```

**参数说明：**
- `-InstallDotNet`: 检查并提示安装 .NET SDK
- `-InstallDocker`: 检查并提示安装 Docker
- `-SetupDatabase`: 设置数据库
- `-RestorePackages`: 还原 NuGet 包（默认：true）
- `-RunMigrations`: 运行数据库迁移
- `-StartServices`: 启动开发服务
- `-All`: 启用所有设置选项

### 🛠️ 修复脚本

项目还包含多个代码修复脚本，用于项目重构和代码规范化：

- `fix-namespace-references.ps1`: 修复命名空间引用
- `fix-namespaces.ps1`: 修复命名空间
- `fix-using-statements.ps1`: 修复 using 语句
- `clean-duplicate-usings.ps1`: 清理重复的 using 语句
- `fix-di-namespace.ps1`: 修复依赖注入命名空间
- `fix-interface-namespace.ps1`: 修复接口命名空间
- `fix-lowercase-namespaces.ps1`: 修复小写命名空间

## 🚀 快速开始

### 新开发者设置

1. **克隆项目**
   ```bash
   git clone <repository-url>
   cd AntFlow.net
   ```

2. **设置开发环境**
   ```powershell
   .\build\setup-dev.ps1 -All
   ```

3. **构建项目**
   ```powershell
   .\build\build.ps1
   ```

4. **运行项目**
   ```powershell
   dotnet run --project src/AntFlow.WebApi
   ```

### 生产部署

1. **发布应用**
   ```powershell
   .\build\publish.ps1 -Runtime linux-x64 -SelfContained
   ```

2. **构建 Docker 镜像**
   ```powershell
   .\build\docker-build.ps1 -Tag v1.0.0
   ```

3. **推送到仓库**
   ```powershell
   .\build\docker-build.ps1 -Tag v1.0.0 -Push -Registry myregistry.com
   ```

## 📋 系统要求

### 开发环境
- **.NET 9.0 SDK** 或更高版本
- **PowerShell 5.1** 或 **PowerShell Core 7.0+**
- **Git** 版本控制
- **Visual Studio 2022** 或 **VS Code**（推荐）

### 可选组件
- **Docker Desktop**（用于容器化部署）
- **MySQL/PostgreSQL/SQL Server**（数据库）
- **Redis**（缓存，可选）

## 🔧 配置说明

### 数据库配置
1. 复制 `appsettings.json` 到 `appsettings.Development.json`
2. 修改数据库连接字符串
3. 运行数据库初始化脚本：`build/bpm_init_db_mysql.sql`

### Docker 配置
项目会自动创建 `Dockerfile`，如需自定义，请修改 `docker-build.ps1` 脚本。

## 🐛 故障排除

### 常见问题

1. **构建失败**
   - 检查 .NET SDK 版本
   - 运行 `dotnet restore` 还原包
   - 检查项目引用是否正确

2. **Docker 构建失败**
   - 检查 Docker 是否运行
   - 检查 Dockerfile 语法
   - 清理 Docker 缓存：`docker system prune`

3. **数据库连接失败**
   - 检查连接字符串配置
   - 确认数据库服务运行
   - 检查防火墙设置

### 获取帮助

- 查看脚本帮助：在脚本中使用 `-?` 参数
- 项目文档：查看 `docs/` 目录
- 问题反馈：提交 GitHub Issue
- QQ群：629171398

## 📝 许可证

本项目采用 Apache 2.0 许可证，详见 [LICENSE](../LICENSE) 文件。