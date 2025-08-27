namespace AntFlow.Core.Entity;

public class Employee
{
    public string Id { get; set; }
    public string Username { get; set; }

    /// <summary>
    ///     direct leader id
    /// </summary>
    public long LeaderId { get; set; }

    /// <summary>
    ///     email, for notification
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    ///     mobile
    /// </summary>
    public string Mobile { get; set; }

    public int IsDel { get; set; }

    /// <summary>
    ///     hrbp id
    /// </summary>
    public int HrbpId { get; set; }

    /// <summary>
    ///     avatar
    /// </summary>
    public string HeadImg { get; set; }

    /// <summary>
    ///     is show mobile, you should respect user's privacy, if he or she do not want to show his or her mobile, you should
    ///     set it false
    /// </summary>
    public bool MobileIsShow { get; set; }

    public string Path { get; set; }

    public int DepartmentId { get; set; }
}