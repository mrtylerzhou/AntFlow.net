using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeToService : IAntFlowRepositoryMix<BpmnNodeTo, IBpmnNodeToRepository>
{
    void EditNodeTo(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
}
