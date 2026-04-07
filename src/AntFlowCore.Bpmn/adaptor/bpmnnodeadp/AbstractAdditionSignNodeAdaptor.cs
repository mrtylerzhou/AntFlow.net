using System.Text.Json;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.util;
using AntFlowCore.Common.util.Extension;
using AntFlowCore.Core.adaptor.bpmnnodeadp;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

public abstract class AbstractAdditionSignNodeAdaptor : IBpmnNodeAdaptor
{
    private readonly IBpmnNodeAdditionalSignConfService _bpmnNodeAdditionalSignConfService;
    private readonly IRoleService _roleService;

    public AbstractAdditionSignNodeAdaptor(IBpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
        IRoleService roleService)
    {
        _bpmnNodeAdditionalSignConfService = bpmnNodeAdditionalSignConfService;
        _roleService = roleService;
    }
    public virtual void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        int? extraFlags = bpmnNodeVo.ExtraFlags;
        if (extraFlags == null)
        {
            return;
        }

        List<BpmnNodeAdditionalSignConf> bpmnNodeAdditionalSignConfs = _bpmnNodeAdditionalSignConfService
            .GetByBpmnNodeId(bpmnNodeVo.Id);
        if (bpmnNodeAdditionalSignConfs.IsEmpty())
        {
            return;
        }
        BpmnNodePropertysVo bpmnNodePropertysVo = new BpmnNodePropertysVo();
        List<ExtraSignInfoVo> additionalSignInfoList = new List<ExtraSignInfoVo>();
        foreach (BpmnNodeAdditionalSignConf bpmnNodeAdditionalSignConf in bpmnNodeAdditionalSignConfs)
        {
            int? signProperty = bpmnNodeAdditionalSignConf.SignProperty;
            int? signPropertyType = bpmnNodeAdditionalSignConf.SignPropertyType;
            String signInfos = bpmnNodeAdditionalSignConf.SignInfos;
            List<BaseIdTranStruVo> baseIdTranStruVos = JsonSerializer.Deserialize<List<BaseIdTranStruVo>>(signInfos);
            
            ExtraSignInfoVo extraSignInfoVo = new ExtraSignInfoVo();
            extraSignInfoVo.NodeProperty=signProperty;
            extraSignInfoVo.PropertyType=signPropertyType;
            extraSignInfoVo.SignInfos=baseIdTranStruVos;
            if (signPropertyType==(int)NodePropertyEnum.NODE_PROPERTY_ROLE)
            {
                List<String> roleIds = baseIdTranStruVos.Select(a => a.Id).ToList();
                
                List<BaseIdTranStruVo> roleInfos = _roleService.QueryUserByRoleIds(roleIds);//这个是用于存储角色下的人信息的
                extraSignInfoVo.OtherSignInfos=roleInfos;
            }
            additionalSignInfoList.Add(extraSignInfoVo);
        }
        bpmnNodePropertysVo.AdditionalSignInfoList=additionalSignInfoList;//这个是给前端用于反显设计数据的,其中指定人员直接从这里获取
        AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p=> p.AdditionalSignInfoList=additionalSignInfoList);
    }

    public virtual void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        int? extraFlags = bpmnNodeVo.ExtraFlags;
        if (extraFlags == null) {
            return;
        }
        List<ExtraSignInfoVo> additionalSignInfoList = bpmnNodeVo.Property.AdditionalSignInfoList;
        if (additionalSignInfoList.IsEmpty()) {
            return;
        }
        
       List<BpmnNodeAdditionalSignConf> bpmnNodeAdditionalSignConfs = new System.Collections.Generic.List<BpmnNodeAdditionalSignConf>();
       
        foreach (ExtraSignInfoVo extraSignInfoVo in additionalSignInfoList) {
            
            int? signProperty = extraSignInfoVo.NodeProperty;
            int? signPropertyType = extraSignInfoVo.PropertyType;
            List<BaseIdTranStruVo> signInfos = extraSignInfoVo.SignInfos;
            String signInfosStr = JsonSerializer.Serialize(signInfos);

            BpmnNodeAdditionalSignConf bpmnNodeAdditionalSignConf = new BpmnNodeAdditionalSignConf();
            bpmnNodeAdditionalSignConf.BpmnNodeId=bpmnNodeVo.Id;
            bpmnNodeAdditionalSignConf.SignInfos=signInfosStr;
            bpmnNodeAdditionalSignConf.SignProperty=signProperty;
            bpmnNodeAdditionalSignConf.SignPropertyType=signPropertyType;
            bpmnNodeAdditionalSignConf.CreateUser=SecurityUtils.GetLogInEmpIdStr();
            bpmnNodeAdditionalSignConf.CreateTime=DateTime.Now;
            bpmnNodeAdditionalSignConf.UpdateUser=(SecurityUtils.GetLogInEmpIdStr());
            bpmnNodeAdditionalSignConf.UpdateTime=DateTime.Now;
            bpmnNodeAdditionalSignConfs.Add(bpmnNodeAdditionalSignConf);
        }
        _bpmnNodeAdditionalSignConfService.Insert(bpmnNodeAdditionalSignConfs);
    }


    public abstract void SetSupportBusinessObjects();
}