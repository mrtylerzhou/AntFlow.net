using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmVariableMessageRepository : RepositoryBase<BpmVariableMessage>, IBpmVariableMessageRepository
{
    public FsBpmVariableMessageRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
