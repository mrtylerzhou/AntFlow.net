namespace AntFlow.Core.Adaptor.NodeTypeCondition.Judge;

public class AskLeaveJudge : AbstractBinaryComparableJudge
{
    protected override string FieldNameInDb()
    {
        return "LeaveHour";
    }

    protected override string FieldNameInStartConditions()
    {
        return "LeaveHour";
    }
}