using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnNodeConditionsConfService: AFBaseCurdRepositoryService<BpmnNodeConditionsConf>
{
    public BpmnNodeConditionsConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}