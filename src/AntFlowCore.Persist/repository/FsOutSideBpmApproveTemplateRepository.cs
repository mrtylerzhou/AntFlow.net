using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.repository;

public class FsOutSideBpmApproveTemplateRepository : RepositoryBase<OutSideBpmApproveTemplate>, IOutSideBpmApproveTemplateRepository
{
    public FsOutSideBpmApproveTemplateRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<OutSideBpmApproveTemplate> ListPage(Expression<Func<OutSideBpmApproveTemplate, bool>> expression, PagingInfo pagingInfo)
    {
        BasePagingInfo basePagingInfo = pagingInfo.ToBasePagingInfo();
        List<OutSideBpmApproveTemplate> list = _ormContext.FreeSql
            .Select<OutSideBpmApproveTemplate>()
            .Where(expression)
            .OrderByDescending(a => a.CreateTime)
            .Page(basePagingInfo)
            .ToList();
        pagingInfo.Count = basePagingInfo.Count;
        return list;
    }

    public void DeleteById(long id)
    {
        _ormContext.FreeSql.Update<OutSideBpmApproveTemplate>()
            .Set(x => x.IsDel, 1)
            .Where(x => x.Id == id)
            .ExecuteAffrows();
    }
}
