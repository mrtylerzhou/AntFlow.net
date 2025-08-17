using antflowcore.entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class LFMainFieldService: AFBaseCurdRepositoryService<LFMainField>,ILFMainFieldService
{
    public LFMainFieldService(IFreeSql freeSql) : base(freeSql)
    {
    }
}