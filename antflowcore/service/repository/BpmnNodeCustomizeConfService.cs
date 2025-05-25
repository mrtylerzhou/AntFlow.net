using antflowcore.entity;

namespace antflowcore.service.repository;

public class BpmnNodeCustomizeConfService: AFBaseCurdRepositoryService<BpmnNodeCustomizeConf>
{
    public BpmnNodeCustomizeConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}