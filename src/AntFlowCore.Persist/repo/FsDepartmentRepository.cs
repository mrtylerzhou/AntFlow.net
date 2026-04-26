using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsDepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
{
    public FsDepartmentRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
