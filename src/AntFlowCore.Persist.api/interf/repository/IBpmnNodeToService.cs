using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeToService : IBaseRepositoryService<BpmnNodeTo>
{
    void EditNodeTo(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
}
