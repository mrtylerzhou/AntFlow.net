﻿using System;
using System.Text.Json.Serialization;
using antflowcore.conf.json;

namespace antflowcore.vo
{
    public class BaseIdTranStruVo
    {
        [JsonPropertyName("id"),JsonConverter(typeof(IntToStringConverter))]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        // 默认构造函数
        public BaseIdTranStruVo() { }

        // 带参数的构造函数
        public BaseIdTranStruVo(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}