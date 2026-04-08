using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmVariableMessageService : AFBaseCurdRepositoryService<BpmVariableMessage>,IBpmVariableMessageService
{
    public BpmVariableMessageService(IFreeSql freeSql) : base(freeSql)
    {
    }
}