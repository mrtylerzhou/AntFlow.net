using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnNodeConditionsConfRepository : RepositoryBase<BpmnNodeConditionsConf>, IBpmnNodeConditionsConfRepository
{
    public FsBpmnNodeConditionsConfRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<string> QueryConditionParamNameByProcessNumber(BusinessDataVo businessDataVo)
    {
        throw new NotImplementedException();
    }
}
