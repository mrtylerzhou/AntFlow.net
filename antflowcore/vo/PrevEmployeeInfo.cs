using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class PrevEmployeeInfo
    {
        [JsonPropertyName("employeeId")]
        public int? EmployeeId { get; set; }

        [JsonPropertyName("employeeName")]
        public string EmployeeName { get; set; }

        [JsonPropertyName("jobLevelId")]
        public long? JobLevelId { get; set; }

        [JsonPropertyName("jobLevel")]
        public string JobLevel { get; set; }

        [JsonPropertyName("departName")]
        public string DepartName { get; set; }

        [JsonPropertyName("departId")]
        public int? DepartId { get; set; }
    }
}