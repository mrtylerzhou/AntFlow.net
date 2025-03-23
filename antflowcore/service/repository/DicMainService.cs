using antflowcore.entity;

namespace antflowcore.service.repository;

public class DicMainService : AFBaseCurdRepositoryService<DictMain>
{
    public DicMainService(IFreeSql freeSql) : base(freeSql)
    {
    }
}