# 修复DI命名空间引用的PowerShell脚本

$sourceDir = "d:\Code\AntFlow.net\src"
$processedFiles = 0
$modifiedFiles = 0

# 获取所有C#文件
$csFiles = Get-ChildItem -Path $sourceDir -Filter "*.cs" -Recurse

foreach ($file in $csFiles) {
    $processedFiles++
    $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
    $originalContent = $content
    $modified = $false
    
    # 替换错误的DI命名空间引用
    if ($content -match "using AntFlow\.Core\.Conf\.Di;") {
        $content = $content -replace "using AntFlow\.Core\.Conf\.Di;", "using AntFlow.Core.Configuration.DependencyInjection;"
        $modified = $true
    }
    
    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -Encoding UTF8 -NoNewline
        $modifiedFiles++
        Write-Host "Fixed file: $($file.FullName)"
    }
}

Write-Host "Processing completed!"
Write-Host "Total files processed: $processedFiles"
Write-Host "Modified files count: $modifiedFiles"