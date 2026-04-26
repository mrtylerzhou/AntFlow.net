using AntFlowCore.Abstraction.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.repository;

public interface IBpmnNodeButtonConfRepository : IBaseRepository<BpmnNodeButtonConf>
{
    void EditButtons(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
    List<BpmnNodeButtonConf>? QueryConfByBpmnConde(string version);
}
