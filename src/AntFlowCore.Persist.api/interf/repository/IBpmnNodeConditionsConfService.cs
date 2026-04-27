using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeConditionsConfService : IAntFlowRepositoryMix<BpmnNodeConditionsConf, IBpmnNodeConditionsConfRepository>
{
    List<string> QueryConditionParamNameByProcessNumber(BusinessDataVo businessDataVo);
}
