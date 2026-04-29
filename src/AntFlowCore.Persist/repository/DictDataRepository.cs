using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class DictDataRepository : RepositoryBase<DictData>, IDictDataRepository
{
    public DictDataRepository(AntFlowOrmContext ormContext) : base(ormContext)
    {
    }

    public List<DictData> QueryDictDataListByExpression(Expression<Func<DictData, bool>> expression, PagingInfo pagingInfo)
    {
        int totalCount = 0;
        List<DictData> dictDatas = Db.Queryable<DictData>()
            .Where(expression)
            .OrderByDescending(c => c.CreateTime)
            .ToPageList(pagingInfo.PageNumber, pagingInfo.PageSize, ref totalCount);
        pagingInfo.Count = totalCount;
        return dictDatas;
    }

    public List<DictData> QueryDictDataListByExpression(Expression<Func<DictData, BpmnConf, bool>> expression, PagingInfo pagingInfo)
    {
        int totalCount = 0;
        List<DictData> dictDataList = Db.Queryable<DictData>()
            .InnerJoin<BpmnConf>((a, b) => a.Value == b.FormCode && b.IsLowCodeFlow == 1)
            .Where(expression)
            .OrderByDescending(a => a.CreateTime)
            .ToPageList(pagingInfo.PageNumber, pagingInfo.PageSize, ref totalCount);
        pagingInfo.Count = totalCount;
        return dictDataList;
    }
}
