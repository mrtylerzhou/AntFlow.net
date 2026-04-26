using System.Linq.Expressions;

namespace AntFlowCore.Abstraction.Orm.repository;

/// <summary>
/// FreeSql Repository 基类
/// 替换 EF Core 版 RepositoryBase
/// </summary>
public class RepositoryBase<TEntity> : IBaseRepository<TEntity>
    where TEntity : class
{
    protected readonly AntFlowOrmContext _ormContext;
    
    public RepositoryBase( AntFlowOrmContext ormContext)
    {
        _ormContext=ormContext;
    }

    #region IBaseRepository 接口方法实现

    public virtual IQueryable<TEntity> GetQueryable()
    {
        return _ormContext.FreeSql.GetRepository<TEntity>().Select.AsQueryable();
    }

    public virtual TEntity? GetById(object id)
    {
        return _ormContext.FreeSql.Select<TEntity>(id).First();
    }

    public virtual List<TEntity> GetAll()
    {
        return _ormContext.FreeSql.GetRepository<TEntity>().Select.ToList();
    }

    public virtual List<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
    {
        return _ormContext.FreeSql.GetRepository<TEntity>().Select.Where(predicate).ToList();
    }

    public virtual TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return _ormContext.FreeSql.GetRepository<TEntity>().Select.Where(predicate).First();
    }

    public virtual TEntity? FirstOrDefault()
    {
        return _ormContext.FreeSql.GetRepository<TEntity>().Select.First();
    }

    public virtual void Add(TEntity entity)
    {
        _ormContext.FreeSql.GetRepository<TEntity>().Insert(entity);
    }

    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        _ormContext.FreeSql.GetRepository<TEntity>().Insert(entities);
    }

    public virtual void Update(TEntity entity)
    {
        _ormContext.FreeSql.GetRepository<TEntity>().Update(entity);
    }

    public virtual void Remove(TEntity entity)
    {
        _ormContext.FreeSql.GetRepository<TEntity>().Delete(entity);
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        _ormContext.FreeSql.GetRepository<TEntity>().Delete(entities);
    }

    public virtual int Count(Expression<Func<TEntity, bool>>? predicate = null)
    {
        if (predicate == null)
        {
            return (int) _ormContext.FreeSql.GetRepository<TEntity>().Select.Count();
        }
        return (int) _ormContext.FreeSql.GetRepository<TEntity>().Select.Where(predicate).Count();
    }

    public virtual bool Any(Expression<Func<TEntity, bool>> predicate)
    {
        return  _ormContext.FreeSql.GetRepository<TEntity>().Select.Where(predicate).Any();
    }

    public virtual int SaveChanges()
    {
        // FreeSql 的 IBaseRepository 操作即时生效，无需显式 SaveChanges
        // 保留此方法以兼容接口，返回 1 表示成功
        return 1;
    }

    #endregion

    #region 额外的便捷异步方法

    public virtual IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return  _ormContext.FreeSql.GetRepository<TEntity>().Select.Where(predicate).AsQueryable();
    }

    public virtual Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
       return   _ormContext.FreeSql.GetRepository<TEntity>().Select.Where(predicate).FirstAsync(cancellationToken);
    }

    public virtual Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
    {
        return  _ormContext.FreeSql.GetRepository<TEntity>().Select.ToListAsync();
    }

    public virtual IQueryable<TEntity> Query()
    {
        return GetQueryable();
    }

    public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return  _ormContext.FreeSql.GetRepository<TEntity>().InsertAsync(entity, cancellationToken);
    }

    public virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        return  _ormContext.FreeSql.GetRepository<TEntity>().InsertAsync(entities, cancellationToken);
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        _ormContext.FreeSql.GetRepository<TEntity>().Update(entities);
    }

    public virtual Task<int> RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return  _ormContext.FreeSql.GetRepository<TEntity>().DeleteAsync(predicate, cancellationToken);
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("SaveChangesAsync not implemented yet");
    }

    #endregion

}
