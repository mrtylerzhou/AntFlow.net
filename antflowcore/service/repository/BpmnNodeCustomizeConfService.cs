using antflowcore.entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodeCustomizeConfService: AFBaseCurdRepositoryService<BpmnNodeCustomizeConf>,IBpmnNodeCustomizeConfService
{
    public BpmnNodeCustomizeConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}