using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.service;

public interface IDictService
{
    List<BaseKeyValueStruVo> GetLowCodeFlowFormCodes();
    int AddFormCode(BaseKeyValueStruVo dto);
  
}
