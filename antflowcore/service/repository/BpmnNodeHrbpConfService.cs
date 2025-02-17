using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnNodeHrbpConfService: AFBaseCurdRepositoryService<BpmnNodeHrbpConf>
{
    public BpmnNodeHrbpConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}