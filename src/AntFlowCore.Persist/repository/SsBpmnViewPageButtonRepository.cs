using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnViewPageButtonRepository : RepositoryBase<BpmnViewPageButton>, IBpmnViewPageButtonRepository
{
    public SsBpmnViewPageButtonRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void DeleteByConfId(long confId)
    {
        Db.Deleteable<BpmnViewPageButton>()
            .Where(a => a.ConfId == confId)
            .ExecuteCommand();
    }
}
