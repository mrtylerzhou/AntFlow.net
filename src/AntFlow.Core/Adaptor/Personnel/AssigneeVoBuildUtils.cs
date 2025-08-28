using AntFlow.Core.Exception;
using AntFlow.Core.Service;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel;

public class AssigneeVoBuildUtils
{
    private readonly IBpmnEmployeeInfoProviderService _bpmnEmployeeInfoProviderService;
    private readonly ILogger<AssigneeVoBuildUtils> _logger;

    public AssigneeVoBuildUtils(ILogger<AssigneeVoBuildUtils> logger,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService)
    {
        _logger = logger;
        _bpmnEmployeeInfoProviderService = bpmnEmployeeInfoProviderService;
    }

    public List<BpmnNodeParamsAssigneeVo> BuildVOs(IEnumerable<BaseIdTranStruVo> assigneeInfos, string nodeName,
        bool hasSuffix)
    {
        if (string.IsNullOrEmpty(nodeName))
        {
            nodeName = "�������";
        }

        List<BpmnNodeParamsAssigneeVo>? assigneeVos = new();
        int index = 0;

        foreach (BaseIdTranStruVo? assigneeInfo in assigneeInfos)
        {
            index++;
            string nameSuffix = $"_{index}";

            BpmnNodeParamsAssigneeVo? bpmnNodeParamsAssigneeVo = new()
            {
                Assignee = assigneeInfo.Id,
                AssigneeName = assigneeInfo.Name,
                ElementName = hasSuffix ? nodeName + nameSuffix : nodeName
            };

            assigneeVos.Add(bpmnNodeParamsAssigneeVo);
        }

        return assigneeVos;
    }

    public List<BpmnNodeParamsAssigneeVo> BuildVos(IEnumerable<string> assignees, string nodeName, bool hasSuffix)
    {
        List<BaseIdTranStruVo>? assigneeInfos = new();
        Dictionary<string, string>? employeeInfo = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(assignees);

        if (employeeInfo == null || !employeeInfo.Any())
        {
            throw new AFBizException("δ�ܸ���ָ��id�ҵ���Ա��Ϣ!");
        }

        foreach (string? assignee in assignees)
        {
            if (employeeInfo.TryGetValue(assignee, out string? assigneeName) && !string.IsNullOrEmpty(assigneeName))
            {
                assigneeInfos.Add(new BaseIdTranStruVo { Id = assignee, Name = assigneeName });
            }
            else
            {
                _logger.LogWarning("Cannot get employee name by its id: {Assignee}, it might be invalid.", assignee);
            }
        }

        return BuildVOs(assigneeInfos, nodeName, hasSuffix);
    }

    public BpmnNodeParamsAssigneeVo BuildVo(string assignee, string nodeName)
    {
        Dictionary<string, string>? employeeInfo =
            _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(new List<string> { assignee });

        if (employeeInfo == null || !employeeInfo.Any())
        {
            throw new AFBizException("δ�ܸ���ָ��id�ҵ���Ա��Ϣ!");
        }

        string? assigneeName = employeeInfo.TryGetValue(assignee, out string? name) ? name : null;

        return new BpmnNodeParamsAssigneeVo
        {
            Assignee = assignee, AssigneeName = assigneeName, ElementName = nodeName
        };
    }

    public BpmnNodeParamsAssigneeVo BuildZeroVo()
    {
        return new BpmnNodeParamsAssigneeVo { Assignee = "0", IsDeduplication = 0 };
    }
}