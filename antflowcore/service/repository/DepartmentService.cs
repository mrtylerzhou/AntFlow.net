using antflowcore.entity;

namespace antflowcore.service.repository;

public class DepartmentService: AFBaseCurdRepositoryService<Department>
{
    public DepartmentService(IFreeSql freeSql) : base(freeSql)
    {
    }
}