using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IBpmnViewPageButtonBizService
{
    void EditBpmnViewPageButton(BpmnConfVo bpmnConfVo, long confId);
}