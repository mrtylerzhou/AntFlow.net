using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNoticeRepository: IBaseRepository<BpmProcessNotice>
{
    void DeleteByProcessKey(string processKey);
    void AddRange(List<BpmProcessNotice> notices);
}
