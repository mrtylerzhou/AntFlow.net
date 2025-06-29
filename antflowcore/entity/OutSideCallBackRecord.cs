namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents an external callback record.
    /// </summary>
    public class OutSideCallBackRecord
    {
        public int Id { get; set; }
        public string ProcessNumber { get; set; }
        public int Status { get; set; }
        public int RetryTimes { get; set; }
        public int ButtonOperationType { get; set; }
        public string CallBackTypeName { get; set; }
        public long BusinessId { get; set; }
        public string FormCode { get; set; }
        public int IsDel { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}