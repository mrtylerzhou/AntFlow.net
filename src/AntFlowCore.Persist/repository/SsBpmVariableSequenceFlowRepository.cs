using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmVariableSequenceFlowRepository : RepositoryBase<BpmVariableSequenceFlow>, IBpmVariableSequenceFlowRepository
{
    public SsBpmVariableSequenceFlowRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
