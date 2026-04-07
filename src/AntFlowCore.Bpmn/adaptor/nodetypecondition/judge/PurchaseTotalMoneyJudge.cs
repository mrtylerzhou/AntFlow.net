namespace AntFlowCore.Bpmn.adaptor.nodetypecondition.judge;

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