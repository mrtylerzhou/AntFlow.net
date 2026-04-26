using AntFlowCore.Abstraction.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.repository;

public interface IBpmnNodeConditionsConfRepository : IBaseRepository<BpmnNodeConditionsConf>
{
    List<string> QueryConditionParamNameByProcessNumber(BusinessDataVo businessDataVo);
}
