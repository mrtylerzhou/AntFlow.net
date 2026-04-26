using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IDictDataRepository: IBaseRepository<DictData>
{
    public List<DictData> QueryDictDataListByExpression(Expression<Func<DictData, bool>> expression, PagingInfo pagingInfo);

    public List<DictData> QueryDictDataListByExpression(Expression<Func<DictData, BpmnConf, bool>> expression,
        PagingInfo pagingInfo);
}
