using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodeSignUpConfService : IBpmnNodeSignUpConfService
{
    public BpmnNodeSignUpConfService(IBpmnNodeSignUpConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeSignUpConfRepository _repository { get; }

    public void EditSignUpConf(BpmnNodeVo bpmnNodeVo, long bpmnNodeId)
    {
        _repository.EditSignUpConf(bpmnNodeVo, bpmnNodeId);
    }
}
