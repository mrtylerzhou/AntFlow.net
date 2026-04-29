using System.Linq.Expressions;
using SqlSugar;

namespace AntFlowCore.Abstraction.Orm.repository;

public class RepositoryBase<TEntity> : IBaseRepository<TEntity>
    where TEntity : class, new()
{
    protected readonly AntFlowOrmContext _ormContext;

    public RepositoryBase(AntFlowOrmContext ormContext)
    {
        _ormContext = ormContext;
    }

    public ISqlSugarClient Db => _ormContext.SqlSugar;
    

    public virtual ISugarQueryable<TEntity> GetQueryable()
    {
        return Db.Queryable<TEntity>();
    }

    public virtual TEntity? GetById(object id)
    {
        return Db.Queryable<TEntity>().InSingle(id);
    }

    public virtual List<TEntity> GetAll()
    {
        return Db.Queryable<TEntity>().ToList();
    }

    public virtual List<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
    {
        return Db.Queryable<TEntity>().Where(predicate).ToList();
    }

    public virtual TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return Db.Queryable<TEntity>().Where(predicate).First();
    }

    public virtual TEntity? FirstOrDefault()
    {
        return Db.Queryable<TEntity>().First();
    }

    public virtual void Add(TEntity entity)
    {
        Db.Insertable(entity).ExecuteCommand();
    }

    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        Db.Insertable(entities.ToList()).ExecuteCommand();
    }

    public virtual void Update(TEntity entity)
    {
        Db.Updateable(entity).ExecuteCommand();
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        Db.Updateable(entities.ToList()).ExecuteCommand();
    }

    public virtual void Remove(TEntity entity)
    {
        Db.Deleteable(entity).ExecuteCommand();
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        Db.Deleteable(entities.ToList()).ExecuteCommand();
    }

    public virtual int Count(Expression<Func<TEntity, bool>>? predicate = null)
    {
        if (predicate == null)
        {
            return Db.Queryable<TEntity>().Count();
        }
        return Db.Queryable<TEntity>().Where(predicate).Count();
    }

    public virtual bool Any(Expression<Func<TEntity, bool>> predicate)
    {
        return Db.Queryable<TEntity>().Where(predicate).Any();
    }

    public virtual int SaveChanges()
    {
        return 1;
    }

    public virtual ISugarQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return Db.Queryable<TEntity>().Where(predicate);
    }

    public virtual Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Db.Queryable<TEntity>().Where(predicate).FirstAsync(cancellationToken);
    }

    public virtual Task<List<TEntity>> ToListAsync(CancellationToken cancellationToken = default)
    {
        return Db.Queryable<TEntity>().ToListAsync(cancellationToken);
    }

    public virtual ISugarQueryable<TEntity> Query()
    {
        return GetQueryable();
    }

    public virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        return Db.Insertable(entity).ExecuteCommandAsync(cancellationToken);
    }

    public virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        return Db.Insertable(entities.ToList()).ExecuteCommandAsync(cancellationToken);
    }

    public virtual Task<int> RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Db.Deleteable<TEntity>().Where(predicate).ExecuteCommandAsync(cancellationToken);
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(1);
    }
}
