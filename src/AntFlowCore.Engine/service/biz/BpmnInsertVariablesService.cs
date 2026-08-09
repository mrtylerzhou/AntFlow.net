using System.Text.Json;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn;
using AntFlowCore.Bpmn.adaptor.variable;
using AntFlowCore.Bpmn.constants;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

/// <summary>
/// Inserts variable configuration for a BPMN process.
/// All button, sign-up, message, and approve-remind data is written to variable_config_json.
/// Old tables (t_bpm_variable_button, t_bpm_variable_view_page_button, t_bpm_variable_sign_up,
/// t_bpm_variable_message, t_bpm_variable_approve_remind) are no longer written.
/// </summary>
public class BpmnInsertVariablesService : IBpmnInsertVariablesService
{
    private readonly IBpmVariableService _bpmVariableService;
    private readonly ILogger<BpmnInsertVariablesService> _logger;

    public BpmnInsertVariablesService(
        IBpmVariableService bpmVariableService,
        ILogger<BpmnInsertVariablesService> logger
        )
    {
        _bpmVariableService = bpmVariableService;
        _logger = logger;
    }

    public void InsertVariables(BpmnConfCommonVo bpmnConfCommonVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        // 1. Insert variable first to get the ID (needed by multiplayer adaptors)
        var bpmVariable = new BpmVariable
        {
            BpmnCode = bpmnConfCommonVo.BpmnCode,
            ProcessNum = bpmnConfCommonVo.ProcessNum,
            ProcessName = bpmnConfCommonVo.ProcessName,
            ProcessDesc = bpmnConfCommonVo.ProcessDesc ?? "",
            ProcessStartConditions = JsonSerializer.Serialize(bpmnStartConditions, JsonConfUtil.Options),
            CreateUser = SecurityUtils.GetLogInEmpIdSafe(),
            CreateTime = DateTime.Now,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };

        _bpmVariableService._repository.Add(bpmVariable);

        var variableId = bpmVariable.Id;

        // 2. Build variable config JSON
        var variableConfig = new VariableConfigJson();

        // build view page buttons
        BuildViewPageButtons(bpmnConfCommonVo, variableConfig);

        var signUpMultimap = new Dictionary<string, List<BpmnConfCommonElementVo>>();
        var elementList = bpmnConfCommonVo.ElementList;

        foreach (var elementVo in elementList)
        {
            var elementType = elementVo.ElementType;
            var elementProperty = elementVo.ElementProperty;

            if (elementType == ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code)
            {
                // write to t_bpm_variable_multiplayer tables (kept, not part of JSON)
                var bpmnInsertVariableSubs = ElementPropertyEnum.GetVariableSubClassByCode(elementProperty);
                if (bpmnInsertVariableSubs != null)
                {
                    var insertVariableSubsService = (IBpmnInsertVariableSubs)ServiceProviderUtils.GetService(bpmnInsertVariableSubs);
                    insertVariableSubsService?.InsertVariableSubs(elementVo, variableId);
                }

                // set nodesignup data
                if (elementVo.IsSignUp == 1)
                {
                    if (!signUpMultimap.ContainsKey(elementVo.ElementId))
                    {
                        signUpMultimap[elementVo.ElementId] = new List<BpmnConfCommonElementVo>();
                    }
                    signUpMultimap[elementVo.ElementId].Add(new BpmnConfCommonElementVo());
                }

                if (elementVo.IsSignUpSubElement == 1)
                {
                    if (!signUpMultimap.ContainsKey(elementVo.SignUpElementId))
                        signUpMultimap[elementVo.SignUpElementId] = new List<BpmnConfCommonElementVo>();
                    signUpMultimap[elementVo.SignUpElementId].Add(elementVo);
                }

                // build element buttons to JSON
                BuildElementButtons(variableConfig, elementVo, elementVo.ElementId);
            }
        }

        // build signUp data
        VariableConfigHolder.AddSignUps(variableConfig, signUpMultimap, elementList);

        // build message and approveRemind data
        BuildMessagesAndApproveReminds(variableConfig, bpmnConfCommonVo);

        // 3. Update variable with config JSON
        bpmVariable.VariableConfigJson = JsonConfUtil.ToVariableConfigJson(variableConfig);
        _bpmVariableService._repository.Update(bpmVariable);
    }

