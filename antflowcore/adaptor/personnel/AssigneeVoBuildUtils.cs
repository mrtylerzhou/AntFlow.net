using antflowcore.exception;
using antflowcore.service;
using antflowcore.vo;

namespace antflowcore.adaptor.personnel;

using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

public class AssigneeVoBuildUtils
{
    private readonly ILogger<AssigneeVoBuildUtils> _logger;
    private readonly IBpmnEmployeeInfoProviderService _bpmnEmployeeInfoProviderService;

    public AssigneeVoBuildUtils(ILogger<AssigneeVoBuildUtils> logger, IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService)
    {
        _logger = logger;
        _bpmnEmployeeInfoProviderService = bpmnEmployeeInfoProviderService;
    }

    public List<BpmnNodeParamsAssigneeVo> BuildVOs(IEnumerable<BaseIdTranStruVo> assigneeInfos, string nodeName, bool hasSuffix)
    {
        if (string.IsNullOrEmpty(nodeName))
        {
            nodeName = "层层审批";
        }

        var assigneeVos = new List<BpmnNodeParamsAssigneeVo>();
        int index = 0;

        foreach (var assigneeInfo in assigneeInfos)
        {
            index++;
            string nameSuffix = $"_{index}";

            var bpmnNodeParamsAssigneeVo = new BpmnNodeParamsAssigneeVo
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
        var assigneeInfos = new List<BaseIdTranStruVo>();
        var employeeInfo = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(assignees);

        if (employeeInfo == null || !employeeInfo.Any())
        {
            throw new AFBizException("未能根据指定id找到人员信息!");
        }

        foreach (var assignee in assignees)
        {
            if (employeeInfo.TryGetValue(assignee, out var assigneeName) && !string.IsNullOrEmpty(assigneeName))
            {
                assigneeInfos.Add(new BaseIdTranStruVo
                {
                    Id = assignee,
                    Name = assigneeName
                });
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
        var employeeInfo = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(new List<string> { assignee });

        if (employeeInfo == null || !employeeInfo.Any())
        {
            throw new AFBizException("未能根据指定id找到人员信息!");
        }

        var assigneeName = employeeInfo.TryGetValue(assignee, out var name) ? name : null;

        return new BpmnNodeParamsAssigneeVo
        {
            Assignee = assignee,
            AssigneeName = assigneeName,
            ElementName = nodeName
        };
    }

    public BpmnNodeParamsAssigneeVo BuildZeroVo()
    {
        return new BpmnNodeParamsAssigneeVo
        {
            Assignee = "0",
            IsDeduplication = 0
        };
    }
}