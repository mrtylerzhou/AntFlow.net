using System;
using System.Runtime.Serialization;

namespace AntFlowCore.Vo
{
    public class DefaultTemplateVo
    {
        public long Id { get; set; }

        // Event type
        public int Event { get; set; }

        public string EventValue { get; set; }

        // Template ID
        public long? TemplateId { get; set; }

        public string TemplateName { get; set; }

        // 0 for normal, 1 for deleted
        public int IsDel { get; set; }

        // Create time
        public DateTime? CreateTime { get; set; }

        // Create user
        public string CreateUser { get; set; }

        // Update time
        public DateTime? UpdateTime { get; set; }

        // Update user
        public string UpdateUser { get; set; }
    }
}