using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeToService : IBaseRepositoryService<BpmnNodeTo>
{
    void EditNodeTo(BpmnNodeVo bpmnNodeVo, long bpmnNodeId);
}
