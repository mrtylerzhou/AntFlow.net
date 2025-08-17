using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodePersonnelConfService: AFBaseCurdRepositoryService<BpmnNodePersonnelConf>,IBpmnNodePersonnelConfService
{
    public BpmnNodePersonnelConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}