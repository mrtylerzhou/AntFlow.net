using AntFlowCore.Core.constant.enums;
using AntFlowCore.Vo;

namespace AntFlowCore.Core.vo;

public class UserMsgBatchVo
{
   
    public UserMsgVo UserMsgVo { get; set; }

   
    public List<MessageSendTypeEnum> MessageSendTypeEnums{ get; set;}
}