using System.Text.Json.Serialization;

namespace AntFlowCore.Base.entity.jsonconf;

public class AppCategoryConfigJson
{
    [JsonPropertyName("categories")]
    public List<CategoryItem>? Categories { get; set; }

    public class CategoryItem
    {
        [JsonPropertyName("categoryId")]
        public long? CategoryId { get; set; }

        [JsonPropertyName("sort")]
        public int? Sort { get; set; }

        [JsonPropertyName("state")]
        public int? State { get; set; }

        [JsonPropertyName("visbleState")]
        public int? VisbleState { get; set; }

        [JsonPropertyName("historyId")]
        public long? HistoryId { get; set; }

        [JsonPropertyName("commonUseState")]
        public int? CommonUseState { get; set; }
    }
}
