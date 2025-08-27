# PowerShell script to fix namespace naming conventions
# This script will update all namespace declarations to follow C# naming conventions

$sourceDir = "d:\Code\AntFlow.net\src\AntFlow.Core"

# Define namespace mappings from old to new (ordered by specificity - most specific first)
$namespaceMappings = @(
    @{Old="antflowcore.service.processor.lowcodeflow"; New="AntFlow.Core.Service.Processor.LowCodeFlow"},
    @{Old="antflowcore.service.processor.personnel"; New="AntFlow.Core.Service.Processor.Personnel"},
    @{Old="antflowcore.service.processor.filter"; New="AntFlow.Core.Service.Processor.Filter"},
    @{Old="antflowcore.service.interf.repository"; New="AntFlow.Core.Service.Interface.Repository"},
    @{Old="antflowcore.service.processor"; New="AntFlow.Core.Service.Processor"},
    @{Old="antflowcore.service.repository"; New="AntFlow.Core.Service.Repository"},
    @{Old="antflowcore.service.formprocess"; New="AntFlow.Core.Service.FormProcess"},
    @{Old="antflowcore.service.interf"; New="AntFlow.Core.Service.Interface"},
    @{Old="antflowcore.service.biz"; New="AntFlow.Core.Service.Business"},
    @{Old="antflowcore.factory.tagparser"; New="AntFlow.Core.Factory.TagParser"},
    @{Old="antflowcore.util.Extension"; New="AntFlow.Core.Util.Extension"},
    @{Old="antflowcore.constant.enus"; New="AntFlow.Core.Constant.Enums"},
    @{Old="antflowcore.constant.enums"; New="AntFlow.Core.Constant.Enums"},
    @{Old="antflowcore.conf.serviceregistration"; New="AntFlow.Core.Configuration.ServiceRegistration"},
    @{Old="antflowcore.conf.middleware"; New="AntFlow.Core.Configuration.Middleware"},
    @{Old="antflowcore.conf.di"; New="AntFlow.Core.Configuration.DependencyInjection"},
    @{Old="antflowcore.bpmn.service"; New="AntFlow.Core.Bpmn.Service"},
    @{Old="antflowcore.service"; New="AntFlow.Core.Service"},
    @{Old="antflowcore.controller"; New="AntFlow.Core.Controller"},
    @{Old="antflowcore.entity"; New="AntFlow.Core.Entity"},
    @{Old="antflowcore.vo"; New="AntFlow.Core.Vo"},
    @{Old="antflowcore.dto"; New="AntFlow.Core.Dto"},
    @{Old="antflowcore.util"; New="AntFlow.Core.Util"},
    @{Old="antflowcore.constant"; New="AntFlow.Core.Constant"},
    @{Old="antflowcore.factory"; New="AntFlow.Core.Factory"},
    @{Old="antflowcore.evt"; New="AntFlow.Core.Event"},
    @{Old="antflowcore.http"; New="AntFlow.Core.Http"},
    @{Old="antflowcore.exception"; New="AntFlow.Core.Exception"},
    @{Old="antflowcore"; New="AntFlow.Core"},
    @{Old="AntFlowCore.Entity"; New="AntFlow.Core.Entity"},
    @{Old="AntFlowCore.Entities"; New="AntFlow.Core.Entity"},
    @{Old="AntFlowCore.Vo"; New="AntFlow.Core.Vo"},
    @{Old="Antflowcore.Vo"; New="AntFlow.Core.Vo"},
    @{Old="AntFlowCore.Util"; New="AntFlow.Core.Util"},
    @{Old="AntOffice.Base.Util"; New="AntFlow.Core.Util"},
    @{Old="AntFlowCore.Enums"; New="AntFlow.Core.Constant.Enums"},
    @{Old="AntFlowCore.Constants"; New="AntFlow.Core.Constant"},
    @{Old="AntFlowCore"; New="AntFlow.Core"},
    @{Old="YourNamespace"; New="AntFlow.Core.Constant.Enums"}
)

# Get all C# files
$files = Get-ChildItem -Path $sourceDir -Filter "*.cs" -Recurse

Write-Host "Found $($files.Count) C# files to process..."

foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    $originalContent = $content
    $modified = $false
    
    # Process each namespace mapping (ordered by specificity)
    foreach ($mapping in $namespaceMappings) {
        $oldNamespace = $mapping.Old
        $newNamespace = $mapping.New
        
        # Match namespace declarations (with or without semicolon)
        $pattern = "namespace\s+" + [regex]::Escape($oldNamespace) + "(\s*[;{]|\s*$)"
        if ($content -match $pattern) {
            $content = $content -replace $pattern, "namespace $newNamespace`$1"
            $modified = $true
            Write-Host "Updated namespace '$oldNamespace' to '$newNamespace' in: $($file.Name)"
        }
    }
    
    # Save the file if it was modified
    if ($modified) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
    }
}

Write-Host "Namespace fixing completed!"