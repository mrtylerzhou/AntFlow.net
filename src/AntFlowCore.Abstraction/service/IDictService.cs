using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.service;

public interface IDictService
{
    List<BaseKeyValueStruVo> GetLowCodeFlowFormCodes();
    int AddFormCode(BaseKeyValueStruVo dto);
    ResultAndPage<BaseKeyValueStruVo> SelectLFFormCodePageList(PageDto pageDto, TaskMgmtVO taskMgmtVo);
    ResultAndPage<BaseKeyValueStruVo> SelectLFActiveFormCodePageList(PageDto pageDto, TaskMgmtVO taskMgmtVO);
}
