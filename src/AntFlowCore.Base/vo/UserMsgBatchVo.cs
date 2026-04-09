using AntFlowCore.Base.constant.enums;

namespace AntFlowCore.Base.vo;

public class UserMsgBatchVo
{
   
    public UserMsgVo UserMsgVo { get; set; }

   
    public List<MessageSendTypeEnum> MessageSendTypeEnums{ get; set;}
}