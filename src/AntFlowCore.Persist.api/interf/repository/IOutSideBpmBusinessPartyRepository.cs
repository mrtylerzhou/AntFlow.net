using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmBusinessPartyRepository : IBaseRepository<OutSideBpmBusinessParty>
{
    List<OutSideBpmBusinessParty> ListPage(Expression<Func<OutSideBpmBusinessParty, bool>> expression, PagingInfo pagingInfo);
}
