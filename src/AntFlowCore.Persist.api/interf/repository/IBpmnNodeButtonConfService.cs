using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeButtonConfService : IBaseRepositoryService<BpmnNodeButtonConf>
{
    void EditButtons(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
    List<BpmnNodeButtonConf>? QueryConfByBpmnConde(string version);
}
