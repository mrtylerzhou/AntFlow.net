using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entities
{
    /// <summary>
    /// 用户消息状态实体类
    /// </summary>
    [Table(Name = "t_user_message_status")]
    public class UserMessageStatus
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Column(Name = "user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// 消息状态
        /// </summary>
        [Column(Name = "message_status")]
        public bool? MessageStatus { get; set; }

        /// <summary>
        /// 邮件状态
        /// </summary>
        [Column(Name = "mail_status")]
        public bool? MailStatus { get; set; }

        /// <summary>
        /// 是否开启震动
        /// </summary>
        public bool? Shock { get; set; }

        /// <summary>
        /// 是否开启声音
        /// </summary>
        public bool? Sound { get; set; }

        /// <summary>
        /// 是否公开手机号
        /// </summary>
        [Column(Name = "open_phone")]
        public bool? OpenPhone { get; set; }

        /// <summary>
        /// 是否开启勿扰模式
        /// </summary>
        [Column(Name = "not_trouble")]
        public bool? NotTrouble { get; set; }

        /// <summary>
        /// 勿扰模式开始时间
        /// </summary>
        [Column(Name = "not_trouble_time_begin")]
        public DateTime? NotTroubleTimeBegin { get; set; }

        /// <summary>
        /// 勿扰模式结束时间
        /// </summary>
        [Column(Name = "not_trouble_time_end")]
        public DateTime? NotTroubleTimeEnd { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// 更新用户
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }
    }
}
