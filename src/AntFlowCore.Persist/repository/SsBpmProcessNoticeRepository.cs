using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmProcessNoticeRepository : RepositoryBase<BpmProcessNotice>, IBpmProcessNoticeRepository
{
    public SsBpmProcessNoticeRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void DeleteByProcessKey(string processKey)
    {
        Db.Deleteable<BpmProcessNotice>()
            .Where(a => a.ProcessKey == processKey)
            .ExecuteCommand();
    }

    public void AddRange(List<BpmProcessNotice> notices)
    {
        Db.Insertable(notices).ExecuteCommand();
    }
}
