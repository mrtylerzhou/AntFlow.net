using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class BpmVariableSequenceFlowService : AFBaseCurdRepositoryService<BpmVariableSequenceFlow>,
    IBpmVariableSequenceFlowService
{
    public BpmVariableSequenceFlowService(IFreeSql freeSql) : base(freeSql)
    {
    }
}