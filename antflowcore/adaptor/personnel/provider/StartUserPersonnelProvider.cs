using antflowcore.conf.di;
using antflowcore.exception;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provider;

[NamedService(nameof(StartUserPersonnelProvider))]
public class StartUserPersonnelProvider : IBpmnPersonnelProviderService
{
    private readonly AssigneeVoBuildUtils _assigneeVoBuildUtils;

    public StartUserPersonnelProvider(AssigneeVoBuildUtils assigneeVoBuildUtils)
    {
        _assigneeVoBuildUtils = assigneeVoBuildUtils;
    }

    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
    {
        if (startConditionsVo.StartUserId == null)
        {
            throw new AFBizException("未获取到发起人信息!");
        }

        string startUserId = startConditionsVo.StartUserId;
        string elementName = bpmnNodeVo.NodeName;

        var assigneeVos = _assigneeVoBuildUtils.BuildVos(new List<string> { startUserId }, elementName, false);
        return assigneeVos;
    }
}