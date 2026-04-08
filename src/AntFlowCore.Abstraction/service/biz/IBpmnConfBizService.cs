using AntFlowCore.Base.dto;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IBpmnConfBizService
{
    void Edit(BpmnConfVo bpmnConfVo);
    ResultAndPage<BpmnConfVo> SelectPage(PageDto pageDto, BpmnConfVo vo);
    BpmnConfVo Detail(long id);
    BpmnConfVo Detail(string bpmnCode);
    BpmnConfVo DetailByFormCode(string formCode);
    int? GetCustomizeNodeSignType(long nodeId);
}
