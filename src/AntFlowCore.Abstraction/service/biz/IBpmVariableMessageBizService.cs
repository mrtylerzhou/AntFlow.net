using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IBpmVariableMessageBizService
{
    BpmVariableMessageVo GetBpmVariableMessageVo(BusinessDataVo businessDataVo);
    BpmVariableMessageVo GetBpmVariableMessageVo(BpmVariableMessageVo vo);
    void InsertVariableMessage(long variableId, BpmnConfCommonVo bpmnConfCommonVo);
    bool CheckIsSendByTemplate(BpmVariableMessageVo vo);
    void SendTemplateMessages(BpmVariableMessageVo vo);
    void SendTemplateMessages(BusinessDataVo businessDataVo);
    Dictionary<string, string> GetUrlMap(BpmVariableMessageVo vo, InformationTemplateVo informationTemplateVo);
    BpmVariableMessageVo FromBusinessDataVo(BusinessDataVo businessDataVo);
    
}