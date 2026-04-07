
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.bpmnprocessnotice;

public interface IProcessNoticeAdaptor
{
    void SendMessageBatchByType(List<UserMsgVo> userMsgVos);
     int GetSupportCode();
}