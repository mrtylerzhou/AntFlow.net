using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodeBusinessTableConfService: AFBaseCurdRepositoryService<BpmnNodeBusinessTableConf>,IBpmnNodeBusinessTableConfService
{
    public BpmnNodeBusinessTableConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}