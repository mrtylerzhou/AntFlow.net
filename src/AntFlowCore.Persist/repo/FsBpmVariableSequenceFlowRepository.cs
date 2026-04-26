using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmVariableSequenceFlowRepository : RepositoryBase<BpmVariableSequenceFlow>, IBpmVariableSequenceFlowRepository
{
    public FsBpmVariableSequenceFlowRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
