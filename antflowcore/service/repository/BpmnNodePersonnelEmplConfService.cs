using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnNodePersonnelEmplConfService: AFBaseCurdRepositoryService<BpmnNodePersonnelEmplConf>
{
    public BpmnNodePersonnelEmplConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}