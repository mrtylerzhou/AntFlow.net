using AntFlow.Core.Entity;

namespace AntFlow.Core.Vo;

public class ProcessTypeInforVo
{
    /// <summary>
    ///     Process business connection
    /// </summary>
    public BpmBusinessProcess BpmBusinessProcess { get; set; }

    /// <summary>
    ///     Process key
    /// </summary>
    public string ProcessinessKey { get; set; }

    /// <summary>
    ///     Business number
    /// </summary>
    public string BusinessNumber { get; set; }

    /// <summary>
    ///     Receiver ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    ///     Process type
    /// </summary>
    public string ProcessType { get; set; }

    /// <summary>
    ///     Process name
    /// </summary>
    public string ProcessName { get; set; }

    /// <summary>
    ///     Other user ID
    /// </summary>
    public string OtherUserId { get; set; }

    /// <summary>
    ///     Carbon copy
    /// </summary>
    public string[] Cc { get; set; }

    /// <summary>
    ///     Email URL
    /// </summary>
    public string EmailUrl { get; set; }

    /// <summary>
    ///     In-site message URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    ///     App push URL
    /// </summary>
    public string AppPushUrl { get; set; }

    /// <summary>
    ///     Task ID
    /// </summary>
    public string TaskId { get; set; }

    /// <summary>
    ///     Process operation type
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    ///     Node ID
    /// </summary>
    public string NodeId { get; set; }

    /// <summary>
    ///     Form code
    /// </summary>
    public string FormCode { get; set; }
}