using antflowcore.entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class DepartmentService: AFBaseCurdRepositoryService<Department>,IDepartmentService
{
    public DepartmentService(IFreeSql freeSql) : base(freeSql)
    {
    }
}