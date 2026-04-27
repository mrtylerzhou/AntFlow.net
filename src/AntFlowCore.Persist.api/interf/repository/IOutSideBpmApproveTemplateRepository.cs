using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmApproveTemplateRepository : IBaseRepository<OutSideBpmApproveTemplate>
{
   List<OutSideBpmApproveTemplate> ListPage(Expression<Func<OutSideBpmApproveTemplate, bool>> expression, PagingInfo pagingInfo);
    void DeleteById(long id);
}
