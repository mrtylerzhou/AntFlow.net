using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IActivitiBpmMsgTemplateService
{
    void SendBpmCustomMsg(ActivitiBpmMsgVo activitiBpmMsgVo, string content);
    Task SendBpmApprovalMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo);
    Task SendBpmApprovalMsgBatchAsync(List<ActivitiBpmMsgVo> activitiBpmMsgVos);
    Task SendBpmForwardedMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo);
    Task SendBpmForwardedMsgBatchAsync(List<ActivitiBpmMsgVo> activitiBpmMsgVos);
    Task SendBpmFinishMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo);
    Task SendBpmFinishMsgBatchAsync(List<ActivitiBpmMsgVo> activitiBpmMsgVos);
    Task SendBpmRejectMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo);
    Task SendBpmRejectMsgBatchAsync(List<ActivitiBpmMsgVo> activitiBpmMsgVos);
    Task SendBpmTerminationMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo);
    Task SendBpmTerminationMsgBatchAsync(List<ActivitiBpmMsgVo> activitiBpmMsgVos);
    Task SendBpmGenerationApprovalMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo);
    Task SendBpmGenerationApprovalMsgBatchAsync(List<ActivitiBpmMsgVo> activitiBpmMsgVos);
    Task SendBpmChangePersonOrgiMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo);
    Task SendBpmChangePersonNewMsgAsync(ActivitiBpmMsgVo activitiBpmMsgVo);
    string ReplaceTemplateDetail(ActivitiBpmMsgVo activitiBpmMsgVo, string content);
    void SendBpmApprovalMsg(ActivitiBpmMsgVo activitiBpmMsgVo);
}
