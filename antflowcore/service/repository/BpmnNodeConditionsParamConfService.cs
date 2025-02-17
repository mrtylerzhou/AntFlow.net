using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnNodeConditionsParamConfService: AFBaseCurdRepositoryService<BpmnNodeConditionsParamConf>
{
    public BpmnNodeConditionsParamConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}