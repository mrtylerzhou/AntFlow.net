#!/usr/bin/env pwsh
# AntFlow.NET Docker 构建脚本
# 用于构建 Docker 镜像

param(
    [string]$ImageName = "antflow-net",
    [string]$Tag = "latest",
    [string]$DockerfilePath = "./Dockerfile",
    [string]$Context = ".",
    [switch]$Push = $false,
    [string]$Registry = "",
    [switch]$NoBuildCache = $false,
    [switch]$Verbose = $false
)

# 设置错误处理
$ErrorActionPreference = "Stop"

# 获取脚本所在目录的父目录（项目根目录）
$ProjectRoot = Split-Path -Parent $PSScriptRoot
Set-Location $ProjectRoot

Write-Host "=== AntFlow.NET Docker 构建脚本 ===" -ForegroundColor Green
Write-Host "项目根目录: $ProjectRoot" -ForegroundColor Yellow
Write-Host "镜像名称: $ImageName" -ForegroundColor Yellow
Write-Host "标签: $Tag" -ForegroundColor Yellow
Write-Host "Dockerfile 路径: $DockerfilePath" -ForegroundColor Yellow
Write-Host "构建上下文: $Context" -ForegroundColor Yellow

try {
    # 检查 Docker 是否可用
    Write-Host "\n[1/4] 检查 Docker 环境..." -ForegroundColor Cyan
    try {
        $dockerVersion = docker --version
        Write-Host "Docker 版本: $dockerVersion" -ForegroundColor Green
    } catch {
        Write-Host "错误: Docker 未安装或不可用" -ForegroundColor Red
        exit 1
    }

    # 检查 Dockerfile 是否存在
    if (-not (Test-Path $DockerfilePath)) {
        Write-Host "\n[2/4] 创建默认 Dockerfile..." -ForegroundColor Cyan
        $dockerfileContent = @"
# AntFlow.NET Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 复制项目文件
COPY ["Directory.Packages.props", "."]
COPY ["common.props", "."]
COPY ["version.props", "."]
COPY ["src/", "src/"]

# 还原依赖
RUN dotnet restore "src/AntFlow.WebApi/AntFlow.WebApi.csproj"

# 构建应用
WORKDIR "/src/src/AntFlow.WebApi"
RUN dotnet build "AntFlow.WebApi.csproj" -c Release -o /app/build

# 发布应用
FROM build AS publish
RUN dotnet publish "AntFlow.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 运行时镜像
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 创建非 root 用户
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "AntFlow.WebApi.dll"]
"@
        Set-Content -Path $DockerfilePath -Value $dockerfileContent -Encoding UTF8
        Write-Host "已创建默认 Dockerfile: $DockerfilePath" -ForegroundColor Green
    } else {
        Write-Host "\n[2/4] 使用现有 Dockerfile..." -ForegroundColor Cyan
        Write-Host "Dockerfile 路径: $DockerfilePath" -ForegroundColor Green
    }

    # 构建 Docker 镜像
    Write-Host "\n[3/4] 构建 Docker 镜像..." -ForegroundColor Cyan
    
    $fullImageName = if ($Registry) { "$Registry/$ImageName`:$Tag" } else { "$ImageName`:$Tag" }
    Write-Host "完整镜像名称: $fullImageName" -ForegroundColor Yellow

    $buildArgs = @(
        "build"
        "-t", $fullImageName
        "-f", $DockerfilePath
    )

    if ($NoBuildCache) {
        $buildArgs += "--no-cache"
    }

    if ($Verbose) {
        $buildArgs += "--progress=plain"
    }

    $buildArgs += $Context

    Write-Host "执行命令: docker $($buildArgs -join ' ')" -ForegroundColor Gray
    & docker @buildArgs

    # 推送镜像（如果需要）
    if ($Push) {
        Write-Host "\n[4/4] 推送 Docker 镜像..." -ForegroundColor Cyan
        if (-not $Registry) {
            Write-Host "警告: 未指定镜像仓库，跳过推送" -ForegroundColor Yellow
        } else {
            Write-Host "推送镜像: $fullImageName" -ForegroundColor Yellow
            docker push $fullImageName
        }
    } else {
        Write-Host "\n[4/4] 跳过镜像推送" -ForegroundColor Cyan
    }

    # 显示构建结果
    Write-Host "\n=== Docker 构建完成 ===" -ForegroundColor Green
    Write-Host "镜像名称: $fullImageName" -ForegroundColor Yellow
    
    # 显示镜像信息
    $imageInfo = docker images $fullImageName --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}\t{{.CreatedAt}}"
    Write-Host "\n镜像信息:" -ForegroundColor Cyan
    Write-Host $imageInfo -ForegroundColor White

} catch {
    Write-Host "\n=== Docker 构建失败 ===" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}

# 使用示例:
# .\build\docker-build.ps1                                    # 默认构建
# .\build\docker-build.ps1 -Tag v1.0.0                       # 指定标签
# .\build\docker-build.ps1 -Push -Registry myregistry.com    # 构建并推送
# .\build\docker-build.ps1 -NoBuildCache -Verbose            # 无缓存详细构建