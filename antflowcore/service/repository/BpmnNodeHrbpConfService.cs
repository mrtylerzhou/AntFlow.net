using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodeHrbpConfService: AFBaseCurdRepositoryService<BpmnNodeHrbpConf>,IBpmnNodeHrbpConfService
{
    public BpmnNodeHrbpConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}