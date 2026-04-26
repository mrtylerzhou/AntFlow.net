using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.biz;

public interface IDicDataBizSerivce
{
    ResultAndPage<BaseKeyValueStruVo> SelectLFFormCodePageList(PageDto pageDto, TaskMgmtVO taskMgmtVo);
    ResultAndPage<BaseKeyValueStruVo> SelectLFActiveFormCodePageList(PageDto pageDto, TaskMgmtVO taskMgmtVO);
}
