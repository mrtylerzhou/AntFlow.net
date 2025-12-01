using antflowcore.vo;

namespace antflowcore.adaptor.personnel;

public interface IMissAssigneeProcessing
{
    BaseIdTranStruVo ProcessMissAssignee(int? processingWay);
}