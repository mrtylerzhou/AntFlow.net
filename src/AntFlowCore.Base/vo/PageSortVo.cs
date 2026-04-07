using AntFlowCore.Common.constant.enus;

namespace AntFlowCore.Core.vo;

public class PageSortVo
{
    //sort field
    public String OrderField { get; set; }

    //sort type
    public SortTypeEnum SortTypeEnum { get; set; }
}