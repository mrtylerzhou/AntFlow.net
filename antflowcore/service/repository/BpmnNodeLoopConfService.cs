using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnNodeLoopConfService : AFBaseCurdRepositoryService<BpmnNodeLoopConf>
{
    public BpmnNodeLoopConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}