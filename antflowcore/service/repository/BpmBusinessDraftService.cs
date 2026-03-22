using antflowcore.entity;
using antflowcore.service.interf;
using AntFlowCore.Vo;
using FreeSql;

namespace antflowcore.service.repository;

public class BpmBusinessDraftService : AFBaseCurdRepositoryService<BpmBusinessDraft>
{
    public BpmBusinessDraftService(IFreeSql freeSql) : base(freeSql)
    {
    }
}
