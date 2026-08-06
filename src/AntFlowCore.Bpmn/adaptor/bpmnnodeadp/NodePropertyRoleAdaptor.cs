using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

public class NodePropertyRoleAdaptor : AbstractAdditionSignNodeAdaptor
{
    public NodePropertyRoleAdaptor(
        IRoleService roleService
        ) : base(roleService)
    {
    }

    public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        base.FormatToBpmnNodeVo(bpmnNodeVo);

        // Prefer JSON config if available
        var nodeConfig = bpmnNodeVo.NodeConfigJsonObj;
        if (nodeConfig?.ApproverConf?.RoleConfList != null && nodeConfig.ApproverConf.RoleConfList.Count > 0)
        {
            var roleConfs = nodeConfig.ApproverConf.RoleConfList;
            var roles = roleConfs
                .Select(rc => new BaseIdTranStruVo { Id = rc.RoleId, Name = rc.RoleName })
                .ToList();

            AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p =>
            {
                p.RoleIds = roles.Select(r => r.Id).ToList();
                p.RoleList = roles;
                p.SignType = roleConfs[0].SignType;
                p.ArbitrationRatio = roleConfs[0].ArbitrationRatio;
            });

            // Outside employees for role
            if (bpmnNodeVo.IsOutSideProcess == 1)
            {
                var firstRole = roleConfs[0];
                if (firstRole.OutsideEmployees != null && firstRole.OutsideEmployees.Count > 0)
                {
                    bpmnNodeVo.Property.EmplIds = firstRole.OutsideEmployees
                        .Select(e => e.EmplId).Where(id => id != null).ToList()!;
                    bpmnNodeVo.Property.EmplList = firstRole.OutsideEmployees
                        .Select(e => new BaseIdTranStruVo { Id = e.EmplId!, Name = e.EmplName })
                        .ToList();
                }
            }

            return;
        }

        throw new AFBizException("migration error,please contact the author");
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_ROLE);
    }
}
