using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeAdditionalSignConfService : IAntFlowRepositoryMix<BpmnNodeAdditionalSignConf, IBpmnNodeAdditionalSignConfRepository>
{
    List<BpmnNodeAdditionalSignConf> GetByBpmnNodeId(long? bpmnNodeId);
    void Insert(List<BpmnNodeAdditionalSignConf> entities);
}
