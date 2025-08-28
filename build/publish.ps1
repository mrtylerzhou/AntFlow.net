#!/usr/bin/env pwsh
# AntFlow.NET 发布脚本
# 用于发布应用程序到不同环境

param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$OutputPath = "./publish",
    [string]$Project = "src/AntFlow.WebApi/AntFlow.WebApi.csproj",
    [switch]$SelfContained = $false,
    [switch]$SingleFile = $false,
    [switch]$Trimmed = $false,
    [switch]$Clean = $false
)

# 设置错误处理
$ErrorActionPreference = "Stop"

# 获取脚本所在目录的父目录（项目根目录）
$ProjectRoot = Split-Path -Parent $PSScriptRoot
Set-Location $ProjectRoot

Write-Host "=== AntFlow.NET 发布脚本 ===" -ForegroundColor Green
Write-Host "项目根目录: $ProjectRoot" -ForegroundColor Yellow
Write-Host "配置: $Configuration" -ForegroundColor Yellow
Write-Host "运行时: $Runtime" -ForegroundColor Yellow
Write-Host "输出路径: $OutputPath" -ForegroundColor Yellow
Write-Host "项目文件: $Project" -ForegroundColor Yellow
Write-Host "自包含: $SelfContained" -ForegroundColor Yellow
Write-Host "单文件: $SingleFile" -ForegroundColor Yellow
Write-Host "裁剪: $Trimmed" -ForegroundColor Yellow

try {
    # 检查项目文件是否存在
    if (-not (Test-Path $Project)) {
        Write-Host "错误: 找不到项目文件 $Project" -ForegroundColor Red
        Write-Host "请检查项目路径是否正确" -ForegroundColor Red
        exit 1
    }

    # 清理输出目录
    if ($Clean -and (Test-Path $OutputPath)) {
        Write-Host "\n[1/4] 清理输出目录..." -ForegroundColor Cyan
        Remove-Item $OutputPath -Recurse -Force
        Write-Host "已清理输出目录: $OutputPath" -ForegroundColor Green
    }

    # 还原依赖
    Write-Host "\n[2/4] 还原 NuGet 包..." -ForegroundColor Cyan
    dotnet restore $Project

    # 构建发布参数
    $publishArgs = @(
        "publish"
        $Project
        "--configuration", $Configuration
        "--runtime", $Runtime
        "--output", $OutputPath
        "--no-restore"
    )

    if ($SelfContained) {
        $publishArgs += "--self-contained", "true"
    } else {
        $publishArgs += "--self-contained", "false"
    }

    if ($SingleFile) {
        $publishArgs += "--property:PublishSingleFile=true"
    }

    if ($Trimmed) {
        $publishArgs += "--property:PublishTrimmed=true"
    }

    # 发布应用程序
    Write-Host "\n[3/4] 发布应用程序..." -ForegroundColor Cyan
    Write-Host "执行命令: dotnet $($publishArgs -join ' ')" -ForegroundColor Gray
    & dotnet @publishArgs

    # 显示发布结果
    Write-Host "\n[4/4] 发布完成" -ForegroundColor Cyan
    $publishedFiles = Get-ChildItem $OutputPath -Recurse | Measure-Object
    Write-Host "发布文件数量: $($publishedFiles.Count)" -ForegroundColor Green
    
    $outputSize = (Get-ChildItem $OutputPath -Recurse | Measure-Object -Property Length -Sum).Sum
    $outputSizeMB = [math]::Round($outputSize / 1MB, 2)
    Write-Host "发布包大小: $outputSizeMB MB" -ForegroundColor Green

    Write-Host "\n=== 发布成功 ===" -ForegroundColor Green
    Write-Host "发布路径: $OutputPath" -ForegroundColor Yellow
    Write-Host "运行时: $Runtime" -ForegroundColor Yellow
    Write-Host "配置: $Configuration" -ForegroundColor Yellow

} catch {
    Write-Host "\n=== 发布失败 ===" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}

# 使用示例:
# .\build\publish.ps1                                    # 默认发布
# .\build\publish.ps1 -Runtime linux-x64                # Linux 发布
# .\build\publish.ps1 -SelfContained -SingleFile        # 自包含单文件发布
# .\build\publish.ps1 -Configuration Debug -Clean       # Debug 发布并清理