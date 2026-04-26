using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class DepartmentService: AFBaseCurdRepositoryService<Department>,IDepartmentService
{
    public DepartmentService(IFreeSql freeSql) : base(freeSql)
    {
    }
}