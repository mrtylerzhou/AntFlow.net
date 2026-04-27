using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsOutSideBpmAdminPersonnelRepository : RepositoryBase<OutSideBpmAdminPersonnel>, IOutSideBpmAdminPersonnelRepository
{
    public FsOutSideBpmAdminPersonnelRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
