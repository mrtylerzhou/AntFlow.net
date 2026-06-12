using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.bpmnnodeadp;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

public class NodePropertyLoopAdaptor : IBpmnNodeAdaptor
{
    private readonly IBpmnEmployeeInfoProviderService _bpmnEmployeeInfoProviderService;
    private readonly IRoleService _roleService;
    private readonly ILogger<NodePropertyLoopAdaptor> _logger;

    public NodePropertyLoopAdaptor(
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService,
        IRoleService roleService,
        ILogger<NodePropertyLoopAdaptor> logger)
    {
        _bpmnEmployeeInfoProviderService = bpmnEmployeeInfoProviderService;
        _roleService = roleService;
        _logger = logger;
    }

    public void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        // Prefer JSON config if available
        var nodeConfig = bpmnNodeVo.NodeConfigJsonObj;
        if (nodeConfig?.ApproverConf?.LoopConf != null)
        {
            var lc = nodeConfig.ApproverConf.LoopConf;

            List<string> list = !string.IsNullOrEmpty(lc.LoopEndPerson)
                ? lc.LoopEndPerson.Split(',').ToList()
                : new List<string>();

            List<string> noList = !string.IsNullOrEmpty(lc.NoparticipatingStaffIds)
                ? lc.NoparticipatingStaffIds.Split(',').ToList()
                : new List<string>();

            var loopEndPersonList = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(list)
                .Select(entry => new BaseIdTranStruVo
                {
                    Id = entry.Key,
                    Name = entry.Value
                }).ToList();

            var noparticipatingStaffs = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(noList)
                .Select(entry => new BaseIdTranStruVo
                {
                    Id = entry.Key,
                    Name = entry.Value
                }).ToList();

            bpmnNodeVo.Property = new BpmnNodePropertysVo
            {
                LoopEndType = lc.LoopEndType,
                LoopNumberPlies = lc.LoopNumberPlies,
                LoopEndGrade = lc.LoopEndGrade,
                LoopEndPersonList = list,
                LoopEndPersonObjList = loopEndPersonList,
                NoparticipatingStaffIds = noList,
                NoparticipatingStaffs = noparticipatingStaffs
            };
            bpmnNodeVo.OrderedNodeType = (int)OrderNodeTypeEnum.LOOP_NODE;
        }
        else
        {
            throw new AFBizException("migration error,please contact the author");
        }
    }

    public void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        // Write path is now handled by BpmnNodeConfigHolder
    }

    public PersonnelRuleVo FormaFieldAttributeInfoVO()
    {
        return new PersonnelRuleVo();
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_LOOP);
    }
}
