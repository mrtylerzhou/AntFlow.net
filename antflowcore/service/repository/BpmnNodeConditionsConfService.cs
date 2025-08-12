using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodeConditionsConfService: AFBaseCurdRepositoryService<BpmnNodeConditionsConf>,IBpmnNodeConditionsConfService
{
    public BpmnNodeConditionsConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}