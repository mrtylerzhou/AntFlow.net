using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.repositorysitory;

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

    /// <summary>
    /// 字典管理分页查询(过滤 is_del=0 + 租户, 支持类型/关键字筛选, 按 sort asc + id desc)
    /// </summary>
    public List<DictData> QueryPageList(DictDataPageReq req, string tenantId, Page<DictData> page)
    {
        BasePagingInfo pagingInfo = page.ToPagingInfo().ToBasePagingInfo();
        var query = _ormContext.FreeSql.Select<DictData>()
            .Where(a => a.IsDel == 0);
        if (!string.IsNullOrEmpty(tenantId))
        {
            query = query.Where(a => a.TenantId == tenantId);
        }
        if (!string.IsNullOrEmpty(req.DictType))
        {
            query = query.Where(a => a.DictType == req.DictType);
        }
        if (!string.IsNullOrEmpty(req.Keyword))
        {
            query = query.Where(a => a.Label.Contains(req.Keyword) || a.Value.Contains(req.Keyword));
        }
        List<DictData> list = query.OrderBy(a => a.Sort).OrderByDescending(a => a.Id)
            .Page(pagingInfo)
            .ToList();
        page.Total = (int)pagingInfo.Count;
        return list;
    }
}