using AntFlowCore.Abstraction.service;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

public class NodePropertyPersonnelAdaptor : AbstractAdditionSignNodeAdaptor
{
    private readonly IBpmnEmployeeInfoProviderService _bpmnEmployeeInfoProviderService;

    public NodePropertyPersonnelAdaptor(
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService,
        IRoleService roleService
        ) : base(roleService)
    {
        _bpmnEmployeeInfoProviderService = bpmnEmployeeInfoProviderService;
    }

    public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        base.FormatToBpmnNodeVo(bpmnNodeVo);

        // Prefer JSON config if available
        var nodeConfig = bpmnNodeVo.NodeConfigJsonObj;
        if (nodeConfig?.ApproverConf?.PersonnelConf != null)
        {
            var pc = nodeConfig.ApproverConf.PersonnelConf;
            var emplIds = new List<string>();
            var emplNames = new List<string>();
            if (pc.Employees != null && pc.Employees.Count > 0)
            {
                foreach (var e in pc.Employees)
                {
                    emplIds.Add(e.EmplId);
                    if (!string.IsNullOrEmpty(e.EmplName))
                    {
                        emplNames.Add(e.EmplName);
                    }
                }
            }

            AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p =>
            {
                p.SignType = pc.SignType;
                p.ArbitrationRatio = pc.ArbitrationRatio;
                p.EmplIds = emplIds;
                p.EmplList = GetEmplList(emplIds, emplNames);
            });
            return;
        }

        throw new AFBizException("migration error,please contact the author");
    }

    /// <summary>
    /// Get employee list.
    /// If emplNames is not empty, it is stored in the database and then loaded.
    /// </summary>
    /// <param name="emplIds">List of employee IDs.</param>
    /// <param name="emplNames">List of employee names.</param>
    /// <returns>List of BaseIdTranStruVo objects.</returns>
    private List<BaseIdTranStruVo> GetEmplList(List<string> emplIds, List<string> emplNames)
    {
        var result = new List<BaseIdTranStruVo>();

        if (emplNames != null && emplNames.Count > 0)
        {
            if (emplIds.Count != emplNames.Count)
            {
                throw new AFBizException("指定人员审批存在姓名不存在的人员!");
            }

            for (int i = 0; i < emplIds.Count; i++)
            {
                var vo = new BaseIdTranStruVo
                {
                    Id = emplIds[i],
                    Name = emplNames[i]
                };
                result.Add(vo);
            }

            return result;
        }

        var employeeInfos = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(emplIds);

        foreach (var emplId in emplIds)
        {
            var vo = new BaseIdTranStruVo
            {
                Id = emplId,
                Name = employeeInfos.TryGetValue(emplId, out var empName) ? empName : null
            };
            result.Add(vo);
        }

        return result;
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(
            BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_PERSONNEL,
            BpmnNodeAdpConfEnum.ADP_CONF_NODE_TYPE_COPY);
    }
}
