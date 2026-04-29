using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsOutSideBpmApproveTemplateRepository : RepositoryBase<OutSideBpmApproveTemplate>, IOutSideBpmApproveTemplateRepository
{
    public SsOutSideBpmApproveTemplateRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<OutSideBpmApproveTemplate> ListPage(Expression<Func<OutSideBpmApproveTemplate, bool>> expression, PagingInfo pagingInfo)
    {
        int count = 0;
        List<OutSideBpmApproveTemplate> outSideBpmApproveTemplates = Db.Queryable<OutSideBpmApproveTemplate>()
            .Where(expression)
            .OrderByDescending(a => a.CreateTime)
            .ToPageList(pagingInfo.PageNumber, pagingInfo.PageSize, ref count);
        pagingInfo.Count = count;
        return outSideBpmApproveTemplates;
    }

    public void DeleteById(long id)
    {
        Db.Updateable<OutSideBpmApproveTemplate>()
            .SetColumns(a => a.IsDel == 1)
            .Where(a => a.Id == id)
            .ExecuteCommand();
    }
}
