using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNoticeService : IBaseRepositoryService<BpmProcessNotice>
{
    List<BpmProcessNotice> ProcessNoticeList(string processKey);
    void SaveProcessNotice(BpmProcessDeptVo vo);
    IDictionary<string, List<BpmProcessNotice>> ProcessNoticeMap(List<string> formCodes);
}
