using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsOutSideBpmConditionsTemplateRepository : RepositoryBase<OutSideBpmConditionsTemplate>, IOutSideBpmConditionsTemplateRepository
{
    public FsOutSideBpmConditionsTemplateRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
