using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IDictDataRepository: IBaseRepository<DictData>
{
    public List<DictData> QueryDictDataListByExpression(Expression<Func<DictData, bool>> expression, PagingInfo pagingInfo);

    public List<DictData> QueryDictDataListByExpression(Expression<Func<DictData, BpmnConf, bool>> expression,
        PagingInfo pagingInfo);

    /// <summary>
    /// 字典管理分页查询(过滤 is_del=0 + 租户, 支持类型/关键字筛选, 按 sort asc + id desc)
    /// </summary>
    public List<DictData> QueryPageList(DictDataPageReq req, string tenantId, Page<DictData> page);
}