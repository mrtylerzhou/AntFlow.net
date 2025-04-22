using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the process data for the app.
    /// </summary>
    [Table(Name = "bpm_process_app_data")]
    public class BpmProcessAppData
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Process key.
        /// </summary>
        [Column(Name = "process_key")]
        public string ProcessKey { get; set; }

        /// <summary>
        /// Process name.
        /// </summary>
        [Column(Name = "process_name")]
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
        [Column(Name = "is_all")]
        public int IsAll { get; set; }

        /// <summary>
        /// Version ID.
        /// </summary>
        [Column(Name = "version_id")]
        public long VersionId { get; set; }

        /// <summary>
        /// Application ID.
        /// </summary>
        [Column(Name = "application_id")]
        public long ApplicationId { get; set; }

        /// <summary>
        /// Type (1 for version app, 2 for app data).
        /// </summary>
        [Column(Name = "type")]
        public int Type { get; set; }
    }
}