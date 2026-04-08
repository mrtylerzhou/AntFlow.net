using AntFlowCore.Core.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmnNodeBusinessTableConfService : IBaseRepositoryService<BpmnNodeBusinessTableConf>
{
    BpmnNodeBusinessTableConf? GetByBpmnNodeId(long bpmnNodeId);
    void Insert(BpmnNodeBusinessTableConf entity);
}
