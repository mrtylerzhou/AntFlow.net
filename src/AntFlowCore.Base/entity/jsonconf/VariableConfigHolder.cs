using System.Text.Json;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.entity.jsonconf;

public static class VariableConfigHolder
{
    public static void AddElementButtons(
        VariableConfigJson config,
        BpmnConfCommonElementVo elementVo,
        string elementId)
    {
        AddButtons(config, elementVo.Buttons?.StartPage, elementId, (int)ButtonPageTypeEnum.INITIATE);
        AddButtons(config, elementVo.Buttons?.ApprovalPage, elementId, (int)ButtonPageTypeEnum.AUDIT);
        AddButtons(config, elementVo.Buttons?.ViewPage, elementId, (int)ButtonPageTypeEnum.TOVIEW);
    }

    public static void AddViewPageButtons(VariableConfigJson config, BpmnConfViewPageButtonVo? viewPageButtons)
    {
        AddViewButtons(config, viewPageButtons?.ViewPageStart, (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_START);
        AddViewButtons(config, viewPageButtons?.ViewPageOther, (int)ViewPageTypeEnum.VIEW_PAGE_TYPE_OTHER);
    }

    public static void AddSignUps(
        VariableConfigJson config,
        Dictionary<string, List<BpmnConfCommonElementVo>> signUpMultimap,
        List<BpmnConfCommonElementVo> elementList)
    {
        foreach (var key in signUpMultimap.Keys)
        {
            var elementVo = elementList.FirstOrDefault(o => o.ElementId == key) ?? new BpmnConfCommonElementVo();
            var subElements = signUpMultimap[key]
                .Where(o => o != null && !string.IsNullOrEmpty(o.ElementId))
                .ToList();

            config.SignUps.Add(new VariableSignUpItem
            {
                ElementId = key,
                NodeId = elementVo.NodeId,
                AfterSignUpWay = elementVo.AfterSignUpWay,
                SubElements = JsonSerializer.Serialize(subElements, JsonConfUtil.Options)
            });
        }
    }

    public static void AddMessages(
        VariableConfigJson config,
        List<BpmnTemplateVo>? templateVos,
        string elementId,
        int messageType)
    {
        if (templateVos.IsEmpty())
        {
            return;
        }

        config.Messages.AddRange(templateVos.Select(o => new VariableMessageItem
        {
            ElementId = elementId,
            MessageType = 2,
            EventType = o.Event,
            Content = JsonSerializer.Serialize(o, JsonConfUtil.Options)
        }));
    }

    public static void AddApproveRemind(
        VariableConfigJson config,
        string elementId,
        BpmnApproveRemindVo? approveRemindVo)
    {
        if (approveRemindVo?.Days == null)
        {
            return;
        }

        config.ApproveReminds.Add(new VariableApproveRemindItem
        {
            ElementId = elementId,
            Content = JsonSerializer.Serialize(approveRemindVo, JsonConfUtil.Options)
        });
    }

    private static void AddButtons(
        VariableConfigJson config,
        List<BpmnConfCommonButtonPropertyVo>? buttons,
        string elementId,
        int buttonPageType)
    {
        if (buttons == null || buttons.Count == 0)
        {
            return;
        }

        config.Buttons.AddRange(buttons.Select(o => new VariableButtonItem
        {
            ElementId = elementId,
            ButtonPageType = buttonPageType,
            ButtonType = o.ButtonType,
            ButtonName = o.ButtonName
        }));
    }

    private static void AddViewButtons(
        VariableConfigJson config,
        List<BpmnConfCommonButtonPropertyVo>? buttons,
        int viewType)
    {
        if (buttons == null || buttons.Count == 0)
        {
            return;
        }

        config.Buttons.AddRange(buttons.Select(o => new VariableButtonItem
        {
            ElementId = string.Empty,
            ButtonPageType = (int)ButtonPageTypeEnum.TOVIEW,
            ViewType = viewType,
            ButtonType = o.ButtonType,
            ButtonName = o.ButtonName
        }));
    }

    private static int GetMessageSendType(int messageEvent, int defaultMessageSendType)
    {
        var eventTypeEnum = (EventTypeEnum)messageEvent;
        EventTypeEnumExtensions.EventTypeMappings.TryGetValue(eventTypeEnum, out var eventTypeMapping);
        return eventTypeMapping?.IsInNode == true ? 2 : defaultMessageSendType;
    }
}
