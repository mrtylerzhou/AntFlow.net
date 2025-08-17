using antflowcore.entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class DicMainService : AFBaseCurdRepositoryService<DictMain>,IDicMainService
{
    public DicMainService(IFreeSql freeSql) : base(freeSql)
    {
    }
}