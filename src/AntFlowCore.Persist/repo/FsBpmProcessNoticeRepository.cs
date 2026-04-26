using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmProcessNoticeRepository : RepositoryBase<BpmProcessNotice>, IBpmProcessNoticeRepository
{
    public FsBpmProcessNoticeRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void DeleteByProcessKey(string processKey)
    {
        _ormContext.FreeSql.Delete<BpmProcessNotice>()
            .Where(a => a.ProcessKey == processKey)
            .ExecuteAffrows();
    }

    public void AddRange(List<BpmProcessNotice> notices)
    {
        _ormContext.FreeSql.Insert(notices).ExecuteAffrows();
    }
}
