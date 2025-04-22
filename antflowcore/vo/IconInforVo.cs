using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class IconInforVo
    {
        /// <summary>
        /// Gets or sets the common function process type information.
        /// </summary>
        [JsonPropertyName("commonFunction")]
        public ProcessTypeInforVo CommonFunction { get; set; }

        /// <summary>
        /// Gets or sets the application list of process type information.
        /// </summary>
        [JsonPropertyName("applicationList")]
        public List<ProcessTypeInforVo> ApplicationList { get; set; }

        /// <summary>
        /// Gets or sets the son application list process type information.
        /// </summary>
        [JsonPropertyName("sonApplicationList")]
        public ProcessTypeInforVo SonApplicationList { get; set; }
    }
    
}