using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.constants;
using AntFlowCore.Bpmn.util;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

public abstract class BpmnElementAdaptor : IAdaptorService
    {
        // 获取 BpmnConfCommonElementVo 对象
        protected abstract BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo, int elementCode, string elementId);

        // 格式化 BpmnNodeVo 为 BpmnConfCommonElementVo 列表
        public  virtual void DoFormatNodesToElements(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos, BpmnNodeVo nodeVo, int nodeCode, int sequenceFlowNum, Dictionary<string, int> numMap)
        {
            BpmnConfCommonElementVo elementVo = FormatNodesToElements(bpmnConfCommonElementVos, nodeVo, nodeCode, sequenceFlowNum, numMap);
            elementVo.NodeId = nodeVo.Id.ToString();
            elementVo.NodeType = nodeVo.NodeType;
            // 处理签到元素
            DoSignUp(bpmnConfCommonElementVos, elementVo, numMap);
        }

        // 格式化节点为元素
        private BpmnConfCommonElementVo FormatNodesToElements(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos, BpmnNodeVo nodeVo, int nodeCode, int sequenceFlowNum, Dictionary<string, int> numMap)
        {
            string elementId = ProcessNodeEnum.GetDescByCode(nodeCode + 1);
            nodeVo.ElementId = elementId;
            
            BpmnNodePropertysVo property = nodeVo.Property??new BpmnNodePropertysVo();
            int signType = property.SignType ?? 1;

            BpmnConfCommonElementVo elementVo = GetElementVo(nodeVo.Property, nodeVo.Params, nodeCode + 1, elementId);
            elementVo.AggregationNode = nodeVo.AggregationNode;
            SetElementButtons(nodeVo, elementVo);

            elementVo.TemplateVos = nodeVo.TemplateVos;
            elementVo.ApproveRemindVo = nodeVo.ApproveRemindVo;
            elementVo.SignType = signType;
          
            // Adjacent deduplication (SKIP_NEXT): if node is deduplicated, add skippedAssignees label
            // so that BpmnTaskListener can auto-skip the task at runtime
            AddSkippedAssigneesLabelIfNeeded(nodeVo);
            SetSignUpProperty(nodeVo, elementVo);
            // carry node labels onto the final BPMN element VO
            elementVo.LabelList = nodeVo.LabelList;
            bpmnConfCommonElementVos.Add(elementVo);

            bool hasAlreadyFlowTo = false;

            if (nodeVo.FromNodes != null && nodeVo.FromNodes.Count > 0)
            {
                hasAlreadyFlowTo = true;
                var parallelGateWayElement = BpmnElementUtils.GetParallelGateWayElement(sequenceFlowNum + 1);
                bpmnConfCommonElementVos.Add(parallelGateWayElement);
                string parallelGateWayElementElementId = parallelGateWayElement.ElementId;
                sequenceFlowNum++;

                foreach (var fromNode in nodeVo.FromNodes)
                {
                    string fromNodeElementId = fromNode.ElementId;
                    bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(sequenceFlowNum + 1, fromNodeElementId, parallelGateWayElementElementId));
                    sequenceFlowNum++;
                }

                bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(sequenceFlowNum + 1, parallelGateWayElementElementId, elementVo.ElementId));
            }

            if (!hasAlreadyFlowTo)
            {
                foreach (var bpmnConfCommonElementVo in bpmnConfCommonElementVos)
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
                bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(sequenceFlowNum + 1, ProcessNodeEnum.GetDescByCode(nodeCode), elementVo.ElementId));
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

        /// <summary>
        /// 相邻节点去重(SKIP策略): 当 DuplicationProcessStrategy==SKIP 时,
        /// 如果节点被标记为去重(IsNodeDeduplication==1),
        /// 收集被去重的审批人ID,给节点打上 skippedAssignees 标签.
        /// 流程运行时 BpmnTaskListener 检查此标签,自动完成匹配的任务.
        /// 对应 Java BpmnElementAdaptor.doFormatNodesToElements 第49-77行.
        /// </summary>
        private void AddSkippedAssigneesLabelIfNeeded(BpmnNodeVo nodeVo)
        {
            // Only apply when strategy is SKIP
            object strategyObj = ThreadLocalContainer.Get(StringConstants.DUPLICATION_PROCESS_STRATEGY);
            int strategy = strategyObj is int s ? s : DuplicationProcessStrategyEnum.REMOVE.Code;
            if (strategy != DuplicationProcessStrategyEnum.SKIP.Code)
            {
                return;
            }

            BpmnNodeParamsVo paramsVo = nodeVo.Params;
            if (paramsVo == null)
            {
                return;
            }

            int? paramType = paramsVo.ParamType;
            List<string> skippedIds = new List<string>();

            if (paramsVo.IsNodeDeduplication == 1)
            {
                // Whole node is deduplicated
                if (paramType == 1 && paramsVo.Assignee != null)
                {
                    // Single player: the assignee is skipped
                    skippedIds.Add(paramsVo.Assignee.Assignee);
                }
                else if (paramType == 2 && paramsVo.AssigneeList != null)
                {
                    // Multiplayer: all assignees in this node are skipped
                    skippedIds.AddRange(paramsVo.AssigneeList.Select(a => a.Assignee));
                }
            }
            else if (paramType == 2 && paramsVo.AssigneeList != null)
            {
                // Node not fully deduplicated, but some assignees might be
                var deduplicatedAssignees = paramsVo.AssigneeList.Where(a => a.IsDeduplication == 1).ToList();
                if (deduplicatedAssignees.Count > 0)
                {
                    skippedIds.AddRange(deduplicatedAssignees.Select(a => a.Assignee));
                }
            }

            if (skippedIds.Count == 0)
            {
                return;
            }

            nodeVo.LabelList ??= new List<BpmnNodeLabelVO>();
            // avoid duplicate label
            if (!nodeVo.LabelList.Any(l => StringConstants.SKIPPED_ASSIGNEE == l.LabelValue))
            {
                nodeVo.LabelList.Add(new BpmnNodeLabelVO(
                    StringConstants.SKIPPED_ASSIGNEE,
                    string.Join(",", skippedIds)
                ));
            }
        }

        // 处理加批
        private void DoSignUp(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos, BpmnConfCommonElementVo fatherElementVo, Dictionary<string, int> numMap)
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
        private void BackApproval(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos, BpmnConfCommonElementVo fatherElementVo, Dictionary<string, int> numMap)
        {
            BpmnConfCommonElementVo signUpSubElementVo = SetAndGetSignUpSubElement(bpmnConfCommonElementVos, fatherElementVo, numMap["nodeCode"], numMap["sequenceFlowNum"], numMap);
            AddBackApproval(bpmnConfCommonElementVos, fatherElementVo, signUpSubElementVo, numMap["nodeCode"], numMap["sequenceFlowNum"], numMap);
        }

        // 添加回到分配者审批
        private void AddBackApproval(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos, BpmnConfCommonElementVo fatherElementVo, BpmnConfCommonElementVo signUpSubElementVo, int nodeCode, int sequenceFlowNum, Dictionary<string, int> numMap)
        {
            int elementCode = nodeCode + 1;
            string elementId = ProcessNodeEnum.GetDescByCode(elementCode);
            int elementSequenceFlowNum = sequenceFlowNum + 1;

            var backApprovalElementVo = BpmnElementUtils.GetSignUpElement(elementId, signUpSubElementVo, ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_SERIAL.Code);
            backApprovalElementVo.CollectionName = fatherElementVo.CollectionName;
            backApprovalElementVo.ElementName = fatherElementVo.ElementName; // 设置元素名称（与加签元素相同）
            backApprovalElementVo.IsSignUpSubElement = 1; // 设置为加签子元素
            backApprovalElementVo.IsBackSignUp = 1; // 设置为回到加批人
            backApprovalElementVo.SignUpElementId = fatherElementVo.ElementId;

            SetSignUpElementButtons(backApprovalElementVo);
            bpmnConfCommonElementVos.Add(backApprovalElementVo);

            bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(elementSequenceFlowNum, ProcessNodeEnum.GetDescByCode(nodeCode), backApprovalElementVo.ElementId));

            nodeCode++;
            sequenceFlowNum++;

            numMap["nodeCode"] = nodeCode;
            numMap["sequenceFlowNum"] = sequenceFlowNum;
        }

        // 不回到分配者审批
        private void UnbackApproval(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos, BpmnConfCommonElementVo fatherElementVo, Dictionary<string, int> numMap)
        {
            SetAndGetSignUpSubElement(bpmnConfCommonElementVos, fatherElementVo, numMap["nodeCode"], numMap["sequenceFlowNum"], numMap);
        }

        // 设置并获取签到元素
        private BpmnConfCommonElementVo SetAndGetSignUpSubElement(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos, BpmnConfCommonElementVo fatherElementVo, int nodeCode, int sequenceFlowNum, Dictionary<string, int> numMap)
        {
            int elementCode = nodeCode + 1;
            string elementId = ProcessNodeEnum.GetDescByCode(elementCode);
            int elementSequenceFlowNum = sequenceFlowNum + 1;

            BpmnConfCommonElementVo signUpElementVo;
            switch (fatherElementVo.SignUpType)
            {
                case 1: // 顺序签到
                    signUpElementVo = BpmnElementUtils.GetSignUpElement(elementId, fatherElementVo, ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_SERIAL.Code);
                    break;
                case 2: // 并行签到
                    signUpElementVo = BpmnElementUtils.GetSignUpElement(elementId, fatherElementVo, ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_PARALLEL.Code);
                    break;
                case 3: // 或签到
                    signUpElementVo = BpmnElementUtils.GetSignUpElement(elementId, fatherElementVo, ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_PARALLEL_OR.Code);
                    break;
                default: // 默认顺序签到
                    signUpElementVo = BpmnElementUtils.GetSignUpElement(elementId, fatherElementVo, ElementPropertyEnum.ELEMENT_PROPERTY_SIGN_UP_SERIAL.Code);
                    break;
            }

            signUpElementVo.IsSignUpSubElement = 1; // 设置为签到子元素
            signUpElementVo.SignUpElementId = fatherElementVo.ElementId; // 设置签到元素 ID

            SetSignUpElementButtons(signUpElementVo);
            bpmnConfCommonElementVos.Add(signUpElementVo);

            var signUpSequenceFlow = BpmnElementUtils.GetSequenceFlow(elementSequenceFlowNum, ProcessNodeEnum.GetDescByCode(nodeCode), signUpElementVo.ElementId);
            signUpSequenceFlow.IsSignUpSequenceFlow = 1;
            bpmnConfCommonElementVos.Add(signUpSequenceFlow);

            nodeCode++;
            sequenceFlowNum++;

            numMap["nodeCode"] = nodeCode;
            numMap["sequenceFlowNum"] = sequenceFlowNum;

            return signUpElementVo;
        }
        /// <summary>
        /// Set sign up element buttons
        /// </summary>
        /// <param name="elementVo">The BpmnConfCommonElementVo object</param>
        private void SetSignUpElementButtons(BpmnConfCommonElementVo elementVo)
        {
            elementVo.Buttons = new BpmnConfCommonButtonsVo
            {
                ApprovalPage = new List<BpmnConfCommonButtonPropertyVo>
                {
                    new BpmnConfCommonButtonPropertyVo
                    {
                        ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_AGREE,
                        ButtonName = ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_AGREE)
                    },
                    new BpmnConfCommonButtonPropertyVo
                    {
                        ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_DISAGREE,
                        ButtonName =ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_DISAGREE)
                    }
                }
            };
        }

        // 设置元素按钮
        protected void SetElementButtons(BpmnNodeVo nodeVo, BpmnConfCommonElementVo elementVo)
        {
            elementVo.Buttons = new BpmnConfCommonButtonsVo
            {
                StartPage = nodeVo.Buttons?.StartPage?
                    .Select(ToButtonPropertyVo)
                    .ToList(),
                ApprovalPage = nodeVo.Buttons?.ApprovalPage?
                    .Select(ToButtonPropertyVo)
                    .ToList(),
                ViewPage = nodeVo.Buttons?.ViewPage?
                    .Select(ToButtonPropertyVo)
                    .ToList()
            };
        }

        /// <summary>
        /// 将设计时按钮 VO 转换为运行时按钮 VO.
        /// 自定义名称非空时使用自定义值,否则回退到按钮类型对应的默认名称.
        /// </summary>
        private static BpmnConfCommonButtonPropertyVo ToButtonPropertyVo(BpmnConfCommonButtonPropertyVo src)
        {
            int btnType = src.ButtonType ?? 0;
            string customName = src.ButtonName;
            string resolvedName = !string.IsNullOrWhiteSpace(customName)
                ? customName
                : ButtonTypeEnumExtensions.GetDescByCode(btnType);
            return new BpmnConfCommonButtonPropertyVo
            {
                ButtonType = btnType,
                ButtonName = resolvedName
            };
        }

        public abstract void SetSupportBusinessObjects();

    }

