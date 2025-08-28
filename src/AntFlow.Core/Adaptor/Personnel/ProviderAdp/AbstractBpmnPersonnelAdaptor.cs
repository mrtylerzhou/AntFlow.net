using AntFlow.Core.Adaptor.Personnel.Provider;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Service;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel;

/// <summary>
///     Abstract personnel adaptor to find personnel.
///     AntFlow mainly has three extension points: node adaptor, element adaptor, and personnel adaptor.
/// </summary>
public abstract class AbstractBpmnPersonnelAdaptor : IAdaptorService
{
    private readonly IBpmnEmployeeInfoProviderService _bpmnEmployeeInfoProviderService;
    private readonly IBpmnPersonnelProviderService _bpmnPersonnelProviderService;
    private readonly IServiceProvider _serviceProvider;

    public AbstractBpmnPersonnelAdaptor(
        IBpmnPersonnelProviderService bpmnPersonnelProviderService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService
    )
    {
        _bpmnEmployeeInfoProviderService = bpmnEmployeeInfoProviderService ??
                                           throw new ArgumentNullException(nameof(bpmnEmployeeInfoProviderService));
        _bpmnPersonnelProviderService = bpmnPersonnelProviderService ??
                                        throw new ArgumentNullException(nameof(bpmnPersonnelProviderService));
    }

    public abstract void SetSupportBusinessObjects();

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

        BpmnNodeParamsVo? paramsVo = nodeVo.Params;
        string? nodeName = nodeVo.NodeName;

        if (paramsVo == null)
        {
            throw new InvalidOperationException("Parameter paramsVo cannot be null.");
        }

        int? orderedNodeType = nodeVo.OrderedNodeType;
        if (orderedNodeType.HasValue)
        {
            OrderNodeTypeEnum? orderNodeTypeEnum = OrderNodeTypeEnumExtensions.GetByCode(orderedNodeType.Value);
            IEnumerable<AbstractOrderedSignNodeAdp>? abstractOrderedSignNodeAdps =
                ServiceProviderUtils.GetServices<AbstractOrderedSignNodeAdp>();
            AbstractOrderedSignNodeAdp orderedSignNodeAdp = null;

            foreach (AbstractOrderedSignNodeAdp? abstractOrderedSignNodeAdp in abstractOrderedSignNodeAdps)
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
        List<BpmnNodeParamsAssigneeVo>? assigneeList = AssigneeListUniq(
            _bpmnPersonnelProviderService.GetAssigneeList(nodeVo, startConditionsVo));
        SetAssigneeOrList(paramsVo, assigneeList, nodeParamTypeEnum);
        SetEmployeeName(assigneeList, nodeName);
    }

    private void SetEmployeeName(List<BpmnNodeParamsAssigneeVo> assigneeList, string nodeName)
    {
        List<string>? ids = assigneeList.Select(a => a.Assignee).ToList();
        Dictionary<string, string>? empIdNameMap = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(ids);

        foreach (BpmnNodeParamsAssigneeVo? assigneeVo in assigneeList)
        {
            if (empIdNameMap.TryGetValue(assigneeVo.Assignee, out string? value))
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
        List<BpmnNodeParamsAssigneeVo>? result = new();
        if (paramsList == null || !paramsList.Any())
        {
            return result;
        }

        HashSet<string>? uniqList = new();
        foreach (BpmnNodeParamsAssigneeVo? vo in paramsList)
        {
            if (!uniqList.Contains(vo.Assignee))
            {
                result.Add(vo);
            }

            uniqList.Add(vo.Assignee);
        }

        return result;
    }
}