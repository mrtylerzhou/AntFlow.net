using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeSignUpConfService : IAntFlowRepositoryMix<BpmnNodeSignUpConf, IBpmnNodeSignUpConfRepository>
{
    void EditSignUpConf(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
}
