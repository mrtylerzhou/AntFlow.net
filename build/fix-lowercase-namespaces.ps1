# 修复小写命名空间的PowerShell脚本

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
    
    # 替换命名空间声明和using语句
    $replacements = @(
        @('antflowcore\.adaptor\.nodetypecondition\.judge', 'AntFlow.Core.Adaptor.NodeTypeCondition.Judge'),
        @('antflowcore\.adaptor\.personnel\.provider', 'AntFlow.Core.Adaptor.Personnel.Provider'),
        @('antflowcore\.adaptor\.personnel\.businesstableadp', 'AntFlow.Core.Adaptor.Personnel.BusinessTableAdp'),
        @('antflowcore\.adaptor\.processoperation', 'AntFlow.Core.Adaptor.ProcessOperation'),
        @('antflowcore\.adaptor\.bpmnelementadp', 'AntFlow.Core.Adaptor.BpmnElementAdp'),
        @('antflowcore\.adaptor\.nodetypecondition', 'AntFlow.Core.Adaptor.NodeTypeCondition'),
        @('antflowcore\.adaptor\.personnel', 'AntFlow.Core.Adaptor.Personnel'),
        @('antflowcore\.adaptor\.variable', 'AntFlow.Core.Adaptor.Variable'),
        @('antflowcore\.adaptor', 'AntFlow.Core.Adaptor'),
        @('antflowcore\.bpmn\.listener', 'AntFlow.Core.Bpmn.Listener'),
        @('antflowcore\.bpmn', 'AntFlow.Core.Bpmn'),
        @('antflowcore\.conf\.json', 'AntFlow.Core.Conf.Json'),
        @('antflowcore\.conf\.freesql', 'AntFlow.Core.Conf.FreeSql'),
        @('antflowcore\.aop', 'AntFlow.Core.Aop')
    )
    
    foreach ($replacement in $replacements) {
        $oldPattern = $replacement[0]
        $newNamespace = $replacement[1]
        
        # 替换namespace声明
        if ($content -match "namespace\s+$oldPattern\s*[;{]") {
            $content = $content -replace "namespace\s+$oldPattern\s*([;{])", "namespace $newNamespace`$1"
            $modified = $true
        }
        
        # 替换using语句
        if ($content -match "using\s+$oldPattern\s*;") {
            $content = $content -replace "using\s+$oldPattern\s*;", "using $newNamespace;"
            $modified = $true
        }
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