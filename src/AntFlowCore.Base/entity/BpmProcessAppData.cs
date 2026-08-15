namespace AntFlowCore.Base.entity;


/// <summary>
/// Represents the process data for the app.
/// </summary>
public class BpmProcessAppData
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Process key.
    /// </summary>
    public string ProcessKey { get; set; }

    /// <summary>
    /// Process name.
    /// </summary>
    public string ProcessName { get; set; }

    /// <summary>
    /// Online status (0 for no, 1 for yes).
    /// </summary>
    public int State { get; set; }

    /// <summary>
    /// APP route.
    /// </summary>
    public string Route { get; set; }

    /// <summary>
    /// Sort order.
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// Picture source route.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Is for all (0 for no, 1 for yes).
    /// </summary>
    public int IsAll { get; set; }

    /// <summary>
    /// Version ID.
    /// </summary>
    public long VersionId { get; set; }

    /// <summary>
    /// Application ID(与Java String一致: type=1/3存对象id字符串, type=2存formCode).
    /// </summary>
    public string ApplicationId { get; set; }

    /// <summary>
    /// Type (1 for version app, 2 for app data, 3 for quick entry).
    /// </summary>
    public int Type { get; set; }
}