using antflowcore.constant.enus;

namespace antflowcore.vo;

public class PageSortVo
{
    //sort field
    public String OrderField { get; set; }

    //sort type
    public SortTypeEnum SortTypeEnum { get; set; }
}