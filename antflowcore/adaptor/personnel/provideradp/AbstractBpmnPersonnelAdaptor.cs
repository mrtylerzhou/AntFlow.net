using antflowcore.adaptor.personnel.loopsign;
using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enums;
using antflowcore.service;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provideradp;

/// <summary>
/// Abstract personnel adaptor to find personnel.
/// AntFlow mainly has three extension points: node adaptor, element adaptor, and personnel adaptor.
/// </summary>
public abstract class AbstractBpmnPersonnelAdaptor : IAdaptorService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IBpmnEmployeeInfoProviderService _bpmnEmployeeInfoProviderService;
    private readonly IBpmnPersonnelProviderService _bpmnPersonnelProviderService;

    public AbstractBpmnPersonnelAdaptor(
        IBpmnPersonnelProviderService bpmnPersonnelProviderService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService
        )
    {
        _bpmnEmployeeInfoProviderService = bpmnEmployeeInfoProviderService ?? throw new ArgumentNullException(nameof(bpmnEmployeeInfoProviderService));
        _bpmnPersonnelProviderService = bpmnPersonnelProviderService ?? throw new ArgumentNullException(nameof(bpmnPersonnelProviderService));
    }

    public void SetNodeParams(
        BpmnNodeVo nodeVo,
        BpmnStartConditionsVo startConditionsVo,
        BpmnNodeParamTypeEnum nodeParamTypeEnum,
        string nextId,
        Dictionary<string, BpmnNodeVo> mapPreNodes,
        HashSet<BpmnNodeVo> setAddNodes)
    {
        if (nodeVo == null)
        {
            throw new InvalidOperationException("nodeVo cannot be null if you want to set properties to it.");
        }

        if (nodeParamTypeEnum == null)
        {
            throw new InvalidOperationException("Parameter nodeParamTypeEnum cannot be null.");
        }

        var paramsVo = nodeVo.Params;
        var nodeName = nodeVo.NodeName;

        if (paramsVo == null)
        {
            throw new InvalidOperationException("Parameter paramsVo cannot be null.");
        }

        var orderedNodeType = nodeVo.OrderedNodeType;
        if (orderedNodeType.HasValue)
        {
            var orderNodeTypeEnum = OrderNodeTypeEnumExtensions.GetByCode(orderedNodeType.Value);
            var abstractOrderedSignNodeAdps = ServiceProviderUtils.GetServices<AbstractOrderedSignNodeAdp>();
            AbstractOrderedSignNodeAdp orderedSignNodeAdp = null;

            foreach (var abstractOrderedSignNodeAdp in abstractOrderedSignNodeAdps)
            {
                if (((IAdaptorService)abstractOrderedSignNodeAdp).IsSupportBusinessObject(orderNodeTypeEnum))
                {
                    orderedSignNodeAdp = abstractOrderedSignNodeAdp;
                    break;
                }
            }

            orderedSignNodeAdp?.FormatNodes(nodeVo, startConditionsVo, nextId, mapPreNodes, setAddNodes);
            return;
        }

        paramsVo.ParamType = (int)nodeParamTypeEnum;
        var assigneeList = AssigneeListUniq(
            _bpmnPersonnelProviderService.GetAssigneeList(nodeVo, startConditionsVo));
        SetAssigneeOrList(paramsVo, assigneeList, nodeParamTypeEnum);
        SetEmployeeName(assigneeList, nodeName);
    }

    private void SetEmployeeName(List<BpmnNodeParamsAssigneeVo> assigneeList, string nodeName)
    {
        var ids = assigneeList.Select(a => a.Assignee).ToList();
        var empIdNameMap = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(ids);

        foreach (var assigneeVo in assigneeList)
        {
            if (empIdNameMap.TryGetValue(assigneeVo.Assignee, out var value))
            {
                assigneeVo.AssigneeName = value;
            }
        }
    }

    private void SetAssigneeOrList(BpmnNodeParamsVo paramsVo,
                                   List<BpmnNodeParamsAssigneeVo> assigneeList,
                                   BpmnNodeParamTypeEnum nodeParamTypeEnum)
    {
        if (paramsVo == null)
        {
            throw new InvalidOperationException("Parameter paramsVo cannot be null.");
        }

        if (assigneeList == null || !assigneeList.Any())
        {
            throw new InvalidOperationException("The assignee list must contain personnel information!");
        }

        if (nodeParamTypeEnum == BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_SINGLE)
        {
            paramsVo.Assignee = assigneeList.First();
        }
        else
        {
            paramsVo.AssigneeList = assigneeList;
        }
    }

    private List<BpmnNodeParamsAssigneeVo> AssigneeListUniq(List<BpmnNodeParamsAssigneeVo> paramsList)
    {
        var result = new List<BpmnNodeParamsAssigneeVo>();
        if (paramsList == null || !paramsList.Any())
        {
            return result;
        }

        var uniqList = new HashSet<string>();
        foreach (var vo in paramsList)
        {
            if (!uniqList.Contains(vo.Assignee))
            {
                result.Add(vo);
            }
            uniqList.Add(vo.Assignee);
        }

        return result;
    }

    public abstract void SetSupportBusinessObjects();
}