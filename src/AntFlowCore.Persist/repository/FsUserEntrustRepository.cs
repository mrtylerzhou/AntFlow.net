using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsUserEntrustRepository : RepositoryBase<UserEntrust>, IUserEntrustRepository
{
    public FsUserEntrustRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<Entrust> QueryEntrustPageList(string userId)
    {
        Expression<Func<UserEntrust, User, bool>> expression = (a, b) => 1 == 1;
        if (!string.IsNullOrEmpty(userId))
        {
            LambadaExpressionExtensions.And(expression, (a, b) => a.Sender == userId || a.ReceiverId == userId);
        }

        List<Entrust> entrusts = _ormContext.FreeSql
            .Select<UserEntrust, User>()
            .LeftJoin((a, b) => a.Sender == b.Id.ToString())
            .Where(expression)
            .ToList<Entrust>(a => new Entrust()
            {
                Id = a.t1.Id,
                Name = a.t2.Name,
                Sender = a.t1.Sender,
                ReceiverId = a.t1.ReceiverId,
                ReceiverName = a.t1.ReceiverName,
                PowerId = a.t1.PowerId,
                BeginTime = a.t1.BeginTime,
                EndTime = a.t1.EndTime,
                CreateTime = a.t1.CreateTime,
            });
        return entrusts;
    }
}
