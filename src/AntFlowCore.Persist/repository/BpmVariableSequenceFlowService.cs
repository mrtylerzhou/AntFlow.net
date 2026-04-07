using AntFlowCore.Core.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmVariableSequenceFlowService: AFBaseCurdRepositoryService<BpmVariableSequenceFlow>,IBpmVariableSequenceFlowService
{
    public BpmVariableSequenceFlowService(IFreeSql freeSql) : base(freeSql)
    {
    }
}