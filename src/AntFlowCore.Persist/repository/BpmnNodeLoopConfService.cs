using AntFlowCore.Core.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodeLoopConfService: AFBaseCurdRepositoryService<BpmnNodeLoopConf>,IBpmnNodeLoopConfService
{
    public BpmnNodeLoopConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}