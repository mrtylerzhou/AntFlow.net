using antflowcore.exception;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.nodetypecondition.judge;

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