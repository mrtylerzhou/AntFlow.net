using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnConfLfFormdataFieldService: AFBaseCurdRepositoryService<BpmnConfLfFormdataField>
{
    public BpmnConfLfFormdataFieldService(IFreeSql freeSql) : base(freeSql)
    {
    }
}