using AntFlowCore.Base.entity;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeAdditionalSignConfService : IBaseRepositoryService<BpmnNodeAdditionalSignConf>
{
    List<BpmnNodeAdditionalSignConf> GetByBpmnNodeId(long? bpmnNodeId);
    void Insert(List<BpmnNodeAdditionalSignConf> entities);
}
