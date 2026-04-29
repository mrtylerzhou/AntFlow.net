using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsUserEntrustRepository : RepositoryBase<UserEntrust>, IUserEntrustRepository
{
    public SsUserEntrustRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<Entrust> QueryEntrustPageList(string userId)
    {
        var query = Db.Queryable<UserEntrust>()
            .LeftJoin<User>((a, b) => a.Sender == b.Id.ToString());

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where((a, b) => a.Sender == userId || a.ReceiverId == userId);
        }

        List<Entrust> entrusts = query.Select((a, b) => new Entrust()
        {
            Id = a.Id,
            Name = b.Name,
            Sender = a.Sender,
            ReceiverId = a.ReceiverId,
            ReceiverName = a.ReceiverName,
            PowerId = a.PowerId,
            BeginTime = a.BeginTime,
            EndTime = a.EndTime,
            CreateTime = a.CreateTime,
        }).ToList();
        return entrusts;
    }
}
