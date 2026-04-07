using AntFlowCore.Core.dto;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Engine.service.repository;

public interface IDictService
{
    List<BaseKeyValueStruVo> GetLowCodeFlowFormCodes();
    int AddFormCode(BaseKeyValueStruVo dto);
    ResultAndPage<BaseKeyValueStruVo> SelectLFFormCodePageList(PageDto pageDto, TaskMgmtVO taskMgmtVo);
    ResultAndPage<BaseKeyValueStruVo> SelectLFActiveFormCodePageList(PageDto pageDto, TaskMgmtVO taskMgmtVO);
}
