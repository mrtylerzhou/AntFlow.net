using antflowcore.entity;
using antflowcore.util;

namespace antflowcore.dto;

public class PageRequestDTO<T> where T : new()
{
    public PageDto PageDto { get; set; }=PageUtils.GetPageDto(new Page<object>());
    public T Entity { get; set; }
    
}