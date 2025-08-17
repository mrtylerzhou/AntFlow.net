using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodeConditionsParamConfService: AFBaseCurdRepositoryService<BpmnNodeConditionsParamConf>,IBpmnNodeConditionsParamConfService
{
    public BpmnNodeConditionsParamConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}