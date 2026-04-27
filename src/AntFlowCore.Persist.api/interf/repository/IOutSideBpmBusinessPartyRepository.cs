using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmBusinessPartyRepository : IBaseRepository<OutSideBpmBusinessParty>
{
    List<OutSideBpmBusinessParty> ListPage(Expression<Func<OutSideBpmBusinessParty, bool>> expression, PagingInfo pagingInfo);
}
