using antflowcore.entity;

namespace antflowcore.service.repository;

public class LFMainService : AFBaseCurdRepositoryService<LFMain>
{
    public LFMainService(IFreeSql freeSql) : base(freeSql)
    {
    }
}