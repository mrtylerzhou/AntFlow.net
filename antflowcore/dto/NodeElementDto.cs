using antflowcore.vo;

namespace antflowcore.dto;

public class NodeElementDto
{
    public string NodeId { get; set; }
    public string ElementId { get; set; }
    public List<BaseIdTranStruVo> AssigneeInfoList { get; set; }
    public bool IsSingle { get; set; }
}