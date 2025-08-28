#!/usr/bin/env pwsh
# AntFlow.NET 开发环境设置脚本
# 用于快速设置开发环境

param(
    [switch]$InstallDotNet = $false,
    [switch]$InstallDocker = $false,
    [switch]$SetupDatabase = $false,
    [switch]$RestorePackages = $true,
    [switch]$RunMigrations = $false,
    [switch]$StartServices = $false,
    [switch]$All = $false
)

# 设置错误处理
$ErrorActionPreference = "Stop"

# 获取脚本所在目录的父目录（项目根目录）
$ProjectRoot = Split-Path -Parent $PSScriptRoot
Set-Location $ProjectRoot

Write-Host "=== AntFlow.NET 开发环境设置 ===" -ForegroundColor Green
Write-Host "项目根目录: $ProjectRoot" -ForegroundColor Yellow

# 如果指定了 -All 参数，启用所有选项
if ($All) {
    $RestorePackages = $true
    $SetupDatabase = $true
    $RunMigrations = $true
    Write-Host "启用所有设置选项" -ForegroundColor Yellow
}

try {
    # 检查 .NET SDK
    Write-Host "\n[1/8] 检查 .NET SDK..." -ForegroundColor Cyan
    try {
        $dotnetVersion = dotnet --version
        Write-Host ".NET SDK 版本: $dotnetVersion" -ForegroundColor Green
        
        # 检查是否为 .NET 9.0
        if ($dotnetVersion -notlike "9.*") {
            Write-Host "警告: 建议使用 .NET 9.0 SDK" -ForegroundColor Yellow
            if ($InstallDotNet) {
                Write-Host "请手动安装 .NET 9.0 SDK: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
            }
        }
    } catch {
        Write-Host "错误: .NET SDK 未安装" -ForegroundColor Red
        if ($InstallDotNet) {
            Write-Host "请安装 .NET 9.0 SDK: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
        }
        exit 1
    }

    # 检查 Docker（可选）
    Write-Host "\n[2/8] 检查 Docker..." -ForegroundColor Cyan
    try {
        $dockerVersion = docker --version
        Write-Host "Docker 版本: $dockerVersion" -ForegroundColor Green
    } catch {
        Write-Host "Docker 未安装（可选）" -ForegroundColor Yellow
        if ($InstallDocker) {
            Write-Host "请安装 Docker Desktop: https://www.docker.com/products/docker-desktop" -ForegroundColor Yellow
        }
    }

    # 检查项目文件
    Write-Host "\n[3/8] 检查项目文件..." -ForegroundColor Cyan
    $solutionFile = Get-ChildItem -Path $ProjectRoot -Filter "*.slnx" | Select-Object -First 1
    if ($solutionFile) {
        Write-Host "解决方案文件: $($solutionFile.Name)" -ForegroundColor Green
    } else {
        Write-Host "警告: 未找到解决方案文件" -ForegroundColor Yellow
    }

    # 还原 NuGet 包
    if ($RestorePackages) {
        Write-Host "\n[4/8] 还原 NuGet 包..." -ForegroundColor Cyan
        dotnet restore
        Write-Host "NuGet 包还原完成" -ForegroundColor Green
    } else {
        Write-Host "\n[4/8] 跳过 NuGet 包还原" -ForegroundColor Cyan
    }

    # 设置数据库
    if ($SetupDatabase) {
        Write-Host "\n[5/8] 设置数据库..." -ForegroundColor Cyan
        
        # 检查是否存在数据库初始化脚本
        $dbScript = "$ProjectRoot/build/bpm_init_db_mysql.sql"
        if (Test-Path $dbScript) {
            Write-Host "找到数据库初始化脚本: $dbScript" -ForegroundColor Green
            Write-Host "请手动执行数据库脚本或配置数据库连接字符串" -ForegroundColor Yellow
        } else {
            Write-Host "未找到数据库初始化脚本" -ForegroundColor Yellow
        }
    } else {
        Write-Host "\n[5/8] 跳过数据库设置" -ForegroundColor Cyan
    }

    # 运行数据库迁移
    if ($RunMigrations) {
        Write-Host "\n[6/8] 运行数据库迁移..." -ForegroundColor Cyan
        try {
            # 尝试运行 EF Core 迁移（如果存在）
            dotnet ef database update --project src/AntFlow.Infrastructure --startup-project src/AntFlow.WebApi 2>$null
            Write-Host "数据库迁移完成" -ForegroundColor Green
        } catch {
            Write-Host "跳过数据库迁移（可能未配置 EF Core）" -ForegroundColor Yellow
        }
    } else {
        Write-Host "\n[6/8] 跳过数据库迁移" -ForegroundColor Cyan
    }

    # 构建项目
    Write-Host "\n[7/8] 构建项目..." -ForegroundColor Cyan
    dotnet build --configuration Debug --no-restore
    Write-Host "项目构建完成" -ForegroundColor Green

    # 启动服务（可选）
    if ($StartServices) {
        Write-Host "\n[8/8] 启动开发服务..." -ForegroundColor Cyan
        
        # 检查是否存在 WebApi 项目
        $webApiProject = "src/AntFlow.WebApi/AntFlow.WebApi.csproj"
        if (Test-Path $webApiProject) {
            Write-Host "启动 Web API 服务..." -ForegroundColor Yellow
            Write-Host "使用命令: dotnet run --project $webApiProject" -ForegroundColor Gray
            Write-Host "或者在 IDE 中直接运行项目" -ForegroundColor Gray
        } else {
            Write-Host "未找到 Web API 项目" -ForegroundColor Yellow
        }
    } else {
        Write-Host "\n[8/8] 跳过服务启动" -ForegroundColor Cyan
    }

    # 显示开发环境信息
    Write-Host "\n=== 开发环境设置完成 ===" -ForegroundColor Green
    Write-Host "\n开发指南:" -ForegroundColor Cyan
    Write-Host "1. 配置数据库连接字符串（appsettings.Development.json）" -ForegroundColor White
    Write-Host "2. 运行数据库初始化脚本（build/bpm_init_db_mysql.sql）" -ForegroundColor White
    Write-Host "3. 启动 API 服务: dotnet run --project src/AntFlow.WebApi" -ForegroundColor White
    Write-Host "4. 查看 API 文档: https://localhost:5001/swagger" -ForegroundColor White
    Write-Host "5. 查看项目文档: docs/ 目录" -ForegroundColor White
    
    Write-Host "\n有用的命令:" -ForegroundColor Cyan
    Write-Host "- 构建项目: .\\build\\build.ps1" -ForegroundColor White
    Write-Host "- 发布项目: .\\build\\publish.ps1" -ForegroundColor White
    Write-Host "- Docker 构建: .\\build\\docker-build.ps1" -ForegroundColor White
    Write-Host "- 运行测试: dotnet test" -ForegroundColor White

} catch {
    Write-Host "\n=== 开发环境设置失败 ===" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}

# 使用示例:
# .\build\setup-dev.ps1                    # 基本设置
# .\build\setup-dev.ps1 -All              # 完整设置
# .\build\setup-dev.ps1 -SetupDatabase    # 设置数据库
# .\build\setup-dev.ps1 -StartServices    # 启动服务