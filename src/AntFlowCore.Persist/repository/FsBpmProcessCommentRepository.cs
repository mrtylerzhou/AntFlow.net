using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmProcessCommentRepository : RepositoryBase<BpmProcessComment>, IBpmProcessCommentRepository
{
    public FsBpmProcessCommentRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
