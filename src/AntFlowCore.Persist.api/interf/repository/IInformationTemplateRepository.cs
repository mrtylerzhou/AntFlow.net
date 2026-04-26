using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IInformationTemplateRepository : IBaseRepository<InformationTemplate>
{
    public (List<InformationTemplate>, PagingInfo pagingInfo) GetInformationTemplateByExpression(
        Expression<Func<InformationTemplate, bool>> expression, BasePagingInfo basePagingInfo);
}
