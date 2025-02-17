using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnNodeLfFormdataFieldControlService: AFBaseCurdRepositoryService<BpmnNodeLfFormdataFieldControl>
{
    public BpmnNodeLfFormdataFieldControlService(IFreeSql freeSql) : base(freeSql)
    {
    }
}