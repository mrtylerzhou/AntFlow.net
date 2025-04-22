using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmVariableSequenceFlowService : AFBaseCurdRepositoryService<BpmVariableSequenceFlow>
{
    public BpmVariableSequenceFlowService(IFreeSql freeSql) : base(freeSql)
    {
    }
}