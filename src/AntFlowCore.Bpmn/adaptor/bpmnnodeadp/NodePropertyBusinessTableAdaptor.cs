using System.Text.Json;
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

public class NodePropertyBusinessTableAdaptor : AbstractAdditionSignNodeAdaptor
{
    private readonly ILogger<NodePropertyBusinessTableAdaptor> _logger;

    public NodePropertyBusinessTableAdaptor(
        IRoleService roleService,
        ILogger<NodePropertyBusinessTableAdaptor> logger) : base(roleService)
    {
        _logger = logger;
    }

    public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        base.FormatToBpmnNodeVo(bpmnNodeVo);

        // Prefer JSON config if available
        var nodeConfig = bpmnNodeVo.NodeConfigJsonObj;
        if (nodeConfig?.ApproverConf?.BusinessTableConf != null)
        {
            var btc = nodeConfig.ApproverConf.BusinessTableConf;
            AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p =>
            {
                p.ConfigurationTableType = btc.ConfigurationTableType;
                p.TableFieldType = btc.TableFieldType;
                p.SignType = btc.SignType;
            });
            return;
        }

        throw new AFBizException("migration error,please contact the author");
    }

    public PersonnelRuleVo FormaFieldAttributeInfoVO()
    {
        var vo = new PersonnelRuleVo();
        vo.NodePropertyName = "关联业务表";
        vo.NodeProperty = (int)NodePropertyEnum.NODE_PROPERTY_BUSINESSTABLE;

        var tableChoices = new FieldAttributeInfoVO();
        tableChoices.FieldLabel = "请选择配置表";
        tableChoices.FieldName = "configurationTableType";
        tableChoices.Sort = 1;
        var collect = Enum.GetValues<ConfigurationTableEnum>()
            .Select(a => new BaseIdTranStruVo
            {
                Id = ((int)a).ToString(),
                Name = a.GetDesc()
            }).ToList();
        tableChoices.Value = collect;

        var tableFieldChoice = new FieldAttributeInfoVO();
        tableFieldChoice.FieldLabel = "请选择配置表字段";
        tableFieldChoice.FieldName = "tableFieldType";
        tableFieldChoice.Sort = 2;
        var choices = new Dictionary<string, List<BaseIdTranStruVo>>();
        foreach (var value in Enum.GetValues<ConfigurationTableEnum>())
        {
            tableFieldChoice.FieldLabel = value.GetDesc();
            tableFieldChoice.FieldValue = ((int)value).ToString();
            var tableFields = BusinessConfTableFieldEnumExtensions.GetByParentTable(value);
            var baseIdTranStruVoList = tableFields
                .Select(a => new BaseIdTranStruVo
                {
                    Id = ((int)a).ToString(),
                    Name = a.GetDesc()
                }).ToList();
            choices[((int)value).ToString()] = baseIdTranStruVoList;
        }
        tableFieldChoice.Value = choices;

        vo.FieldInfos = new List<FieldAttributeInfoVO> { tableChoices, tableFieldChoice };
        return vo;
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_BUSINESSTABLE);
    }
}
