using AntFlowCore.Vo;

namespace antflowcore.vo;

public class UDLFApplyVo: BusinessDataVo
{
    public String Remark { get; set; }
    public Dictionary<String,Object> lfFields { get; set; }
    public String LfFormData { get; set; }
}