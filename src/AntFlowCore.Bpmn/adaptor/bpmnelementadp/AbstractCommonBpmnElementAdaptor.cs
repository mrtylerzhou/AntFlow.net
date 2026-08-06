using System.Collections.Generic;
using System.Linq;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.util;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

/// <summary>
/// Common base for element adaptors that build a single/multiplayer assignee
/// BpmnConfCommonElementVo from a BpmnNodeParamsVo, using a variable (collection)
/// name supplied by each concrete subclass.
/// Mirrors the Java AbstractCommonBpmnElementAdaptor.
/// </summary>
public abstract class AbstractCommonBpmnElementAdaptor : BpmnElementAdaptor
{
    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo, int elementCode, string elementId)
    {
        return CommonElementVo(property, paramsVo, elementCode, elementId);
    }

    /// <summary>
    /// Subclasses provide the variable (collection) name prefix used when building
    /// the BPMN element, e.g. "formUsers".
    /// </summary>
    protected abstract string ProvideVarName();

    public BpmnConfCommonElementVo CommonElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo, int elementCode, string elementId)
    {
        int? paramType = paramsVo.ParamType;
        string varName = ProvideVarName();

        if ((int)BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_SINGLE == paramType)
        {
            var bpmnNodeParamsAssigneeVo = paramsVo.Assignee ?? new BpmnNodeParamsAssigneeVo();
            string assignee = bpmnNodeParamsAssigneeVo.Assignee;
            string assigneeName = bpmnNodeParamsAssigneeVo.AssigneeName;
            var singleAssigneeMap = new Dictionary<string, string>
            {
                { assignee, assigneeName }
            };
            return BpmnElementUtils.GetSingleElement(elementId, bpmnNodeParamsAssigneeVo.ElementName,
                string.Concat(varName, elementCode.ToString()), assignee, singleAssigneeMap);
        }
        else
        {
            List<BpmnNodeParamsAssigneeVo> assigneeList = paramsVo.AssigneeList;
            string elementName = assigneeList[0].ElementName;

            int? signType = property == null ? (int)SignTypeEnum.SIGN_TYPE_SIGN : property.SignType;

            var assigneeMap = new Dictionary<string, string>();

            // Java passes BpmnStartConditionsVo; in .NET the duplication strategy is
            // carried in the thread-local container (see BpmnElementAdaptor).
            int strategy = DuplicationProcessStrategyEnum.REMOVE.Code;
            object strategyObj = ThreadLocalContainer.Get(StringConstants.DUPLICATION_PROCESS_STRATEGY);
            if (strategyObj is int s)
            {
                strategy = s;
            }

            foreach (var assigneeVo in assigneeList)
            {
                if (assigneeVo.IsDeduplication == 0)
                {
                    assigneeMap[assigneeVo.Assignee] = assigneeVo.AssigneeName;
                }
                else if (assigneeVo.IsDeduplication == 1)
                {
                    if (DuplicationProcessStrategyEnum.SKIP.Code.Equals(strategy))
                    {
                        assigneeMap[assigneeVo.Assignee] = assigneeVo.AssigneeName;
                    }
                }
            }

            string elementCodeStr = string.Concat(varName, elementCode.ToString());
            if ((int)SignTypeEnum.SIGN_TYPE_SIGN == signType)
            {
                return BpmnElementUtils.GetMultiplayerSignElement(elementId, elementName,
                    elementCodeStr, assigneeMap.Keys.ToList(), assigneeMap);
            }
            else if ((int)SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER == signType)
            {
                return BpmnElementUtils.GetMultiplayerSignInOrderElement(elementId, elementName,
                    elementCodeStr, assigneeMap.Keys.ToList(), assigneeMap);
            }
            else if ((int)SignTypeEnum.SIGN_TYPE_ARBITRATION == signType)
            {
                return BpmnElementUtils.GetMultiplayerArbitrationElement(elementId, elementName,
                    elementCodeStr, assigneeMap.Keys.ToList(), assigneeMap, property?.ArbitrationRatio);
            }
            else
            {
                return BpmnElementUtils.GetMultiplayerOrSignElement(elementId, elementName,
                    elementCodeStr, assigneeMap.Keys.ToList(), assigneeMap);
            }
        }
    }
}
