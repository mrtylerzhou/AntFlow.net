using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnprocessnotice;

public interface IProcessNoticeAdaptor
{
    void SendMessageBatchByType(List<UserMsgVo> userMsgVos);
     int GetSupportCode();
}