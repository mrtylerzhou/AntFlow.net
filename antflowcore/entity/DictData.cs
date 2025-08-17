using System;

namespace antflowcore.entity
{
    public class DictData
    {
        public long Id { get; set; }
        public int? Sort { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public string DictType { get; set; }
        public string CssClass { get; set; }
        public string ListClass { get; set; }
        public string IsDefault { get; set; }
        public int? IsDel { get; set; }
        public string TenantId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateUser { get; set; }
        public string Remark { get; set; }
    }
}