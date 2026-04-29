using AntFlowCore.Base.entity;

namespace AntFlowCore.Abstraction.Orm.ext;

public static class PagingInfoExtensions
{
    public static (int PageNumber, int PageSize) ToPageParams(this PagingInfo pageDto)
    {
        return (pageDto.PageNumber, pageDto.PageSize);
    }
}
