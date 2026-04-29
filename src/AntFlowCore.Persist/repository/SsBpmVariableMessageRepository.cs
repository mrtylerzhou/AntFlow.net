using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmVariableMessageRepository : RepositoryBase<BpmVariableMessage>, IBpmVariableMessageRepository
{
    public SsBpmVariableMessageRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
