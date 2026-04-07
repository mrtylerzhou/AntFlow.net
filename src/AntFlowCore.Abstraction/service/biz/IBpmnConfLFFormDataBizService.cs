using AntFlowCore.Core.entity;

namespace AntFlowCore.Abstraction.service.biz;

public interface IBpmnConfLFFormDataBizService
{
    BpmnConfLfFormdata GetLFFormDataByFormCode(string formCode);
}
