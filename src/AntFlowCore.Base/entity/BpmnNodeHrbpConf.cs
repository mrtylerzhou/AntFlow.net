using AntFlowCore.Core.util;

namespace AntFlowCore.Core.entity;

public class BpmnNodeHrbpConf
{
    public long Id { get; set; }
    public long BpmnNodeId { get; set; }
    public int? HrbpConfType { get; set; }
    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;
    public int IsDel { get; set; }
    public string TenantId { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; } = DateTime.Now;
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }=DateTime.Now;
}