using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeBusinessTableConfService : IAntFlowRepositoryMix<BpmnNodeBusinessTableConf, IBpmnNodeBusinessTableConfRepository>
{
    BpmnNodeBusinessTableConf? GetByBpmnNodeId(long bpmnNodeId);
    void Insert(BpmnNodeBusinessTableConf entity);
}
