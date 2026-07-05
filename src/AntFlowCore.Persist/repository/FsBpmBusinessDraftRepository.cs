using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmBusinessDraftRepository : RepositoryBase<BpmBusinessDraft>, IBpmBusinessDraftRepository
{
    public FsBpmBusinessDraftRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
