using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeBusinessTableConfService : IAntFlowRepositoryMix<BpmnNodeBusinessTableConf, IBpmnNodeBusinessTableConfRepository>
{
    BpmnNodeBusinessTableConf? GetByBpmnNodeId(long bpmnNodeId);
    void Insert(BpmnNodeBusinessTableConf entity);
}
