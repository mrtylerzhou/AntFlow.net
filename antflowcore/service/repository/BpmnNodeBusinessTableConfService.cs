using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnNodeBusinessTableConfService: AFBaseCurdRepositoryService<BpmnNodeBusinessTableConf>
{
    public BpmnNodeBusinessTableConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}