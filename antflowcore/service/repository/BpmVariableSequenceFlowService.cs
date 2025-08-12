using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmVariableSequenceFlowService: AFBaseCurdRepositoryService<BpmVariableSequenceFlow>,IBpmVariableSequenceFlowService
{
    public BpmVariableSequenceFlowService(IFreeSql freeSql) : base(freeSql)
    {
    }
}