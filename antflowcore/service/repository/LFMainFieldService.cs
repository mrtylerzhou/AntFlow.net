using antflowcore.entity;

namespace antflowcore.service.repository;

public class LFMainFieldService : AFBaseCurdRepositoryService<LFMainField>
{
    public LFMainFieldService(IFreeSql freeSql) : base(freeSql)
    {
    }
}