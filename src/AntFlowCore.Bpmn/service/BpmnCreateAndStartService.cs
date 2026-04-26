using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.bpmnmodel;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.service;

public class BpmnCreateAndStartService : IBpmnCreateAndStartService
{
    private readonly RepositoryService _repositoryService;
    private readonly RuntimeService _runtimeService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;

    public BpmnCreateAndStartService(
        RepositoryService repositoryService,
        RuntimeService runtimeService,
        IBpmBusinessProcessService bpmBusinessProcessService)
    {
        _repositoryService = repositoryService;
        _runtimeService = runtimeService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
    }
    public void CreateBpmnAndStart(BpmnConfCommonVo bpmnConfCommonVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        string deploymentId = _repositoryService.CreateDeployment(bpmnConfCommonVo, bpmnStartConditions);
        ExecutionEntity startProcessInstance = _runtimeService.StartProcessInstance(bpmnConfCommonVo,bpmnStartConditions, deploymentId);
        string processNum = bpmnStartConditions.ProcessNum;
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService._repository.FirstOrDefault(a=>a.BusinessNumber==processNum);
        if (bpmBusinessProcess == null)
        {
            throw new AFBizException($"can not find bpmn process by processNum:{processNum}");
        }
        
        _bpmBusinessProcessService._repository.UpdateProcInstId(bpmBusinessProcess.Id, startProcessInstance.ProcessInstanceId);
    }
}