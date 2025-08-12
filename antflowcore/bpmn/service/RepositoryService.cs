using System.Text.Json;
using antflowcore.entity;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.bpmn.service;

public class RepositoryService
{
    private readonly AFDeploymentService _deploymentService;
    private readonly ILogger<RepositoryService> _logger;

    public RepositoryService(
        AFDeploymentService deploymentService,
        ILogger<RepositoryService> logger)
    {
        _deploymentService = deploymentService;
        _logger = logger;
    }
    
    public string CreateDeployment(BpmnConfCommonVo bpmnConfCommonVo,BpmnStartConditionsVo bpmnStartConditions)
    {
        string bpmnName = bpmnConfCommonVo.BpmnName;
        string bpmnCode = bpmnConfCommonVo.BpmnCode;
        string formCode = bpmnConfCommonVo.FormCode;
        string processNumber = bpmnConfCommonVo.ProcessNum;
        string businessId = bpmnStartConditions.BusinessId;
        string startUserId = bpmnStartConditions.StartUserId;

        List<BpmnConfCommonElementVo> elementList = bpmnConfCommonVo.ElementList;
        BpmAfDeployment deployment = new BpmAfDeployment
        {
            Id = StrongUuidGenerator.GetNextId(),
            Rev = 1,
            Name = bpmnName,
            Content = JsonSerializer.Serialize(elementList),
            CreateTime = DateTime.Now,
            CreateUser = SecurityUtils.GetLogInEmpName(),
            UpdateUser = SecurityUtils.GetLogInEmpName(),
            UpdateTime = DateTime.Now,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };
        _deploymentService.baseRepo.Insert(deployment);
        return deployment.Id;
    }
}