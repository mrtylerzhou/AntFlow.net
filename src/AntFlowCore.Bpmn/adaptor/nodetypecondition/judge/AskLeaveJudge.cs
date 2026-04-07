namespace AntFlowCore.Bpmn.adaptor.nodetypecondition.judge;

public class AskLeaveJudge: AbstractBinaryComparableJudge
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