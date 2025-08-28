# Clean duplicate using statements script

# Get all C# files
$csFiles = Get-ChildItem -Path "d:\Code\AntFlow.net\src" -Filter "*.cs" -Recurse

$totalFiles = $csFiles.Count
$modifiedFiles = 0

Write-Host "Processing $totalFiles C# files to clean duplicate using statements..."

foreach ($file in $csFiles) {
    $lines = Get-Content -Path $file.FullName -Encoding UTF8
    $originalContent = $lines -join "`n"
    
    # Find using statements
    $usingLines = @()
    $nonUsingLines = @()
    $inUsings = $true
    
    foreach ($line in $lines) {
        if ($line.Trim().StartsWith('using ') -and $inUsings) {
            $usingLines += $line.Trim()
        } elseif ($line.Trim() -eq '' -and $inUsings) {
            # Skip empty lines in using section
        } else {
            $inUsings = $false
            $nonUsingLines += $line
        }
    }
    
    # Remove duplicates and sort using statements
    $uniqueUsings = $usingLines | Sort-Object | Get-Unique
    
    # Rebuild file content
    $newContent = @()
    $newContent += $uniqueUsings
    if ($uniqueUsings.Count -gt 0) {
        $newContent += ''
    }
    $newContent += $nonUsingLines
    
    $newContentString = $newContent -join "`n"
    
    if ($newContentString -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $newContentString -Encoding UTF8 -NoNewline
        $modifiedFiles++
        Write-Host "Cleaned: $($file.FullName)"
    }
}

Write-Host "Using statements cleanup completed!"
Write-Host "Total files: $totalFiles"
Write-Host "Modified files: $modifiedFiles"