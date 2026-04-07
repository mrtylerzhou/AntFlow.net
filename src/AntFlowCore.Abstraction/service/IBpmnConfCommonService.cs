using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service;

public interface IBpmnConfCommonService
{
    BpmnConf GetBpmnConfByFormCode(string formCode);
    void StartProcess(string bpmnCode, BpmnStartConditionsVo bpmnStartConditions);
    PreviewNode PreviewNode(string parameters);
    PreviewNode GetPreviewNode(string parameters, bool isStartPagePreview);
    List<BpmnNodeVo> SetNodeFromV2(List<BpmnNodeVo> nodeList);
    PreviewNode StartPagePreviewNode(string paramsJson);
    PreviewNode TaskPagePreviewNode(string paramsJson);
    List<BpmnConf> GetBpmnConfByFormCodeBatch(List<string> formCodes);
    List<BaseIdTranStruVo> LoadNodeOperationUser(string requestParams);
}
