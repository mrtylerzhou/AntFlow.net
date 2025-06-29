using System;

namespace AntFlowCore.Entity;

public class BpmnNodeLfFormdataFieldControl
{
    public long Id { get; set; }
    public long NodeId { get; set; }
    public long? FormdataId { get; set; }
    public string FieldId { get; set; }
    public string FieldName { get; set; }
    public string Perm { get; set; }
    public int IsDel { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; }
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }
}