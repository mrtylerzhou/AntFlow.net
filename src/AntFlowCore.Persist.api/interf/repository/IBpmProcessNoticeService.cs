using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNoticeService : IAntFlowRepositoryMix<BpmProcessNotice, IBpmProcessNoticeRepository>
{
    List<BpmProcessNotice> ProcessNoticeList(string processKey);
    void SaveProcessNotice(BpmProcessDeptVo vo);
    IDictionary<string, List<BpmProcessNotice>> ProcessNoticeMap(List<string> formCodes);
}
