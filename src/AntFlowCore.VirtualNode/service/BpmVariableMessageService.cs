using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmVariableMessageService : AFBaseCurdRepositoryService<BpmVariableMessage>,IBpmVariableMessageService
{
    public BpmVariableMessageService(IFreeSql freeSql) : base(freeSql)
    {
    }
}