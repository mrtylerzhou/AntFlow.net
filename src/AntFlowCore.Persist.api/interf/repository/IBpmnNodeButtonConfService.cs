using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeButtonConfService : IAntFlowRepositoryMix<BpmnNodeButtonConf, IBpmnNodeButtonConfRepository>
{
    void EditButtons(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
    List<BpmnNodeButtonConf>? QueryConfByBpmnConde(string version);
}
