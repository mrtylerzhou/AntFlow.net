using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmnNodeOutSideAccessConfService: AFBaseCurdRepositoryService<BpmnNodeOutSideAccessConf>
{
    public BpmnNodeOutSideAccessConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}