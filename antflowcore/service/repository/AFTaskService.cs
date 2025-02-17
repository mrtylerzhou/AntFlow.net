using antflowcore.entity;

namespace antflowcore.service.repository;

public class AFTaskService: AFBaseCurdRepositoryService<BpmAfTask>
{
    public AFTaskService(IFreeSql freeSql) : base(freeSql)
    {
    }
}