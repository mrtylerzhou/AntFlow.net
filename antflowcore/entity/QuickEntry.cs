namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a quick entry configuration.
    /// </summary>
    public class QuickEntry
    {
        public const int VARIABLE_URL_FLAG_TRUE = 1;
        public const int VARIABLE_URL_FLAG_FALSE = 0;

        public int Id { get; set; }
        public string Title { get; set; }
        public string EffectiveSource { get; set; }
        public int IsDel { get; set; }
        public int? TenantId { get; set; }
        public string Route { get; set; }
        public int Sort { get; set; }
        public DateTime? CreateTime { get; set; }
        public int Status { get; set; }
        public int VariableUrlFlag { get; set; }

        public QuickEntry() { }
    }
}