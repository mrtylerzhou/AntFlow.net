

using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Bpmn.adaptor.personnel;

public interface IMissAssigneeProcessing
{
    BaseIdTranStruVo ProcessMissAssignee(int? processingWay);
}