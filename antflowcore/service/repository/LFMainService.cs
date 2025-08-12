using antflowcore.entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class LFMainService: AFBaseCurdRepositoryService<LFMain>,ILFMainService
{
    public LFMainService(IFreeSql freeSql) : base(freeSql)
    {
    }
}