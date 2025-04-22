using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnNodeAssignLevelConfService : AFBaseCurdRepositoryService<BpmnNodeAssignLevelConf>
{
    public BpmnNodeAssignLevelConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}