using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

public class NodePropertyBusinessTableAdaptor : AbstractAdditionSignNodeAdaptor
{
    private readonly IBpmnNodeBusinessTableConfService _bpmnNodeBusinessTableConfService;
    private readonly ILogger<NodePropertyBusinessTableAdaptor> _logger;

    public NodePropertyBusinessTableAdaptor(
        IBpmnNodeBusinessTableConfService bpmnNodeBusinessTableConfService,
        IBpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
        IRoleService roleService,
        ILogger<NodePropertyBusinessTableAdaptor> logger) : base(bpmnNodeAdditionalSignConfService, roleService)
    {
        _bpmnNodeBusinessTableConfService = bpmnNodeBusinessTableConfService;
        _logger = logger;
    }

    public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        base.FormatToBpmnNodeVo(bpmnNodeVo);
        var bpmnNodeBusinessTableConf = _bpmnNodeBusinessTableConfService.GetByBpmnNodeId(bpmnNodeVo.Id);

        if (bpmnNodeBusinessTableConf != null)
        {
            AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p =>
            {
                p.ConfigurationTableType = bpmnNodeBusinessTableConf.ConfigurationTableType;
                p.TableFieldType = bpmnNodeBusinessTableConf.TableFieldType;
                p.SignType = bpmnNodeBusinessTableConf.SignType;
            });
        }
            
    }

   
    public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        base.EditBpmnNode(bpmnNodeVo);
        var bpmnNodePropertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

        var bpmnNodeBusinessTableConf = new BpmnNodeBusinessTableConf
        {
            BpmnNodeId = bpmnNodeVo.Id,
            ConfigurationTableType = bpmnNodePropertysVo.ConfigurationTableType,
            TableFieldType = bpmnNodePropertysVo.TableFieldType,
            SignType = bpmnNodePropertysVo.SignType,
            CreateTime = DateTime.Now,
            CreateUser = SecurityUtils.GetLogInEmpName(),
            UpdateTime = DateTime.Now,
            UpdateUser = SecurityUtils.GetLogInEmpName(),
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };

        _bpmnNodeBusinessTableConfService.Insert(bpmnNodeBusinessTableConf);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_BUSINESSTABLE);
    }
}