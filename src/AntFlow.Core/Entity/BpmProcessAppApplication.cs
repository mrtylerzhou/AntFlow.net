namespace AntFlow.Core.Entity;

/// <summary>
///     Represents the BPM process application.
/// </summary>
public class BpmProcessAppApplication
{
    public int Id { get; set; }

    /// <summary>
    ///     Business code (default is empty for the main program).
    /// </summary>
    public string BusinessCode { get; set; }

    /// <summary>
    ///     Application name.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Application type (1: process, 2: app, 3: parent app).
    /// </summary>
    public int ApplyType { get; set; }

    /// <summary>
    ///     Icon for PC.
    /// </summary>
    public string PcIcon { get; set; }

    /// <summary>
    ///     Icon for mobile platform.
    /// </summary>
    public string EffectiveSource { get; set; }

    /// <summary>
    ///     Is child app (0: no, 1: yes).
    /// </summary>
    public int IsSon { get; set; }

    /// <summary>
    ///     View URL.
    /// </summary>
    public string LookUrl { get; set; }

    /// <summary>
    ///     Submit URL.
    /// </summary>
    public string SubmitUrl { get; set; }

    /// <summary>
    ///     Condition URL.
    /// </summary>
    public string ConditionUrl { get; set; }

    /// <summary>
    ///     Parent app ID.
    /// </summary>
    public int ParentId { get; set; }

    /// <summary>
    ///     Application URL.
    /// </summary>
    public string ApplicationUrl { get; set; }

    /// <summary>
    ///     App route.
    /// </summary>
    public string Route { get; set; }

    /// <summary>
    ///     Process key.
    /// </summary>
    public string ProcessKey { get; set; }

    /// <summary>
    ///     Permission code.
    /// </summary>
    public string PermissionsCode { get; set; }

    /// <summary>
    ///     Deletion status: 0 for not deleted, 1 for deleted.
    /// </summary>
    public int IsDel { get; set; }

    /// <summary>
    ///     Creator's user ID.
    /// </summary>
    public string CreateUserId { get; set; }

    /// <summary>
    ///     Creation time.
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    ///     Last update user.
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    ///     Last update time.
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    ///     Is for all (page with no configuration function, default writes to database).
    /// </summary>
    public int IsAll { get; set; }

    /// <summary>
    ///     Process state: 0 for no, 1 for yes.
    /// </summary>
    public int State { get; set; }

    /// <summary>
    ///     Sort order.
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    ///     Source.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    ///     User request URI.
    /// </summary>
    public string UserRequestUri { get; set; }

    /// <summary>
    ///     Role request URI.
    /// </summary>
    public string RoleRequestUri { get; set; }
}