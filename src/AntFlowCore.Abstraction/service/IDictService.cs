using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.service;

public interface IDictService
{
    List<BaseKeyValueStruVo> GetLowCodeFlowFormCodes();
    int AddFormCode(BaseKeyValueStruVo dto);
    //page-added DIY(LF 后端 + 自定义 Vue 前端, dict_type=diylowcodeflow)
    int AddDIYFormCode(BaseKeyValueStruVo dto);
    List<DIYProcessInfoDTO> GetDIYActiveFormCodes();
}
