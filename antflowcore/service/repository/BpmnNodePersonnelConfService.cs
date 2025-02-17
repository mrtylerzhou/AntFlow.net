using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnNodePersonnelConfService: AFBaseCurdRepositoryService<BpmnNodePersonnelConf>
{
    public BpmnNodePersonnelConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}