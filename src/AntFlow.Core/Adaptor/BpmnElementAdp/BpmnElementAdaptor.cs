using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

public abstract class BpmnElementAdaptor : IAdaptorService
{
    public abstract void SetSupportBusinessObjects();

    // ��ȡ BpmnConfCommonElementVo ����
    protected abstract BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo,
        int elementCode, string elementId);

    // ��ʽ�� BpmnNodeVo Ϊ BpmnConfCommonElementVo �б�
    public virtual void DoFormatNodesToElements(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        BpmnNodeVo nodeVo, int nodeCode, int sequenceFlowNum, Dictionary<string, int> numMap)
    {
        BpmnConfCommonElementVo elementVo =
            FormatNodesToElements(bpmnConfCommonElementVos, nodeVo, nodeCode, sequenceFlowNum, numMap);
        elementVo.NodeId = nodeVo.Id.ToString();
        elementVo.NodeType = nodeVo.NodeType;
        // ����ǩ��Ԫ��
        DoSignUp(bpmnConfCommonElementVos, elementVo, numMap);
    }

    // ��ʽ���ڵ�ΪԪ��
    private BpmnConfCommonElementVo FormatNodesToElements(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        BpmnNodeVo nodeVo, int nodeCode, int sequenceFlowNum, Dictionary<string, int> numMap)
    {
        string elementId = ProcessNodeEnum.GetDescByCode(nodeCode + 1);
        nodeVo.ElementId = elementId;

        BpmnNodePropertysVo property = nodeVo.Property ?? new BpmnNodePropertysVo();
        int signType = property.SignType ?? 1;

        BpmnConfCommonElementVo elementVo = GetElementVo(nodeVo.Property, nodeVo.Params, nodeCode + 1, elementId);
        SetElementButtons(nodeVo, elementVo);

        elementVo.TemplateVos = nodeVo.TemplateVos;
        elementVo.ApproveRemindVo = nodeVo.ApproveRemindVo;
        elementVo.SignType = signType;
        SetSignUpProperty(nodeVo, elementVo);

        bpmnConfCommonElementVos.Add(elementVo);

        bool hasAlreadyFlowTo = false;

        if (nodeVo.FromNodes != null && nodeVo.FromNodes.Count > 0)
        {
            hasAlreadyFlowTo = true;
            BpmnConfCommonElementVo? parallelGateWayElement =
                BpmnElementUtils.GetParallelGateWayElement(sequenceFlowNum + 1);
            bpmnConfCommonElementVos.Add(parallelGateWayElement);
            string parallelGateWayElementElementId = parallelGateWayElement.ElementId;
            sequenceFlowNum++;

            foreach (BpmnNodeVo? fromNode in nodeVo.FromNodes)
            {
                string fromNodeElementId = fromNode.ElementId;
                bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(sequenceFlowNum + 1, fromNodeElementId,
                    parallelGateWayElementElementId));
                sequenceFlowNum++;
            }

            bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(sequenceFlowNum + 1,
                parallelGateWayElementElementId, elementVo.ElementId));
        }

        if (!hasAlreadyFlowTo)
        {
            foreach (BpmnConfCommonElementVo? bpmnConfCommonElementVo in bpmnConfCommonElementVos)
            {
                if (elementVo.ElementId == bpmnConfCommonElementVo.FlowTo)
                {
                    hasAlreadyFlowTo = true;
                    break;
                }
            }
        }

        if (!hasAlreadyFlowTo)
        {
            bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(sequenceFlowNum + 1,
                ProcessNodeEnum.GetDescByCode(nodeCode), elementVo.ElementId));
        }

        nodeCode++;
        sequenceFlowNum++;

        numMap["nodeCode"] = nodeCode;
        numMap["sequenceFlowNum"] = sequenceFlowNum;

        return elementVo;
    }

    // ����ǩ���ڵ�����
    private void SetSignUpProperty(BpmnNodeVo nodeVo, BpmnConfCommonElementVo elementVo)
    {
        elementVo.IsSignUp = nodeVo.IsSignUp;
        elementVo.AfterSignUpWay = nodeVo.Property?.AfterSignUpWay ?? 0;
        elementVo.SignUpType = nodeVo.Property?.SignUpType ?? 0;
    }

    // ����ǩ��
    private void DoSignUp(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        BpmnConfCommonElementVo fatherElementVo, Dictionary<string, int> numMap)
    {
        if (fatherElementVo.IsSignUp == 1)
        {
            switch (fatherElementVo.AfterSignUpWay)
            {
                case 1: // �ص�������
                    BackApproval(bpmnConfCommonElementVos, fatherElementVo, numMap);
                    break;
                case 2:
                default: // ���ص�������
                    UnbackApproval(bpmnConfCommonElementVos, fatherElementVo, numMap);
                    break;
            }
        }
    }

    // �ص�����������
    private void BackApproval(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        BpmnConfCommonElementVo fatherElementVo, Dictionary<string, int> numMap)
    {
        BpmnConfCommonElementVo? signUpSubElementVo = SetAndGetSignUpSubElement(bpmnConfCommonElementVos,
            fatherElementVo, numMap["nodeCode"], numMap["sequenceFlowNum"], numMap);
        AddBackApproval(bpmnConfCommonElementVos, fatherElementVo, signUpSubElementVo, numMap["nodeCode"],
            numMap["sequenceFlowNum"], numMap);
    }

    // ��ӻص�����������
    private void AddBackApproval(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        BpmnConfCommonElementVo fatherElementVo, BpmnConfCommonElementVo signUpSubElementVo, int nodeCode,
        int sequenceFlowNum, Dictionary<string, int> numMap)
    {
        int elementCode = nodeCode + 1;
        string elementId = ProcessNodeEnum.GetDescByCode(elementCode);
        int elementSequenceFlowNum = sequenceFlowNum + 1;

        BpmnConfCommonElementVo? backApprovalElementVo = BpmnElementUtils.GetSignUpElement(elementId,
            signUpSubElementVo, ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_SERIAL.Code);
        backApprovalElementVo.CollectionName = fatherElementVo.CollectionName;
        backApprovalElementVo.ElementName = fatherElementVo.ElementName; // ����Ԫ�����ƣ���ǩ��Ԫ����ͬ��
        backApprovalElementVo.IsSignUpSubElement = 1; // ����Ϊǩ����Ԫ��
        backApprovalElementVo.IsBackSignUp = 1; // ����Ϊ�ص�ǩ��
        backApprovalElementVo.SignUpElementId = fatherElementVo.ElementId; // ����ǩ��Ԫ�� ID

        SetSignUpElementButtons(backApprovalElementVo);
        bpmnConfCommonElementVos.Add(backApprovalElementVo);

        bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(elementSequenceFlowNum,
            ProcessNodeEnum.GetDescByCode(nodeCode), backApprovalElementVo.ElementId));

        nodeCode++;
        sequenceFlowNum++;

        numMap["nodeCode"] = nodeCode;
        numMap["sequenceFlowNum"] = sequenceFlowNum;
    }

    // ���ص�����������
    private void UnbackApproval(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        BpmnConfCommonElementVo fatherElementVo, Dictionary<string, int> numMap)
    {
        SetAndGetSignUpSubElement(bpmnConfCommonElementVos, fatherElementVo, numMap["nodeCode"],
            numMap["sequenceFlowNum"], numMap);
    }

    // ���ò���ȡǩ��Ԫ��
    private BpmnConfCommonElementVo SetAndGetSignUpSubElement(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        BpmnConfCommonElementVo fatherElementVo, int nodeCode, int sequenceFlowNum, Dictionary<string, int> numMap)
    {
        int elementCode = nodeCode + 1;
        string elementId = ProcessNodeEnum.GetDescByCode(elementCode);
        int elementSequenceFlowNum = sequenceFlowNum + 1;

        BpmnConfCommonElementVo signUpElementVo;
        switch (fatherElementVo.SignUpType)
        {
            case 1: // ˳��ǩ��
                signUpElementVo = BpmnElementUtils.GetSignUpElement(elementId, fatherElementVo,
                    ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_SERIAL.Code);
                break;
            case 2: // ����ǩ��
                signUpElementVo = BpmnElementUtils.GetSignUpElement(elementId, fatherElementVo,
                    ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_PARALLEL.Code);
                break;
            case 3: // ��ǩ��
                signUpElementVo = BpmnElementUtils.GetSignUpElement(elementId, fatherElementVo,
                    ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_PARALLEL_OR.Code);
                break;
            default: // Ĭ��˳��ǩ��
                signUpElementVo = BpmnElementUtils.GetSignUpElement(elementId, fatherElementVo,
                    ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_SERIAL.Code);
                break;
        }

        signUpElementVo.IsSignUpSubElement = 1; // ����Ϊǩ����Ԫ��
        signUpElementVo.SignUpElementId = fatherElementVo.ElementId; // ����ǩ��Ԫ�� ID

        SetSignUpElementButtons(signUpElementVo);
        bpmnConfCommonElementVos.Add(signUpElementVo);

        BpmnConfCommonElementVo? signUpSequenceFlow = BpmnElementUtils.GetSequenceFlow(elementSequenceFlowNum,
            ProcessNodeEnum.GetDescByCode(nodeCode), signUpElementVo.ElementId);
        signUpSequenceFlow.IsSignUpSequenceFlow = 1;
        bpmnConfCommonElementVos.Add(signUpSequenceFlow);

        nodeCode++;
        sequenceFlowNum++;

        numMap["nodeCode"] = nodeCode;
        numMap["sequenceFlowNum"] = sequenceFlowNum;

        return signUpElementVo;
    }

    /// <summary>
    ///     Set sign up element buttons
    /// </summary>
    /// <param name="elementVo">The BpmnConfCommonElementVo object</param>
    private void SetSignUpElementButtons(BpmnConfCommonElementVo elementVo)
    {
        elementVo.Buttons = new BpmnConfCommonButtonsVo
        {
            ApprovalPage = new List<BpmnConfCommonButtonPropertyVo>
            {
                new()
                {
                    ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_AGREE,
                    ButtonName =
                        ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_AGREE)
                },
                new()
                {
                    ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_DISAGREE,
                    ButtonName =
                        ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_DISAGREE)
                }
            }
        };
    }

    // ����Ԫ�ذ�ť
    protected void SetElementButtons(BpmnNodeVo nodeVo, BpmnConfCommonElementVo elementVo)
    {
        elementVo.Buttons = new BpmnConfCommonButtonsVo
        {
            StartPage = nodeVo.Buttons.StartPage
                .Select(o => new BpmnConfCommonButtonPropertyVo
                {
                    ButtonType = o, ButtonName = ButtonTypeEnumExtensions.GetDescByCode(o)
                })
                .ToList(),
            ApprovalPage = nodeVo.Buttons.ApprovalPage
                .Select(o => new BpmnConfCommonButtonPropertyVo
                {
                    ButtonType = o, ButtonName = ButtonTypeEnumExtensions.GetDescByCode(o)
                })
                .ToList()
        };
    }
}