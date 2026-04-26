using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNoticeService : IAntFlowRepositoryMix<BpmProcessNotice, IBpmProcessNoticeRepository>
{
    List<BpmProcessNotice> ProcessNoticeList(string processKey);
    void SaveProcessNotice(BpmProcessDeptVo vo);
    IDictionary<string, List<BpmProcessNotice>> ProcessNoticeMap(List<string> formCodes);
}
