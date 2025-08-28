using AntFlow.Core.Constant.Enums;

namespace AntFlow.Core.Vo;

public class PageSortVo
{
    //sort field
    public string OrderField { get; set; }

    //sort type
    public SortTypeEnum SortTypeEnum { get; set; }
}