using antflowcore.entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class DicDataSerivce: AFBaseCurdRepositoryService<DictData>,IDicDataSerivce
{
    public DicDataSerivce(IFreeSql freeSql) : base(freeSql)
    {
    }
}