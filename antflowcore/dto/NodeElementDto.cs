using antflowcore.vo;

namespace antflowcore.dto;

public class NodeElementDto
{
    public string NodeId { get; set; }
    public string ElementId { get; set; }
    public List<BaseInfoTranStructVo> AssigneeInfoList { get; set; }
    public bool IsSingle { get; set; }
    public string VarName{ get; set; }
}