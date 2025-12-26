using System;

namespace antflowcore.entity;

public class BpmBusinessProcess
{
    public static readonly int VERSION_DEFAULT_0 = 0;
    public static readonly int VERSION_1 = 1;

    /// <summary>
    /// Process ID (Primary Key)
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Process key
    /// </summary>
    public string ProcessinessKey { get; set; }

    /// <summary>
    /// Business ID
    /// </summary>
    public string BusinessId { get; set; }

    /// <summary>
    /// Business Number
    /// </summary>
    public string BusinessNumber { get; set; }

    /// <summary>
    /// Entry ID
    /// </summary>
    public string EntryId { get; set; }

    /// <summary>
    /// Process version
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Create time
    /// </summary>
    public DateTime? CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Update time
    /// </summary>
    public DateTime? UpdateTime { get; set; }=DateTime.Now;

    /// <summary>
    /// Process description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Process state: 1=approved, 2=approving, 3=canceled
    /// </summary>
    public int ProcessState { get; set; }

    /// <summary>
    /// Created by user
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Process digest
    /// </summary>
    public string ProcessDigest { get; set; }

    /// <summary>
    /// Is deleted: 0=no, 1=yes
    /// </summary>
    public int IsDel { get; set; }
    public string TenantId { get; set; }
    /// <summary>
    /// Data source ID (no meaning at the moment)
    /// </summary>
    public long? DataSourceId { get; set; }

    /// <summary>
    /// Process instance ID (important for linking business and Activiti processes)
    /// </summary>
    public string ProcInstId { get; set; }

    /// <summary>
    /// Back to user ID
    /// </summary>
    public string BackUserId { get; set; }

    /// <summary>
    /// Is it an external process: 0=no, 1=yes
    /// </summary>
    public int IsOutSideProcess { get; set; }

    /// <summary>
    /// Is it a low-code flow: 0=no, 1=yes
    /// </summary>
    public int IsLowCodeFlow { get; set; }
}
