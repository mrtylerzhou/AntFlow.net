using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeConditionsConfService : IAntFlowRepositoryMix<BpmnNodeConditionsConf, IBpmnNodeConditionsConfRepository>
{
    List<string> QueryConditionParamNameByProcessNumber(BusinessDataVo businessDataVo);
}
