using AntFlow.Core.Constant.Enums;

namespace AntFlow.Core.Vo;

public class UserMsgBatchVo
{
    public UserMsgVo UserMsgVo { get; set; }


    public List<MessageSendTypeEnum> MessageSendTypeEnums { get; set; }
}