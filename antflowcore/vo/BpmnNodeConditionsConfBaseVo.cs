using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace antflowcore.vo
{
    public class BpmnNodeConditionsConfBaseVo
    {
        [JsonPropertyName("conditionParamTypes")]
        public List<int> ConditionParamTypes { get; set; }

        [JsonPropertyName("isDefault")]
        public int? IsDefault { get; set; }

        [JsonPropertyName("sort")]
        public int? Sort { get; set; }

        [JsonPropertyName("templateMarksList")]
        public List<BaseIdTranStruVo> TemplateMarksList { get; set; }

        [JsonPropertyName("jobLevelVo")]
        public BaseIdTranStruVo JobLevelVo { get; set; }

        [JsonPropertyName("parkArea")]
        public double? ParkArea { get; set; }

        [JsonPropertyName("accountType")]
        public List<int> AccountType { get; set; }

        [JsonPropertyName("accountTypeList")]
        public List<BaseIdTranStruVo> AccountTypeList { get; set; }

        [JsonPropertyName("planProcurementTotalMoney")]
        public double? PlanProcurementTotalMoney { get; set; }

        [JsonPropertyName("purchaseType")]
        public List<int> PurchaseType { get; set; }

        [JsonPropertyName("purchaseTypeList")]
        public List<BaseIdTranStruVo> PurchaseTypeList { get; set; }

        [JsonPropertyName("totalMoney")]
        public string TotalMoney { get; set; }

        [JsonPropertyName("outTotalMoney")]
        public string OutTotalMoney { get; set; }

        [JsonPropertyName("leaveHour")]
        public double? LeaveHour { get; set; }

        [JsonPropertyName("numberOperator")]
        public int? NumberOperator { get; set; }

        [JsonPropertyName("extJson")]
        public string ExtJson { get; set; }

        [JsonPropertyName("outSideConditionsJson")]
        public string OutSideConditionsJson { get; set; }

        [JsonPropertyName("outSideConditionsId")]
        public string OutSideConditionsId { get; set; }

        [JsonPropertyName("outSideConditionsUrl")]
        public string OutSideConditionsUrl { get; set; }

        [JsonPropertyName("outSideMatched")]
        public bool? OutSideMatched { get; set; }

        [JsonPropertyName("templateMarks")]
        public List<int> TemplateMarks { get; set; }

        [JsonPropertyName("lfConditions")]
        public Dictionary<string, object> LfConditions { get; set; }
    }
}
