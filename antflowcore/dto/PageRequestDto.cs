﻿using System.Text.Json.Serialization;
using antflowcore.entity;
using antflowcore.util;

namespace antflowcore.dto;

public class PageRequestDto<T> where T : new()
{
    [JsonPropertyName("pageDto")]
    public PageDto PageDto { get; set; }=PageUtils.GetPageDto(new Page<object>());
    [JsonPropertyName("entity")]
    public T Entity { get; set; }
    
}