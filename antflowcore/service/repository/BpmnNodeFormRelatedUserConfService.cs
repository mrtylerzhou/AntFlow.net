using AntFlowCore.Entity;
using FreeSql;

namespace antflowcore.service.repository;

public class BpmnNodeFormRelatedUserConfService : AFBaseCurdRepositoryService<BpmnNodeFormRelatedUserConf>
{
    public BpmnNodeFormRelatedUserConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}
