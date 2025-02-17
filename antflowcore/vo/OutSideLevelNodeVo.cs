using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class OutSideLevelNodeVo
    {
        [JsonPropertyName("nodeMark")]
        public string NodeMark { get; set; }

        [JsonPropertyName("nodeName")]
        public string NodeName { get; set; }

        [JsonPropertyName("assignees")]
        public List<OutSidelevelAssignees> Assignees { get; set; } = new List<OutSidelevelAssignees>();
    }
}