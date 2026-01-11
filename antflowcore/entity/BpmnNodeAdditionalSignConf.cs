namespace antflowcore.entity;

using System;


/// <summary>
/// 节点附加签核配置
/// </summary>
public class BpmnNodeAdditionalSignConf
{
    /// <summary>
    /// id
    /// </summary>
   
    public long? Id { get; set; }

    /// <summary>
    /// 节点id
    /// </summary>
    public long? BpmnNodeId { get; set; }

    /// <summary>
    /// additional sign id stored as json value
    /// </summary>
   
 
    public string SignInfos { get; set; } = string.Empty;

    /// <summary>
    /// see NodePropertyEnum
    /// </summary>
   
    public int? SignProperty { get; set; }

    /// <summary>
    /// 1 for add, 2 for exclude
    /// </summary>
   
    public int? SignPropertyType { get; set; }

    /// <summary>
    /// sign type 1: all sign 2: or sign, it is meaning less for additional sign, it will inherit from parent
    /// </summary>
    public int? SignType { get; set; }

    /// <summary>
    /// remark
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 0: no, 1: yes
    /// </summary>
    public int? IsDel { get; set; }

    /// <summary>
    /// tenantId
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// as its name says
    /// </summary>
    public string? CreateUser { get; set; }

    /// <summary>
    /// as its name says
    /// </summary>
    public DateTime? CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// as its name says
    /// </summary>
    public string? UpdateUser { get; set; }

    /// <summary>
    /// as its name says
    /// </summary>
    public DateTime? UpdateTime { get; set; } = DateTime.Now;
}