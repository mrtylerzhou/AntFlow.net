using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.repository;

public class FsOutSideBpmBusinessPartyRepository : RepositoryBase<OutSideBpmBusinessParty>, IOutSideBpmBusinessPartyRepository
{
    public FsOutSideBpmBusinessPartyRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<OutSideBpmBusinessParty> ListPage(Expression<Func<OutSideBpmBusinessParty, bool>> expression, PagingInfo pagingInfo)
    {
        BasePagingInfo basePagingInfo = pagingInfo.ToBasePagingInfo();
        List<OutSideBpmBusinessParty> list = _ormContext.FreeSql
            .Select<OutSideBpmBusinessParty>()
            .Where(expression)
            .OrderByDescending(a => a.CreateTime)
            .Page(basePagingInfo)
            .ToList();
        
        pagingInfo.Count = list.Count;
        pagingInfo.PageNumber = basePagingInfo.PageNumber;
        pagingInfo.PageSize = basePagingInfo.PageSize;
        return list;
    }
}
