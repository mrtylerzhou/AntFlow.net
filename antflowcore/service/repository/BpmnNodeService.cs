using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnNodeService : AFBaseCurdRepositoryService<BpmnNode>
{
    public BpmnNodeService(IFreeSql freeSql) : base(freeSql)
    {
    }
}