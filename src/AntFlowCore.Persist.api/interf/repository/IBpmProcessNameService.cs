using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessNameService : IAntFlowRepositoryMix<BpmProcessName, IBpmProcessNameRepository>
{
    BpmProcessName GetBpmProcessName(string processKey);
    void EditProcessName(BpmnConf bpmnConfByCode);
    BpmProcessVo Get(string processKey);
}
