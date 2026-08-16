using System.Text.Json.Serialization;

namespace AntFlowCore.Base.dto
{
    /// <summary>
    /// 字典管理列表查询请求. 对应 Java DictDataPageReq.
    /// </summary>
    public class DictDataPageReq
    {
        [JsonPropertyName("pageDto")]
        public PageDto PageDto { get; set; }

        /// <summary>
        /// 字典类型(精确筛选, lowcodeflow/udr/processlabel)
        /// </summary>
        [JsonPropertyName("dictType")]
        public string DictType { get; set; }

        /// <summary>
        /// 关键字(字典标签/字典键值 模糊匹配)
        /// </summary>
        [JsonPropertyName("keyword")]
        public string Keyword { get; set; }
    }
}