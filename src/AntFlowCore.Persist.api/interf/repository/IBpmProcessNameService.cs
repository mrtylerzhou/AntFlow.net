using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNameService : IAntFlowRepositoryMix<BpmProcessName, IBpmProcessNameRepository>
{
    BpmProcessName GetBpmProcessName(string processKey);
    void EditProcessName(BpmnConf bpmnConfByCode);
    BpmProcessVo Get(string processKey);
}
