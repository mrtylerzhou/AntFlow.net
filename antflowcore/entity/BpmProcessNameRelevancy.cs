using System;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Process name relevancy
    /// </summary>
    [Table(Name = "bpm_process_name_relevancy")]
    public class BpmProcessNameRelevancy
    {
        /// <summary>
        /// 主键 ID
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// 流程名称 ID
        /// </summary>
        [Column(Name = "process_name_id")]
        public long ProcessNameId { get; set; }

        /// <summary>
        /// 流程编号
        /// </summary>
        [Column(Name = "process_key")]
        public string ProcessKey { get; set; }

        /// <summary>
        /// 删除标志
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }
    }
}