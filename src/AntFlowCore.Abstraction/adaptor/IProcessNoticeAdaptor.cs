
using AntFlowCore.Base.vo;

namespace AntFlowCore.Abstraction.adaptor;

public interface IProcessNoticeAdaptor
{
    void SendMessageBatchByType(List<UserMsgVo> userMsgVos);
     int GetSupportCode();
}