using AntFlowCore.Core.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnNodeAdditionalSignConfService : AFBaseCurdRepositoryService<BpmnNodeAdditionalSignConf>, IBpmnNodeAdditionalSignConfService
{
    public BpmnNodeAdditionalSignConfService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmnNodeAdditionalSignConf> GetByBpmnNodeId(long? bpmnNodeId)
    {
        return baseRepo.Where(conf => conf.BpmnNodeId == bpmnNodeId).ToList();
    }

    public void Insert(List<BpmnNodeAdditionalSignConf> entities)
    {
        baseRepo.Insert(entities);
    }
}