using antflowcore.exception;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.nodetypecondition.judge;

public class AskLeaveJudgeService: AbstractComparableJudge
{
    private readonly ILogger<AskLeaveJudgeService> _logger;

    public AskLeaveJudgeService(ILogger<AskLeaveJudgeService> logger)
    {
        _logger = logger;
    }

    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo)
    {
        if (conditionsConf.LeaveHour == null || bpmnStartConditionsVo.LeaveHour == null)
        {
            _logger.LogInformation("Process's total leave hours are empty");
            throw new AFBizException("999", "Process's total leave hours are empty");
        }

        // Convert leave hours to decimal
        decimal leaveHourInDb = Convert.ToDecimal(conditionsConf.LeaveHour);
        decimal leaveHourActual = Convert.ToDecimal(bpmnStartConditionsVo.LeaveHour);

        // Operator type
        int theOperatorType = conditionsConf.NumberOperator ?? throw new AFBizException("999", "Operator type is null");

        return CompareJudge(leaveHourInDb, leaveHourActual, theOperatorType);
    }
}