using AntFlowCore.Vo;

namespace antflowcore.vo;

public class ThirdPartyAccountApplyVo: BusinessDataVo
{
    public int AccountType { get; set; }
    public String AccountOwnerName { get; set; }
    public String Remark { get; set; }
}