using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a BPM process notice.
    /// </summary>
    [Table(Name = "bpm_process_notice")]
    public class BpmProcessNotice
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Notice type:
        /// 1: Mail
        /// 2: SMS
        /// 3: App
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Process key.
        /// </summary>
        [Column(Name = "process_key")]
        public string ProcessKey { get; set; }
    }
}