namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the system version configuration.
    /// </summary>
    public class SysVersion
    {
        public static readonly int HIDE_STATUS_0 = 0;
        public static readonly int HIDE_STATUS_1 = 1;

        public long Id { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int IsDel { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public int Index { get; set; }
        public int IsForce { get; set; }
        public string AndroidUrl { get; set; }
        public string IosUrl { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public int IsHide { get; set; }
        public string DownloadCode { get; set; }
        public DateTime? EffectiveTime { get; set; }

        public SysVersion() { }
    }
}