using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnViewPageButtonRepository : RepositoryBase<BpmnViewPageButton>, IBpmnViewPageButtonRepository
{
    public FsBpmnViewPageButtonRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void DeleteByConfId(long confId)
    {
        _ormContext.FreeSql.Delete<BpmnViewPageButton>().Where(a => a.ConfId == confId).ExecuteAffrows();
       
    }
}