    private void BuildElementButtons(VariableConfigJson config, BpmnConfCommonElementVo elementVo, string elementId)
    {
        if (elementVo.Buttons?.StartPage != null)
        {
            foreach (var o in elementVo.Buttons.StartPage)
            {
                config.Buttons.Add(new VariableButtonItem
                {
                    ElementId = elementId,
                    ButtonPageType = (int)ButtonPageTypeEnum.INITIATE,
                    ButtonType = o.ButtonType,
                    ButtonName = o.ButtonName
                });
            }
        }

        if (elementVo.Buttons?.ApprovalPage != null)
        {
            foreach (var o in elementVo.Buttons.ApprovalPage)
            {
                config.Buttons.Add(new VariableButtonItem
                {
                    ElementId = elementId,
                    ButtonPageType = (int)ButtonPageTypeEnum.AUDIT,
                    ButtonType = o.ButtonType,
                    ButtonName = o.ButtonName
                });
            }
        }

        if (elementVo.Buttons?.ViewPage != null)
        {
            foreach (var o in elementVo.Buttons.ViewPage)
            {
                config.Buttons.Add(new VariableButtonItem
                {
                    ElementId = elementId,
                    ButtonPageType = (int)ButtonPageTypeEnum.TOVIEW,
                    ViewType = (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_OTHER,
                    ButtonType = o.ButtonType,
                    ButtonName = o.ButtonName
                });
            }
        }
    }

    private void BuildViewPageButtons(BpmnConfCommonVo bpmnConfCommonVo, VariableConfigJson config)
    {
        if (bpmnConfCommonVo.ViewPageButtons?.ViewPageStart != null)
        {
            foreach (var o in bpmnConfCommonVo.ViewPageButtons.ViewPageStart)
            {
                config.Buttons.Add(new VariableButtonItem
                {
                    ButtonPageType = 3,
                    ViewType = (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_START,
                    ButtonType = o.ButtonType,
                    ButtonName = o.ButtonName
                });
            }
        }

        if (bpmnConfCommonVo.ViewPageButtons?.ViewPageOther != null)
        {
            foreach (var o in bpmnConfCommonVo.ViewPageButtons.ViewPageOther)
            {
                config.Buttons.Add(new VariableButtonItem
                {
                    ButtonPageType = 3,
                    ViewType = (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_OTHER,
                    ButtonType = o.ButtonType,
                    ButtonName = o.ButtonName
                });
            }
        }
    }

    private void BuildMessagesAndApproveReminds(VariableConfigJson config, BpmnConfCommonVo bpmnConfCommonVo)
    {
        // out-of-node messages
        if (!bpmnConfCommonVo.TemplateVos.IsEmpty())
        {
            VariableConfigHolder.AddMessages(config, bpmnConfCommonVo.TemplateVos, string.Empty, 1);
        }

        // in-node messages and approve reminds
        if (!bpmnConfCommonVo.ElementList.IsEmpty())
        {
            foreach (var elementVo in bpmnConfCommonVo.ElementList)
            {
                if (elementVo.TemplateVos.IsEmpty())
                {
                    continue;
                }
                VariableConfigHolder.AddMessages(config, elementVo.TemplateVos, elementVo.ElementId, 2);

                if (elementVo.ApproveRemindVo?.StandardMinutes != null
                    && !elementVo.ApproveRemindVo.Days.IsEmpty())
                {
                    VariableConfigHolder.AddApproveRemind(config, elementVo.ElementId, elementVo.ApproveRemindVo);
                }
            }
        }
    }
}
