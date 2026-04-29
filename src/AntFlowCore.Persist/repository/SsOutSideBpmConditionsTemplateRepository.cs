using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsOutSideBpmConditionsTemplateRepository : RepositoryBase<OutSideBpmConditionsTemplate>, IOutSideBpmConditionsTemplateRepository
{
    public SsOutSideBpmConditionsTemplateRepository(AntFlowOrmContext context) : base(context)
    {
    }
}
