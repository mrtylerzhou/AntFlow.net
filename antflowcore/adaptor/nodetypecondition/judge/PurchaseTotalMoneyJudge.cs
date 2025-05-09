﻿using antflowcore.exception;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.nodetypecondition.judge;

public class PurchaseTotalMoneyJudge: AbstractBinaryComparableJudge
{
    protected override string FieldNameInDb()
    {
        return "PlanProcurementTotalMoney";
    }

    protected override string FieldNameInStartConditions()
    {
        return "PlanProcurementTotalMoney";
    }
}