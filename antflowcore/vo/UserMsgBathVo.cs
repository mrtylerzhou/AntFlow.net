using antflowcore.constant.enums;
using AntFlowCore.Vo;

namespace antflowcore.vo;

public class UserMsgBathVo
{
   
    public UserMsgVo UserMsgVo { get; set; }

   
    public List<MessageSendTypeEnum> MessageSendTypeEnums{ get; set;}
}