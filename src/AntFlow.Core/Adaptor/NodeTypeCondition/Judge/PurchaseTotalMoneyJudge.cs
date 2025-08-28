namespace AntFlow.Core.Adaptor.NodeTypeCondition.Judge;

public class PurchaseTotalMoneyJudge : AbstractBinaryComparableJudge
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