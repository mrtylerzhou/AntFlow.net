using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;

namespace AntFlow.Core.Service.Repository;

public class DepartmentService : AFBaseCurdRepositoryService<Department>, IDepartmentService
{
    public DepartmentService(IFreeSql freeSql) : base(freeSql)
    {
    }
}