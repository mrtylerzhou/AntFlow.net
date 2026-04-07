using AntFlowCore.Core.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmVariableSingleService: AFBaseCurdRepositoryService<BpmVariableSingle>,IBpmVariableSingleService
{
    public BpmVariableSingleService(IFreeSql freeSql) : base(freeSql)
    {
    }
}