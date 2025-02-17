using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class OutSideBpmAccessRespVo
    {
        /// <summary>
        /// 流程编号
        /// </summary>
        [JsonPropertyName("processNumber")]
        public string ProcessNumber { get; set; }

        /// <summary>
        /// 业务Id
        /// </summary>
        [JsonPropertyName("businessId")]
        public string BusinessId { get; set; }

        /// <summary>
        /// 流程记录列表
        /// </summary>
        [JsonPropertyName("processRecord")]
        public List<OutSideBpmAccessProcessRecordVo> ProcessRecord { get; set; }
    }
}