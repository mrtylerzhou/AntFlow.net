using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.repository;

public class FsInformationTemplateRepository : RepositoryBase<InformationTemplate>, IInformationTemplateRepository
{
   

    public FsInformationTemplateRepository(AntFlowOrmContext context) : base(context)
    {
       
    }
    
    public List<InformationTemplate> GetInformationTemplateByExpression(  Expression<Func<InformationTemplate, bool>> expression, PagingInfo pagingInfo)
    {
        BasePagingInfo basePagingInfo = pagingInfo.ToBasePagingInfo();
        List<InformationTemplate> informationTemplates = _ormContext.FreeSql.GetRepository<InformationTemplate>()
            .Where(expression)
            .Page(basePagingInfo)
            .ToList();

      pagingInfo.Count=informationTemplates.Count;
       
        return informationTemplates;
    }
}
