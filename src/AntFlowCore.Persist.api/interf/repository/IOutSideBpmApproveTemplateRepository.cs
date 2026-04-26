using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmApproveTemplateRepository : IBaseRepository<OutSideBpmApproveTemplate>
{
    (List<OutSideBpmApproveTemplate>, PagingInfo) ListPage(Expression<Func<OutSideBpmApproveTemplate, bool>> expression, BasePagingInfo basePagingInfo);
    void DeleteById(long id);
}
