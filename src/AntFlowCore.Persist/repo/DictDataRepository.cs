using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.repo;

public class DictDataRepository : RepositoryBase<DictData> , IDictDataRepository
{
    public DictDataRepository(AntFlowOrmContext ormContext) : base(ormContext)
    {
    }
    
    public List<DictData> QueryDictDataListByExpression(Expression<Func<DictData, bool>> expression, PagingInfo pagingInfo)
    {
        
        BasePagingInfo basePagingInfo = pagingInfo.ToBasePagingInfo();
        List<DictData> dictDatas = _ormContext.FreeSql.GetRepository<DictData>()
            .Where(expression)
            .Page(basePagingInfo)
            .OrderByDescending(c=>c.CreateTime)
            .ToList();
        pagingInfo.Count = dictDatas.Count;
        pagingInfo.PageNumber = basePagingInfo.PageNumber;
        pagingInfo.PageSize = basePagingInfo.PageSize;
        return dictDatas;
    }
    
    public List<DictData> QueryDictDataListByExpression( Expression<Func<DictData, BpmnConf,bool>> expression, PagingInfo pagingInfo)
    {
        BasePagingInfo basePagingInfo = pagingInfo.ToBasePagingInfo();
        List<DictData> dictDataList = _ormContext.FreeSql
                .Select<DictData, BpmnConf>()
                .InnerJoin((a, b) => a.Value == b.FormCode && b.IsLowCodeFlow == 1)
                .Where(expression)
                .OrderByDescending((a, b) => a.CreateTime)
                .Page(basePagingInfo)
                .ToList<DictData>((a, b) => a);
        pagingInfo.Count = dictDataList.Count;
        pagingInfo.PageNumber = basePagingInfo.PageNumber;
        pagingInfo.PageSize = basePagingInfo.PageSize;
        return dictDataList;
    }
}