using AntFlow.Core.Entity;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

public class BpmnAddFlowElementSignUpSerialAdaptor : IBpmnAddFlowElementAdaptor
{
    public void AddFlowElement(BpmnConfCommonElementVo elementVo, AFProcess process,
        Dictionary<string, object> startParamMap, BpmnStartConditionsVo bpmnStartConditions)
    {
        // ��ȡ����ֵ
        List<string>? collectionValue = elementVo.CollectionValue;

        // ƴ��ѭ�������ı�ʶ
        string? loopCardinality = $"{elementVo.ElementId}size";

        // ����˳��ѭ���û�������ӵ�����
        process.AddFlowElement(BpmnBuildUtils.CreateLoopUserTask(
            elementVo.ElementId,
            elementVo.ElementName,
            loopCardinality,
            elementVo.CollectionName,
            elementVo.CollectionName.Replace("List", "")));

        // ����������������
        startParamMap[elementVo.CollectionName] = collectionValue;
        startParamMap[loopCardinality] = collectionValue.Count;
    }
}