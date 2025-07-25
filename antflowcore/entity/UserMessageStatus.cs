﻿namespace antflowcore.entity
{
    /// <summary>
    /// 用户消息状态实体类
    /// </summary>
    public class UserMessageStatus
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public bool? MessageStatus { get; set; }

        public bool? MailStatus { get; set; }

        public bool? Shock { get; set; }

        public bool? Sound { get; set; }

        public bool? OpenPhone { get; set; }

        public bool? NotTrouble { get; set; }

        public DateTime? NotTroubleTimeBegin { get; set; }

        public DateTime? NotTroubleTimeEnd { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public string CreateUser { get; set; }

        public string UpdateUser { get; set; }
    }
}