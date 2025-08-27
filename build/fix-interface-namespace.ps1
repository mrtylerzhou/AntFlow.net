# Fix interface namespace references script

# Define namespace mappings for interface references
$interfaceMappings = @(
    @{ Old = 'using AntFlow.Core.Service.Interf.Repository;'; New = 'using AntFlow.Core.Service.Interface.Repository;' },
    @{ Old = 'using AntFlow.Core.Service.Interf;'; New = 'using AntFlow.Core.Service.Interface;' }
)

# Get all C# files
$csFiles = Get-ChildItem -Path "d:\Code\AntFlow.net\src" -Filter "*.cs" -Recurse

$totalFiles = $csFiles.Count
$modifiedFiles = 0

Write-Host "Processing $totalFiles C# files for interface namespace fixes..."

foreach ($file in $csFiles) {
    $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
    $originalContent = $content
    
    foreach ($mapping in $interfaceMappings) {
        $oldUsing = $mapping.Old
        $newUsing = $mapping.New
        $content = $content -replace [regex]::Escape($oldUsing), $newUsing
    }
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -Encoding UTF8 -NoNewline
        $modifiedFiles++
        Write-Host "Fixed: $($file.FullName)"
    }
}

Write-Host "Interface namespace fix completed!"
Write-Host "Total files: $totalFiles"
Write-Host "Modified files: $modifiedFiles"