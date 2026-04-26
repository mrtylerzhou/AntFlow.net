using AntFlowCore.Abstraction.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.repository;

public interface IBpmnNodeSignUpConfRepository : IBaseRepository<BpmnNodeSignUpConf>
{
    void EditSignUpConf(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
}
