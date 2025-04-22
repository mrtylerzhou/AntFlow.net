using antflowcore.exception;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.nodetypecondition.judge;

public class PurchaseTotalMoneyJudge : AbstractComparableJudge
{
    private readonly ILogger<PurchaseTotalMoneyJudge> _logger;

    public PurchaseTotalMoneyJudge(ILogger<PurchaseTotalMoneyJudge> logger)
    {
        _logger = logger;
    }

    public override bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo)
    {
        if (conditionsConf.PlanProcurementTotalMoney == null || bpmnStartConditionsVo.PlanProcurementTotalMoney == null)
        {
            _logger.LogInformation("process's Plan Procurement Total money is empty");
            throw new AFBizException("999", "process's Plan Procurement Total is empty");
        }
        decimal purchaseInDb = Convert.ToDecimal(conditionsConf.PlanProcurementTotalMoney);
        decimal purchaseActual = Convert.ToDecimal(bpmnStartConditionsVo.PlanProcurementTotalMoney);

        //operator type
        int? theOperatorType = conditionsConf.NumberOperator;
        if (theOperatorType == null)
        {
            throw new AFBizException("999", "number operator is empty");
        }
        return base.CompareJudge(purchaseInDb, purchaseActual, theOperatorType.Value);
    }
}