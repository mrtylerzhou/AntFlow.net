using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

public abstract class BpmnElementAdaptor : IAdaptorService
{
    public abstract void SetSupportBusinessObjects();

    // 获取 BpmnConfCommonElementVo 对象
    protected abstract BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo,
        int elementCode, string elementId);

    // 格式化 BpmnNodeVo 为 BpmnConfCommonElementVo 列表
    public virtual void DoFormatNodesToElements(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        BpmnNodeVo nodeVo, int nodeCode, int sequenceFlowNum, Dictionary<string, int> numMap)
    {
        BpmnConfCommonElementVo elementVo =
            FormatNodesToElements(bpmnConfCommonElementVos, nodeVo, nodeCode, sequenceFlowNum, numMap);
        elementVo.NodeId = nodeVo.Id.ToString();
        elementVo.NodeType = nodeVo.NodeType;
        // 处理签到元素
        DoSignUp(bpmnConfCommonElementVos, elementVo, numMap);
    }

    // 格式化节点为元素
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

    // 设置签到节点属性
    private void SetSignUpProperty(BpmnNodeVo nodeVo, BpmnConfCommonElementVo elementVo)
    {
        elementVo.IsSignUp = nodeVo.IsSignUp;
        elementVo.AfterSignUpWay = nodeVo.Property?.AfterSignUpWay ?? 0;
        elementVo.SignUpType = nodeVo.Property?.SignUpType ?? 0;
    }

    // 处理签到
    private void DoSignUp(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        BpmnConfCommonElementVo fatherElementVo, Dictionary<string, int> numMap)
    {
        if (fatherElementVo.IsSignUp == 1)
        {
            switch (fatherElementVo.AfterSignUpWay)
            {
                case 1: // 回到分配者
                    BackApproval(bpmnConfCommonElementVos, fatherElementVo, numMap);
                    break;
                case 2:
                default: // 不回到分配者
                    UnbackApproval(bpmnConfCommonElementVos, fatherElementVo, numMap);
                    break;
            }
        }
    }

    // 回到分配者审批
    private void BackApproval(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        BpmnConfCommonElementVo fatherElementVo, Dictionary<string, int> numMap)
    {
        BpmnConfCommonElementVo? signUpSubElementVo = SetAndGetSignUpSubElement(bpmnConfCommonElementVos,
            fatherElementVo, numMap["nodeCode"], numMap["sequenceFlowNum"], numMap);
        AddBackApproval(bpmnConfCommonElementVos, fatherElementVo, signUpSubElementVo, numMap["nodeCode"],
            numMap["sequenceFlowNum"], numMap);
    }

    // 添加回到分配者审批
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
        backApprovalElementVo.ElementName = fatherElementVo.ElementName; // 设置元素名称（与签到元素相同）
        backApprovalElementVo.IsSignUpSubElement = 1; // 设置为签到子元素
        backApprovalElementVo.IsBackSignUp = 1; // 设置为回到签到
        backApprovalElementVo.SignUpElementId = fatherElementVo.ElementId; // 设置签到元素 ID

        SetSignUpElementButtons(backApprovalElementVo);
        bpmnConfCommonElementVos.Add(backApprovalElementVo);

        bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(elementSequenceFlowNum,
            ProcessNodeEnum.GetDescByCode(nodeCode), backApprovalElementVo.ElementId));

        nodeCode++;
        sequenceFlowNum++;

        numMap["nodeCode"] = nodeCode;
        numMap["sequenceFlowNum"] = sequenceFlowNum;
    }

    // 不回到分配者审批
    private void UnbackApproval(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        BpmnConfCommonElementVo fatherElementVo, Dictionary<string, int> numMap)
    {
        SetAndGetSignUpSubElement(bpmnConfCommonElementVos, fatherElementVo, numMap["nodeCode"],
            numMap["sequenceFlowNum"], numMap);
    }

    // 设置并获取签到元素
    private BpmnConfCommonElementVo SetAndGetSignUpSubElement(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        BpmnConfCommonElementVo fatherElementVo, int nodeCode, int sequenceFlowNum, Dictionary<string, int> numMap)
    {
        int elementCode = nodeCode + 1;
        string elementId = ProcessNodeEnum.GetDescByCode(elementCode);
        int elementSequenceFlowNum = sequenceFlowNum + 1;

        BpmnConfCommonElementVo signUpElementVo;
        switch (fatherElementVo.SignUpType)
        {
            case 1: // 顺序签到
                signUpElementVo = BpmnElementUtils.GetSignUpElement(elementId, fatherElementVo,
                    ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_SERIAL.Code);
                break;
            case 2: // 并行签到
                signUpElementVo = BpmnElementUtils.GetSignUpElement(elementId, fatherElementVo,
                    ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_PARALLEL.Code);
                break;
            case 3: // 或签到
                signUpElementVo = BpmnElementUtils.GetSignUpElement(elementId, fatherElementVo,
                    ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_PARALLEL_OR.Code);
                break;
            default: // 默认顺序签到
                signUpElementVo = BpmnElementUtils.GetSignUpElement(elementId, fatherElementVo,
                    ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_SERIAL.Code);
                break;
        }

        signUpElementVo.IsSignUpSubElement = 1; // 设置为签到子元素
        signUpElementVo.SignUpElementId = fatherElementVo.ElementId; // 设置签到元素 ID

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

    // 设置元素按钮
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