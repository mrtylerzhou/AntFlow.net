namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a quick entry type configuration.
    /// </summary>
    public class QuickEntryType
    {
        public long Id { get; set; }
        public long QuickEntryId { get; set; }
        public int Type { get; set; }
        public int IsDel { get; set; }
        public string TenantId { get; set; }
        public DateTime? CreateTime { get; set; } = DateTime.Now;
        public string TypeName { get; set; }

        public QuickEntryType() { }
    }
}