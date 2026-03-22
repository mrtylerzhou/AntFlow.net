using AntFlowCore.Entity;
using FreeSql;

namespace antflowcore.service.repository;

public class BpmnNodeUDRConfService : AFBaseCurdRepositoryService<BpmnNodeUDRConf>
{
    public BpmnNodeUDRConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}
