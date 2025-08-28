# 批量修复命名空间引用和清理重复using语句
$files = Get-ChildItem -Path "d:\Code\AntFlow.net\src" -Recurse -Filter "*.cs" | Where-Object { $_.FullName -notlike "*\bin\*" -and $_.FullName -notlike "*\obj\*" }

foreach ($file in $files) {
    Write-Host "Processing: $($file.FullName)"
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # 替换错误的命名空间引用
     $content = $content -replace 'using AntFlow\.Core\.Constants;\s*\r?\n', ''
     $content = $content -replace 'using AntFlow\.Core\.Enums;\s*\r?\n', "using AntFlow.Core.Constant.Enums;`n"
    
    # 移除重复的using语句
    $lines = $content -split '\r?\n'
    $usingLines = @()
    $nonUsingLines = @()
    $inUsingSection = $true
    
    foreach ($line in $lines) {
        if ($line -match '^\s*using\s+') {
            if ($inUsingSection -and $usingLines -notcontains $line.Trim()) {
                $usingLines += $line.Trim()
            }
        } elseif ($line -match '^\s*$') {
            # 空行
            if (-not $inUsingSection) {
                $nonUsingLines += $line
            }
        } else {
            $inUsingSection = $false
            $nonUsingLines += $line
        }
    }
    
    # 重新组合内容
    if ($usingLines.Count -gt 0) {
        $content = ($usingLines -join "`n") + "`n`n" + ($nonUsingLines -join "`n")
    } else {
        $content = $nonUsingLines -join "`n"
    }
    
    # 清理多余的空行
    $content = $content -replace '\n\s*\n\s*\n', "`n`n"
    
    if ($content -ne $originalContent) {
        Set-Content $file.FullName $content -NoNewline
        Write-Host "Updated: $($file.FullName)"
    }
}

Write-Host "Namespace reference fix completed!"