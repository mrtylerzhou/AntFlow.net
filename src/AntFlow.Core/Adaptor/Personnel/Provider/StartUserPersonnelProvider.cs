using AntFlow.Core.Configuration.DependencyInjection;
using AntFlow.Core.Exception;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel.Provider;

[NamedService(nameof(StartUserPersonnelProvider))]
public class StartUserPersonnelProvider : IBpmnPersonnelProviderService
{
    private readonly AssigneeVoBuildUtils _assigneeVoBuildUtils;

    public StartUserPersonnelProvider(AssigneeVoBuildUtils assigneeVoBuildUtils)
    {
        _assigneeVoBuildUtils = assigneeVoBuildUtils;
    }

    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo startConditionsVo)
    {
        if (startConditionsVo.StartUserId == null)
        {
            throw new AFBizException("未获取到发起人信息!");
        }

        string startUserId = startConditionsVo.StartUserId;
        string elementName = bpmnNodeVo.NodeName;

        List<BpmnNodeParamsAssigneeVo>? assigneeVos =
            _assigneeVoBuildUtils.BuildVos(new List<string> { startUserId }, elementName, false);
        return assigneeVos;
    }
}