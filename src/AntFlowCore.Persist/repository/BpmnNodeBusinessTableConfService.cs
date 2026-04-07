using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodeBusinessTableConfService : AFBaseCurdRepositoryService<BpmnNodeBusinessTableConf>, IBpmnNodeBusinessTableConfService
{
    public BpmnNodeBusinessTableConfService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public BpmnNodeBusinessTableConf? GetByBpmnNodeId(long bpmnNodeId)
    {
        return baseRepo.Where(conf => conf.BpmnNodeId == bpmnNodeId).First();
    }

    public void Insert(BpmnNodeBusinessTableConf entity)
    {
        baseRepo.Insert(entity);
    }
}