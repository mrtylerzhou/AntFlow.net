using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsDepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
{
    public SsDepartmentRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
