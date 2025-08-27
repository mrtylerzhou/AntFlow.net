#!/usr/bin/env pwsh
# AntFlow.NET 构建脚本
# 用于构建整个解决方案

param(
    [string]$Configuration = "Release",
    [string]$OutputPath = "./artifacts",
    [switch]$Clean = $false,
    [switch]$Restore = $true,
    [switch]$Test = $false,
    [switch]$Pack = $false
)

# 设置错误处理
$ErrorActionPreference = "Stop"

# 获取脚本所在目录的父目录（项目根目录）
$ProjectRoot = Split-Path -Parent $PSScriptRoot
Set-Location $ProjectRoot

Write-Host "=== AntFlow.NET 构建脚本 ===" -ForegroundColor Green
Write-Host "项目根目录: $ProjectRoot" -ForegroundColor Yellow
Write-Host "配置: $Configuration" -ForegroundColor Yellow
Write-Host "输出路径: $OutputPath" -ForegroundColor Yellow

try {
    # 清理
    if ($Clean) {
        Write-Host "\n[1/5] 清理项目..." -ForegroundColor Cyan
        dotnet clean --configuration $Configuration
        if (Test-Path $OutputPath) {
            Remove-Item $OutputPath -Recurse -Force
            Write-Host "已清理输出目录: $OutputPath" -ForegroundColor Green
        }
    }

    # 还原依赖
    if ($Restore) {
        Write-Host "\n[2/5] 还原 NuGet 包..." -ForegroundColor Cyan
        dotnet restore
    }

    # 构建
    Write-Host "\n[3/5] 构建解决方案..." -ForegroundColor Cyan
    dotnet build --configuration $Configuration --no-restore

    # 运行测试
    if ($Test) {
        Write-Host "\n[4/5] 运行测试..." -ForegroundColor Cyan
        dotnet test --configuration $Configuration --no-build --verbosity normal
    }

    # 打包
    if ($Pack) {
        Write-Host "\n[5/5] 创建 NuGet 包..." -ForegroundColor Cyan
        dotnet pack --configuration $Configuration --no-build --output $OutputPath
    }

    Write-Host "\n=== 构建完成 ===" -ForegroundColor Green
    Write-Host "配置: $Configuration" -ForegroundColor Yellow
    if ($Pack) {
        Write-Host "输出路径: $OutputPath" -ForegroundColor Yellow
    }

} catch {
    Write-Host "\n=== 构建失败 ===" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}

# 使用示例:
# .\build\build.ps1                          # 默认 Release 构建
# .\build\build.ps1 -Configuration Debug     # Debug 构建
# .\build\build.ps1 -Clean -Test -Pack       # 清理、构建、测试、打包