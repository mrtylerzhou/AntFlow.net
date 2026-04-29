using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsUserMessageStatusRepository : RepositoryBase<UserMessageStatus>, IUserMessageStatusRepository
{
    public SsUserMessageStatusRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
