using antflowcore.entity;

namespace antflowcore.service.repository;

public class DicDataSerivce : AFBaseCurdRepositoryService<DictData>
{
    public DicDataSerivce(IFreeSql freeSql) : base(freeSql)
    {
    }
}