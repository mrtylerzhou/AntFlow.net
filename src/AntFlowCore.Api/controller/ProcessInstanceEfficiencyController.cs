using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Business.service;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

/// <summary>
/// 流程实例效能(流程监控 → 更多 → 效能)
/// 实时计算,不入库。
/// </summary>
[Route("processInstanceEfficiency")]
public class ProcessInstanceEfficiencyController
{
    private readonly ProcessInstanceEfficiencyService _processInstanceEfficiencyService;

    public ProcessInstanceEfficiencyController(ProcessInstanceEfficiencyService processInstanceEfficiencyService)
    {
        _processInstanceEfficiencyService = processInstanceEfficiencyService;
    }

    /// <summary>
    /// 顶部汇总(当时耗时、流程状态、发起时间)
    /// </summary>
    [HttpGet("summary")]
    public Result<InstanceEfficiencySummaryVo> Summary([FromQuery] string processNumber)
    {
        return ResultHelper.Success(_processInstanceEfficiencyService.GetSummary(processNumber));
    }

    /// <summary>
    /// 节点列表(含耗时、退回标识、进行中标识、TOP3 排名)
    /// </summary>
    [HttpGet("nodes")]
    public Result<List<InstanceEfficiencyNodeVo>> Nodes([FromQuery] string processNumber)
    {
        return ResultHelper.Success(_processInstanceEfficiencyService.ListNodes(processNumber));
    }

    /// <summary>
    /// 节点详情(最后一轮人员明细 + 签署信息)
    /// </summary>
    [HttpGet("nodeDetail")]
    public Result<InstanceEfficiencyDetailVo> NodeDetail([FromQuery] string processNumber, [FromQuery] string taskDefKey)
    {
        return ResultHelper.Success(_processInstanceEfficiencyService.GetNodeDetail(processNumber, taskDefKey));
    }
}
