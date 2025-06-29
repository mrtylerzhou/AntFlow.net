using System;

namespace AntFlowCore.Entity;

public class BpmnNodeButtonConf
{
    public long Id { get; set; }
    public long BpmnNodeId { get; set; }
    public int ButtonPageType { get; set; }
    public int ButtonType { get; set; }
    public string ButtonName { get; set; }
    public string Remark { get; set; } = "";
    public int IsDel { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; }
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }
}