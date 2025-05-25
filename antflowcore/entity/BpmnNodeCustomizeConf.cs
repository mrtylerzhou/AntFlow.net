using FreeSql.DataAnnotations;

namespace antflowcore.entity;

/// <summary>
/// hrpb config entity
/// you may notice that where there is an approvement strategy,
/// there is a table behind to support this business
/// @since 0.5
/// </summary>
[Table(Name = "t_bpmn_node_customize_conf")]
public class BpmnNodeCustomizeConf
{
    /// <summary>
    /// auto incr id
    /// </summary>
    [Column(IsIdentity = true, IsPrimary = true)]
    public long Id { get; set; }

    /// <summary>
    /// node id
    /// </summary>
    [Column(Name = "bpmn_node_id")]
    public long BpmnNodeId { get; set; }

    /// <summary>
    /// sign type 1 all sign 2 or sign
    /// </summary>
    [Column(Name = "sign_type")]
    public int? SignType { get; set; }

    /// <summary>
    /// remark
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 0 for normal, 1 for deleted
    /// </summary>
    [Column(Name = "is_del")]
    public int IsDel { get; set; }

    /// <summary>
    /// create user
    /// </summary>
    [Column(Name = "create_user")]
    public string CreateUser { get; set; }

    /// <summary>
    /// create time
    /// </summary>
    [Column(Name = "create_time")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// update user
    /// </summary>
    [Column(Name = "update_user")]
    public string UpdateUser { get; set; }

    /// <summary>
    /// update time
    /// </summary>
    [Column(Name = "update_time")]
    public DateTime UpdateTime { get; set; }
}