using AntFlowCore.Base.constant.enums;

namespace AntFlowCore.Base.vo;

public class PageSortVo
{
    //sort field
    public String OrderField { get; set; }

    //sort type
    public SortTypeEnum SortTypeEnum { get; set; }
}