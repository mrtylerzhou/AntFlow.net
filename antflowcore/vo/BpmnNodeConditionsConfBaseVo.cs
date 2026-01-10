using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace antflowcore.vo
{
    public class BpmnNodeConditionsConfBaseVo
    {
        [JsonPropertyName("conditionParamTypes")]
        public List<int> ConditionParamTypes { get; set; }

        [JsonPropertyName("groupedConditionParamTypes")]
        public IDictionary<int,List<int>>GroupedConditionParamTypes { get; set; }
        
        [JsonPropertyName("isDefault")]
        public int? IsDefault { get; set; }

        [JsonPropertyName("sort")]
        public int? Sort { get; set; }
        
        [JsonPropertyName("groupRelation")]
        public int? GroupRelation { get; set; }

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
        public string LeaveHour { get; set; }

        [JsonPropertyName("numberOperator")]
        public int? NumberOperator { get; set; }

        [JsonPropertyName("numberOperatorList")]
        public List<int> NumberOperatorList { get; set; } = new List<int>();
        
        [JsonPropertyName("groupedNumberOperatorListMap")]
        public IDictionary<int,List<int>> GroupedNumberOperatorListMap { get; set; }=new Dictionary<int, List<int>>();
        [JsonPropertyName("groupedCondRelations")]
        public IDictionary<int,int> GroupedCondRelations { get; set; }=new Dictionary<int, int>();
        
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
        public IDictionary<string, object> LfConditions { get; set; }
        
        [JsonPropertyName("groupedLfConditionsMap")]
        public IDictionary<int,IDictionary<string,object>>? GroupedLfConditionsMap { get; set; }
    }
}
