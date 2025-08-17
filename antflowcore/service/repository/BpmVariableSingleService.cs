using AntFlowCore.Entities;
using antflowcore.entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmVariableSingleService: AFBaseCurdRepositoryService<BpmVariableSingle>,IBpmVariableSingleService
{
    public BpmVariableSingleService(IFreeSql freeSql) : base(freeSql)
    {
    }
}