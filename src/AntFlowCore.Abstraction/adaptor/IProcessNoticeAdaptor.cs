
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.adaptor;

public interface IProcessNoticeAdaptor
{
    void SendMessageBatchByType(List<UserMsgVo> userMsgVos);
     int GetSupportCode();
}