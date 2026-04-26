using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsUserMessageStatusRepository : RepositoryBase<UserMessageStatus>, IUserMessageStatusRepository
{
    public FsUserMessageStatusRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
