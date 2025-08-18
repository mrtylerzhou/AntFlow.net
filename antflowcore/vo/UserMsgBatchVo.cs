using antflowcore.constant.enums;
using AntFlowCore.Vo;

namespace antflowcore.vo;

public class UserMsgBatchVo
{
   
    public UserMsgVo UserMsgVo { get; set; }

   
    public List<MessageSendTypeEnum> MessageSendTypeEnums{ get; set;}
}