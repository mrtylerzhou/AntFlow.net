using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeConditionsConfService : IBaseRepositoryService<BpmnNodeConditionsConf>
{
    List<String> QueryConditionParamNameByProcessNumber(BusinessDataVo businessDataVo);
}
