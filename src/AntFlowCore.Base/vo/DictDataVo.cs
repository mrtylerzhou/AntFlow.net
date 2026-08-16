using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo
{
    /// <summary>
    /// 字典管理列表行. 对应 Java DictDataVo.
    /// </summary>
    public class DictDataVo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// 字典标签
        /// </summary>
        [JsonPropertyName("dictLabel")]
        public string DictLabel { get; set; }

        /// <summary>
        /// 字典键值
        /// </summary>
        [JsonPropertyName("dictValue")]
        public string DictValue { get; set; }

        /// <summary>
        /// 字典类型(lowcodeflow/udr/processlabel)
        /// </summary>
        [JsonPropertyName("dictType")]
        public string DictType { get; set; }

        /// <summary>
        /// 字典类型汉字含义(后端映射, 未知类型原样展示)
        /// </summary>
        [JsonPropertyName("dictTypeLabel")]
        public string DictTypeLabel { get; set; }

        /// <summary>
        /// 字典排序
        /// </summary>
        [JsonPropertyName("sort")]
        public int? Sort { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [JsonPropertyName("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [JsonPropertyName("createUser")]
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonPropertyName("createTime")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [JsonPropertyName("updateTime")]
        public DateTime? UpdateTime { get; set; }
    }
}