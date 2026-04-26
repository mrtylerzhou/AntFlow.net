using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmnNodeConditionsConfService : IBpmnNodeConditionsConfService
{
    public BpmnNodeConditionsConfService(IBpmnNodeConditionsConfRepository repository)
    {
        _repository = repository;
    }

    public IBpmnNodeConditionsConfRepository _repository { get; }

    public List<string> QueryConditionParamNameByProcessNumber(BusinessDataVo businessDataVo)
    {
        return _repository.QueryConditionParamNameByProcessNumber(businessDataVo);
    }
}
