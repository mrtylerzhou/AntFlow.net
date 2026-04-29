using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsOutSideBpmBusinessPartyRepository : RepositoryBase<OutSideBpmBusinessParty>, IOutSideBpmBusinessPartyRepository
{
    public SsOutSideBpmBusinessPartyRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public bool CheckDuplicateData(string name, long? id)
    {
        int count = Db.Queryable<OutSideBpmBusinessParty>()
            .Where(a => a.IsDel == 0 && a.Name == name)
            .WhereIF(id != null && id > 0, a => a.Id != id)
            .Count();
        return count > 0;
    }

    public List<OutSideBpmBusinessParty> GetListByPageNumberAndPageSize(int pageNumber, int pageSize)
    {
        return Db.Queryable<OutSideBpmBusinessParty>()
            .Where(a => a.IsDel == 0)
            .OrderBy(a => a.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public List<OutSideBpmBusinessParty> ListPage(Expression<Func<OutSideBpmBusinessParty, bool>> expression, PagingInfo pagingInfo)
    {
        int totalCount = 0;
        var result = Db.Queryable<OutSideBpmBusinessParty>()
            .Where(expression)
            .OrderByDescending(a => a.CreateTime)
            .ToPageList(pagingInfo.PageNumber, pagingInfo.PageSize, ref totalCount);
        pagingInfo.Count = totalCount;
        return result;
    }
}
