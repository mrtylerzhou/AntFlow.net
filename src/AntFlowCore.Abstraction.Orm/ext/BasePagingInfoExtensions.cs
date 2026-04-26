using AntFlowCore.Base.entity;
using FreeSql.Internal.Model;

namespace AntFlowCore.Abstraction.Orm.ext;

public static class BasePagingInfoExtensions
{
  public  static BasePagingInfo ToBasePagingInfo(this PagingInfo pageDto)
  {
    BasePagingInfo basePagingInfo = new BasePagingInfo()
    {
      PageNumber = pageDto.PageNumber,
      PageSize = pageDto.PageSize,
      Count = pageDto.Count
    };
    return basePagingInfo;
  }
  public static PagingInfo ToPagingInfo(this BasePagingInfo basePagingInfo)
  {
    PagingInfo pagingInfo = new PagingInfo()
    {
      PageNumber = basePagingInfo.PageNumber,
      PageSize = basePagingInfo.PageSize,
      Count = basePagingInfo.Count
    };
    return pagingInfo;
  }
}