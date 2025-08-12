using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodeLoopConfService: AFBaseCurdRepositoryService<BpmnNodeLoopConf>,IBpmnNodeLoopConfService
{
    public BpmnNodeLoopConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}