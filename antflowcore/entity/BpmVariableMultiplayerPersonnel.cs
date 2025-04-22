using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entities
{
    [Table(Name = "t_bpm_variable_multiplayer_personnel")]
    public class BpmVariableMultiplayerPersonnel
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }

        /// <summary>
        /// 变量多方ID
        /// </summary>
        [Column(Name = "variable_multiplayer_id")]
        public long VariableMultiplayerId { get; set; }

        /// <summary>
        /// 被指派人
        /// </summary>
        public string Assignee { get; set; }

        /// <summary>
        /// 被指派人姓名
        /// </summary>
        [Column(Name = "assignee_name")]
        public string AssigneeName { get; set; }

        /// <summary>
        /// 是否承担，0表示否，1表示是
        /// </summary>
        [Column(Name = "undertake_status")]
        public int? UndertakeStatus { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 是否删除，0表示否，1表示是
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新用户
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column(Name = "update_time",IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }
        
    }
}
