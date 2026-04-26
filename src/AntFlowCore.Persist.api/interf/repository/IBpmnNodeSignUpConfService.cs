using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeSignUpConfService : IAntFlowRepositoryMix<BpmnNodeSignUpConf, IBpmnNodeSignUpConfRepository>
{
    void EditSignUpConf(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
}
