using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

public class BpmnAddFlowElementLoopAdaptor : IBpmnAddFlowElementAdaptor
{
    private readonly ILogger<BpmnAddFlowElementLoopAdaptor> _logger;

    public BpmnAddFlowElementLoopAdaptor(ILogger<BpmnAddFlowElementLoopAdaptor> logger)
    {
        _logger = logger;
    }

    public void AddFlowElement(BpmnConfCommonElementVo elementVo, AFProcess process,
        Dictionary<string, object> startParamMap, BpmnStartConditionsVo bpmnStartConditions)
    {
        // ��ȡԪ�صĴ���
        int? elementCode = ProcessNodeEnum.GetCodeByDesc(elementVo.ElementId);

        // ��������ִ���˲������ƺͽ����������
        string? assigneeParamName = $"loopUser{elementCode}";
        string? endLoopMark = $"${{endLoopMark{elementCode}==true}}";

        // ���ѭ������Ԫ�ص�������
        process.AddFlowElement(BpmnBuildUtils.CreateLoopUserTask(
            elementVo.ElementId,
            elementVo.ElementName,
            assigneeParamName,
            endLoopMark
        ));

        // ��Ĭ�ϵĽ��������ӵ�����ӳ����
        startParamMap[$"endLoopMark{elementCode}"] = false;

        _logger.LogInformation($"Loop User Task added with ID: {elementVo.ElementId}, Name: {elementVo.ElementName}");
    }
}