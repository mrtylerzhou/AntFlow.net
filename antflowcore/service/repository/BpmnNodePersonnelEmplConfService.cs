using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodePersonnelEmplConfService: AFBaseCurdRepositoryService<BpmnNodePersonnelEmplConf>,IBpmnNodePersonnelEmplConfService
{
    public BpmnNodePersonnelEmplConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}