using AntFlow.Core.Service.Business;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Util;

public class ActivitiTemplateMsgUtils
{
    private static ActivitiBpmMsgTemplateService GetMessageSendService()
    {
        return ServiceProviderUtils.GetService<ActivitiBpmMsgTemplateService>();
    }

    public static void SendBpmFinishMsgBatch(List<ActivitiBpmMsgVo> activitiBpmMsgVos)
    {
        GetMessageSendService().SendBpmFinishMsgBatchAsync(activitiBpmMsgVos);
    }

    public static void SendBpmRejectMsg(ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        GetMessageSendService().SendBpmRejectMsgAsync(activitiBpmMsgVo);
    }

    public void SendBpmRejectMsgBatch(List<ActivitiBpmMsgVo> activitiBpmMsgVos)
    {
        GetMessageSendService().SendBpmRejectMsgBatchAsync(activitiBpmMsgVos);
    }

    public static void SendBpmTerminationMsg(ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        GetMessageSendService().SendBpmTerminationMsgAsync(activitiBpmMsgVo);
    }

    public static void SendBpmTerminationMsgBatch(List<ActivitiBpmMsgVo> activitiBpmMsgVos)
    {
        GetMessageSendService().SendBpmTerminationMsgBatchAsync(activitiBpmMsgVos);
    }

    public static void SendBpmGenerationApprovalMsg(ActivitiBpmMsgVo activitiBpmMsgVo)
    {
        GetMessageSendService().SendBpmGenerationApprovalMsgAsync(activitiBpmMsgVo);
    }

    public static void SendBpmGenerationApprovalMsgBatch(List<ActivitiBpmMsgVo> activitiBpmMsgVos)
    {
        GetMessageSendService().SendBpmGenerationApprovalMsgBatchAsync(activitiBpmMsgVos);
    }

    public static void SendBpmChangePerson(ActivitiBpmMsgVo activitiBpmMsgVoOrgi, ActivitiBpmMsgVo activitiBpmMsgVoNew)
    {
        ActivitiBpmMsgTemplateService? service = GetMessageSendService();
        service.SendBpmChangePersonOrgiMsgAsync(activitiBpmMsgVoOrgi);
        service.SendBpmChangePersonNewMsgAsync(activitiBpmMsgVoNew);
    }
}