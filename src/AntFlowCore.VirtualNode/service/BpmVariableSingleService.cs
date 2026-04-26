using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmVariableSingleService: AFBaseCurdRepositoryService<BpmVariableSingle>,IBpmVariableSingleService
{
    public BpmVariableSingleService(IFreeSql freeSql) : base(freeSql)
    {
    }
}