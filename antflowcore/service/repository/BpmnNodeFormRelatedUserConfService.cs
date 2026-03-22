using AntFlowCore.Entity;
using antflowcore.entity;
using FreeSql;

namespace antflowcore.service.repository;

public class BpmnNodeFormRelatedUserConfService : AFBaseCurdRepositoryService<BpmnNodeFormRelatedUserConf>
{
    private readonly BpmnNodeService _bpmnNodeService;

    public BpmnNodeFormRelatedUserConfService(IFreeSql freeSql, BpmnNodeService bpmnNodeService) : base(freeSql)
    {
        _bpmnNodeService = bpmnNodeService;
    }

    /// <summary>
    /// 根据流程配置ID查询表单关联用户配置
    /// </summary>
    public List<BpmnNodeFormRelatedUserConf> QueryByConfId(long confId)
    {
       
        List<long> nodeIds = _bpmnNodeService.baseRepo
            .Where(x => x.ConfId == confId && x.IsDel != 1)
            .ToList()
            .Select(x => x.Id)
            .ToList();

        if (nodeIds == null || !nodeIds.Any())
        {
            return new List<BpmnNodeFormRelatedUserConf>();
        }

        return baseRepo
            .Where(x => nodeIds.Contains(x.BpmnNodeId) && x.IsDel != 1)
            .ToList();
    }
}
