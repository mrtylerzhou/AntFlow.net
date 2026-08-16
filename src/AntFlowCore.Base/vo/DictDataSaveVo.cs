using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo
{
    /// <summary>
    /// 字典数据 新增/编辑 请求. 对应 Java DictDataSaveVo.
    /// </summary>
    public class DictDataSaveVo
    {
        /// <summary>
        /// 主键(编辑时必传)
        /// </summary>
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        /// <summary>
        /// 字典标签(必填)
        /// </summary>
        [JsonPropertyName("dictLabel")]
        public string DictLabel { get; set; }

        /// <summary>
        /// 字典键值(必填)
        /// </summary>
        [JsonPropertyName("dictValue")]
        public string DictValue { get; set; }

        /// <summary>
        /// 字典类型(必填, 仅 udr/processlabel 可选, lowcodeflow 禁止)
        /// </summary>
        [JsonPropertyName("dictType")]
        public string DictType { get; set; }

        /// <summary>
        /// 字典排序(选填, 默认 0)
        /// </summary>
        [JsonPropertyName("sort")]
        public int? Sort { get; set; }

        /// <summary>
        /// 备注(选填)
        /// </summary>
        [JsonPropertyName("remark")]
        public string Remark { get; set; }
    }
}