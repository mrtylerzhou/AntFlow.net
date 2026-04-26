using AntFlowCore.Abstraction.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.repository;

public interface IBpmnNodeToRepository : IBaseRepository<BpmnNodeTo>
{
    void EditNodeTo(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
}
