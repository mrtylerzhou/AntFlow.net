using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsInformationTemplateRepository : RepositoryBase<InformationTemplate>, IInformationTemplateRepository
{
    public SsInformationTemplateRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<InformationTemplate> GetInformationTemplateByExpression(
        Expression<Func<InformationTemplate, bool>> expression, PagingInfo basePagingInfo)
    {
        int count = 0;
        List<InformationTemplate> informationTemplates = Db.Queryable<InformationTemplate>()
            .Where(expression)
            .OrderBy(a => a.CreateTime, OrderByType.Desc)
            .ToPageList(basePagingInfo.PageNumber, basePagingInfo.PageSize, ref count);
        basePagingInfo.Count = count;
        return informationTemplates;
    }
}
