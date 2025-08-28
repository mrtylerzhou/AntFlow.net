using AntFlow.Core.Configuration.DependencyInjection;
using AntFlow.Core.Exception;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel.Provider;

[NamedService(nameof(UserPointedPersonnelProvider))]
public class UserPointedPersonnelProvider : IBpmnPersonnelProviderService
{
    private readonly AssigneeVoBuildUtils _assigneeVoBuildUtils;

    public UserPointedPersonnelProvider(AssigneeVoBuildUtils assigneeVoBuildUtils)
    {
        _assigneeVoBuildUtils = assigneeVoBuildUtils;
    }

    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo startConditionsVo)
    {
        if (bpmnNodeVo == null)
        {
            throw new AFBizException("node can not be null!");
        }

        BpmnNodePropertysVo propertysVo = bpmnNodeVo.Property;

        if (propertysVo == null || propertysVo.EmplIds == null || propertysVo.EmplIds.Count == 0)
        {
            throw new AFBizException("appointed assignee does not meet basic condition, can not go on");
        }

        string elementName = bpmnNodeVo.NodeName;
        if (string.IsNullOrEmpty(elementName))
        {
            elementName = "指定人员";
        }

        if (bpmnNodeVo.IsOutSideProcess != null && bpmnNodeVo.IsOutSideProcess == 1)
        {
            List<BaseIdTranStruVo> emplList = bpmnNodeVo.Property.EmplList;
            if (emplList == null || emplList.Count == 0)
            {
                throw new AFBizException("third party process role node has no employee info");
            }

            return _assigneeVoBuildUtils.BuildVOs(emplList, elementName, false);
        }

        List<string> emplIds = propertysVo.EmplIds;
        return _assigneeVoBuildUtils.BuildVos(emplIds, elementName, false);
    }
}