using AntFlowCore.Abstraction;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.conf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.personnel.provider;
[NamedService(nameof(UserPointedPersonnelProvider))]
public class UserPointedPersonnelProvider : IBpmnPersonnelProviderService
{
    private readonly AssigneeVoBuildUtils _assigneeVoBuildUtils;

    public UserPointedPersonnelProvider(AssigneeVoBuildUtils assigneeVoBuildUtils)
    {
        _assigneeVoBuildUtils = assigneeVoBuildUtils;
    }

    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo startConditionsVo)
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

        List<BaseIdTranStruVo> emplList = bpmnNodeVo.Property.EmplList;

        if (bpmnNodeVo.IsOutSideProcess != null && bpmnNodeVo.IsOutSideProcess == 1)
        {
            if (emplList == null || emplList.Count == 0)
            {
                throw new AFBizException("third party process role node has no employee info");
            }

            return _assigneeVoBuildUtils.BuildVOs(emplList, elementName, false);
        }

        // 使用 emplList 构建指派人(对应Java: buildVOs 而非 buildVos),
        // 这样自动节点的虚拟指派人 AUTO_NODE_SKIP(-3) 不需要经过员工信息查询
        return _assigneeVoBuildUtils.BuildVOs(emplList, elementName, false);
    }
}