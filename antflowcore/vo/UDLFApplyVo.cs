using AntFlowCore.Vo;

namespace antflowcore.vo;

public class UDLFApplyVo: BusinessDataVo
{
    public String remark { get; set; }
    public Dictionary<String,Object> lfFields { get; set; }
    public String lfFormData { get; set; }
}