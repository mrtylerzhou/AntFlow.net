using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsUserMessageRepository : RepositoryBase<UserMessage>, IUserMessageRepository
{
    public FsUserMessageRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
