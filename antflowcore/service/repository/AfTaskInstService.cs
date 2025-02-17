using antflowcore.entity;

namespace antflowcore.service.repository;

public class AfTaskInstService: AFBaseCurdRepositoryService<BpmAfTaskInst>
{
    public AfTaskInstService(IFreeSql freeSql) : base(freeSql)
    {
    }
}