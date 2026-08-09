using System.Globalization;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.nodetypecondition.judge;

/// <summary>
/// 退回次数条件判断器.
/// 从 conditionsConf.ReturnCount 读取用户配置的阈值(单值如"3";区间如"1,5"),
/// 从 bpm_business_process.return_count 读取流程实例的退回次数,
/// 使用 AbstractComparableJudge 提供的 9 种运算符进行比较.
/// </summary>
public class ReturnCountJudge : AbstractComparableJudge
{
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly ILogger<ReturnCountJudge> _logger;

    public ReturnCountJudge(IBpmBusinessProcessService bpmBusinessProcessService, ILogger<ReturnCountJudge> logger)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _logger = logger;
    }

    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo, int group, int index)
    {
        string returnCountConf = conditionsConf.ReturnCount;
        if (string.IsNullOrWhiteSpace(returnCountConf))
        {
            return false;
        }
        int? operatorType = conditionsConf.NumberOperator;
        if (operatorType == null)
        {
            _logger.LogWarning("returnCount condition has no NumberOperator, nodeId={NodeId}", nodeId);
            return false;
        }

        // 读取运行时退回次数
        string processNum = bpmnStartConditionsVo.ProcessNum;
        if (string.IsNullOrWhiteSpace(processNum))
        {
            return false;
        }
        BpmBusinessProcess process;
        try
        {
            process = _bpmBusinessProcessService.GetBpmBusinessProcess(processNum);
        }
        catch (Exception ex)
        {
            // 流程提交阶段 BpmBusinessProcess 尚未创建,退回次数条件主要用于动态条件(运行时评估)
            _logger.LogDebug("returnCount judge: BpmBusinessProcess not found, processNum={ProcessNum}, msg={Msg}", processNum, ex.Message);
            return false;
        }
        int actualCount = process.ReturnCount;

        // 解析阈值
        decimal confTotal;
        decimal? confTotal2 = null;
        try
        {
            if (JudgeOperatorEnum.BinaryOperator().Contains(operatorType.Value))
            {
                string[] parts = returnCountConf.Split(',');
                confTotal = decimal.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
                confTotal2 = decimal.Parse(parts[1].Trim(), CultureInfo.InvariantCulture);
            }
            else
            {
                confTotal = decimal.Parse(returnCountConf.Trim(), CultureInfo.InvariantCulture);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "returnCount condition parse failed, conf={Conf}, nodeId={NodeId}", returnCountConf, nodeId);
            return false;
        }

        decimal actual = new decimal(actualCount);
        return CompareJudge(confTotal, confTotal2, actual, operatorType);
    }
}
