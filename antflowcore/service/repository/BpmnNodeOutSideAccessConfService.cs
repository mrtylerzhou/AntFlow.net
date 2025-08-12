using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodeOutSideAccessConfService: AFBaseCurdRepositoryService<BpmnNodeOutSideAccessConf>,IBpmnNodeOutSideAccessConfService
{
    public BpmnNodeOutSideAccessConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}