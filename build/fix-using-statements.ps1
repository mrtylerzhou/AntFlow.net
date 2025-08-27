# Fix using statements script

# Define namespace mappings (ordered by specificity)
$orderedMappings = @(
    @{ Old = 'using antflowcore.service.interf.repository;'; New = 'using AntFlow.Core.Service.Interf.Repository;' },
    @{ Old = 'using antflowcore.service.repository;'; New = 'using AntFlow.Core.Service.Repository;' },
    @{ Old = 'using antflowcore.service.processor.lowcodeflow;'; New = 'using AntFlow.Core.Service.Processor.LowCodeFlow;' },
    @{ Old = 'using antflowcore.service.processor.filter;'; New = 'using AntFlow.Core.Service.Processor.Filter;' },
    @{ Old = 'using antflowcore.service.processor;'; New = 'using AntFlow.Core.Service.Processor;' },
    @{ Old = 'using antflowcore.service.formprocess;'; New = 'using AntFlow.Core.Service.FormProcess;' },
    @{ Old = 'using antflowcore.service.org&dept;'; New = 'using AntFlow.Core.Service.OrgDept;' },
    @{ Old = 'using antflowcore.service.interf;'; New = 'using AntFlow.Core.Service.Interf;' },
    @{ Old = 'using antflowcore.service.biz;'; New = 'using AntFlow.Core.Service.Biz;' },
    @{ Old = 'using antflowcore.service;'; New = 'using AntFlow.Core.Service;' },
    @{ Old = 'using antflowcore.constant.enums;'; New = 'using AntFlow.Core.Constant.Enums;' },
    @{ Old = 'using antflowcore.constant.enus;'; New = 'using AntFlow.Core.Constant.Enums;' },
    @{ Old = 'using antflowcore.constant;'; New = 'using AntFlow.Core.Constant;' },
    @{ Old = 'using antflowcore.adaptor.bpmnelementadp;'; New = 'using AntFlow.Core.Adaptor.BpmnElementAdp;' },
    @{ Old = 'using antflowcore.adaptor;'; New = 'using AntFlow.Core.Adaptor;' },
    @{ Old = 'using antflowcore.bpmn.service;'; New = 'using AntFlow.Core.Bpmn.Service;' },
    @{ Old = 'using antflowcore.bpmn;'; New = 'using AntFlow.Core.Bpmn;' },
    @{ Old = 'using antflowcore.conf.serviceregistration;'; New = 'using AntFlow.Core.Conf.ServiceRegistration;' },
    @{ Old = 'using antflowcore.conf.middleware;'; New = 'using AntFlow.Core.Conf.Middleware;' },
    @{ Old = 'using antflowcore.conf.freesql;'; New = 'using AntFlow.Core.Conf.FreeSql;' },
    @{ Old = 'using antflowcore.conf.json;'; New = 'using AntFlow.Core.Conf.Json;' },
    @{ Old = 'using antflowcore.conf.di;'; New = 'using AntFlow.Core.Conf.Di;' },
    @{ Old = 'using antflowcore.conf;'; New = 'using AntFlow.Core.Conf;' },
    @{ Old = 'using antflowcore.util.Extension;'; New = 'using AntFlow.Core.Util.Extension;' },
    @{ Old = 'using antflowcore.util;'; New = 'using AntFlow.Core.Util;' },
    @{ Old = 'using antflowcore.factory;'; New = 'using AntFlow.Core.Factory;' },
    @{ Old = 'using antflowcore.exception;'; New = 'using AntFlow.Core.Exception;' },
    @{ Old = 'using antflowcore.entity;'; New = 'using AntFlow.Core.Entity;' },
    @{ Old = 'using antflowcore.dto;'; New = 'using AntFlow.Core.Dto;' },
    @{ Old = 'using antflowcore.controller;'; New = 'using AntFlow.Core.Controller;' },
    @{ Old = 'using antflowcore.formatter;'; New = 'using AntFlow.Core.Formatter;' },
    @{ Old = 'using antflowcore.http;'; New = 'using AntFlow.Core.Http;' },
    @{ Old = 'using antflowcore.evt;'; New = 'using AntFlow.Core.Evt;' },
    @{ Old = 'using antflowcore.aop;'; New = 'using AntFlow.Core.Aop;' },
    @{ Old = 'using antflowcore.vo;'; New = 'using AntFlow.Core.Vo;' },
    @{ Old = 'using antflowcore;'; New = 'using AntFlow.Core;' },
    @{ Old = 'using AntFlowCore.Entities;'; New = 'using AntFlow.Core.Entity;' },
    @{ Old = 'using AntFlowCore.Entity;'; New = 'using AntFlow.Core.Entity;' },
    @{ Old = 'using AntFlowCore.Vo;'; New = 'using AntFlow.Core.Vo;' },
    @{ Old = 'using AntFlowCore.Util;'; New = 'using AntFlow.Core.Util;' },
    @{ Old = 'using AntFlowCore.Enums;'; New = 'using AntFlow.Core.Enums;' },
    @{ Old = 'using AntFlowCore.Constants;'; New = 'using AntFlow.Core.Constants;' },
    @{ Old = 'using AntFlowCore;'; New = 'using AntFlow.Core;' },
    @{ Old = 'using Antflowcore.Vo;'; New = 'using AntFlow.Core.Vo;' },
    @{ Old = 'using AntOffice.Base.Util;'; New = 'using AntFlow.Core.Util;' }
)

# Get all C# files
$csFiles = Get-ChildItem -Path "d:\Code\AntFlow.net\src" -Filter "*.cs" -Recurse

$totalFiles = $csFiles.Count
$modifiedFiles = 0

Write-Host "Processing $totalFiles C# files..."

foreach ($file in $csFiles) {
    $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
    $originalContent = $content
    
    foreach ($mapping in $orderedMappings) {
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

Write-Host "Processing completed!"
Write-Host "Total files: $totalFiles"
Write-Host "Modified files: $modifiedFiles"