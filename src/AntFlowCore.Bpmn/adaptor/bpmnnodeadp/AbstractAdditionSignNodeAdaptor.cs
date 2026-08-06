using System.Text.Json;
using AntFlowCore.Base.adaptor.bpmnnodeadp;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

public abstract class AbstractAdditionSignNodeAdaptor : IBpmnNodeAdaptor
{
    private readonly IRoleService _roleService;

    public AbstractAdditionSignNodeAdaptor(IRoleService roleService)
    {
        _roleService = roleService;
    }

    public virtual void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        int? extraFlags = bpmnNodeVo.ExtraFlags;
        if (extraFlags == null)
        {
            return;
        }

        // Prefer JSON config if available
        var nodeConfig = bpmnNodeVo.NodeConfigJsonObj;
        if (nodeConfig?.ButtonSignConf?.AdditionalSignConfList != null && nodeConfig.ButtonSignConf.AdditionalSignConfList.Count > 0)
        {
            FormatAdditionalSignFromJson(bpmnNodeVo, nodeConfig.ButtonSignConf.AdditionalSignConfList);
        }
    }

    private void FormatAdditionalSignFromJson(BpmnNodeVo bpmnNodeVo, List<ButtonSignAdditionalSignConf> addSignConfs)
    {
        var additionalSignInfoList = new List<ExtraSignInfoVo>();
        foreach (var conf in addSignConfs)
        {
            var baseIdTranStruVos = JsonSerializer.Deserialize<List<BaseIdTranStruVo>>(conf.SignInfos ?? "[]");
            var extraSignInfoVo = new ExtraSignInfoVo
            {
                NodeProperty = conf.SignProperty,
                PropertyType = conf.SignPropertyType,
                SignInfos = baseIdTranStruVos
            };
            if (conf.SignPropertyType == (int)NodePropertyEnum.NODE_PROPERTY_ROLE)
            {
                var roleIds = baseIdTranStruVos.Select(a => a.Id).ToList();
                var roleInfos = _roleService.QueryUserByRoleIds(roleIds);
                extraSignInfoVo.OtherSignInfos = roleInfos;
            }
            additionalSignInfoList.Add(extraSignInfoVo);
        }
        AfNodeUtils.AddOrEditProperty(bpmnNodeVo, a => a.AdditionalSignInfoList = additionalSignInfoList);
    }

    public virtual void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
    }

    public abstract void SetSupportBusinessObjects();
}
