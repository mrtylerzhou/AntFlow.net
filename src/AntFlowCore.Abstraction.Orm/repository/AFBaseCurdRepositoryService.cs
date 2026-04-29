using System.Linq.Expressions;
using SqlSugar;

namespace AntFlowCore.Abstraction.Orm.repository;

public abstract class AFBaseCurdRepositoryService<T> where T : class, new()
{
    public readonly ISqlSugarClient AfSqlSugar;

    public AFBaseCurdRepositoryService(ISqlSugarClient sqlSugar)
    {
        AfSqlSugar = sqlSugar;
    }

    public ISugarQueryable<T> Query => AfSqlSugar.Queryable<T>();

    public int Insert(T entity) => AfSqlSugar.Insertable(entity).ExecuteCommand();

    public int InsertRange(List<T> entities) => AfSqlSugar.Insertable(entities).ExecuteCommand();

    public int Update(T entity) => AfSqlSugar.Updateable(entity).ExecuteCommand();

    public int UpdateRange(List<T> entities) => AfSqlSugar.Updateable(entities).ExecuteCommand();

    public int Delete(T entity) => AfSqlSugar.Deleteable(entity).ExecuteCommand();

    public int Delete(Expression<Func<T, bool>> expression)
        => AfSqlSugar.Deleteable<T>().Where(expression).ExecuteCommand();

    public int DeleteById(object id) => AfSqlSugar.Deleteable<T>().In(id).ExecuteCommand();

    public T? GetById(object id) => AfSqlSugar.Queryable<T>().InSingle(id);

    public List<T> GetList() => AfSqlSugar.Queryable<T>().ToList();

    public List<T> GetList(Expression<Func<T, bool>> expression)
        => AfSqlSugar.Queryable<T>().Where(expression).ToList();
}
