using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IInformationTemplateRepository : IBaseRepository<InformationTemplate>
{
    public List<InformationTemplate> GetInformationTemplateByExpression(
        Expression<Func<InformationTemplate, bool>> expression, PagingInfo basePagingInfo);
}
