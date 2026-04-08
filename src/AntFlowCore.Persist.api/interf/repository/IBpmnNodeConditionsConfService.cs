using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeConditionsConfService : IBaseRepositoryService<BpmnNodeConditionsConf>
{
    List<String> QueryConditionParamNameByProcessNumber(BusinessDataVo businessDataVo);
}
