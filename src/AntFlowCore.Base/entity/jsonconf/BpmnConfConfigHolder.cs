using System.Text.Json;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Base.entity.jsonconf;

public static class BpmnConfConfigHolder
{
    public static BpmnConfConfigJson BuildConfConfig(BpmnConfVo confVo)
    {
        var config = new BpmnConfConfigJson();

        // View page buttons
        config.ViewPageButtons = BuildViewPageButtons(confVo.ViewPageButtons);

        
        config.NoticeTemplateConfig = BuildNoticeTemplateConfig();

       
        config.ConfTemplates = BuildConfTemplates(confVo.TemplateVos, confVo.FormCode);

       
        if (confVo.IsLowCodeFlow != null && confVo.IsLowCodeFlow == 1)
        {
            config.LowCodeFormConfig = BuildLowCodeFormConfig(confVo);
        }

        // Notice channel types
        if (confVo.NoticeChannelTypes != null && confVo.NoticeChannelTypes.Count > 0)
        {
            config.NoticeChannelTypes = confVo.NoticeChannelTypes;
        }

        return config;
    }

    public static List<BpmnConfLfFormdataField> BuildLowCodeFields(
        long confId,
        long formDataId,
        string? lfFormData,
        string? tenantId = null)
    {
        var fields = new List<BpmnConfLfFormdataField>();
        foreach (var field in BuildLowCodeFormFields(lfFormData))
        {
            fields.Add(new BpmnConfLfFormdataField
            {
                BpmnConfId = confId,
                FormDataId = formDataId,
                FieldId = field.FieldId ?? string.Empty,
                FieldName = field.FieldName ?? string.Empty,
                FieldType = field.FieldType,
                IsConditionField = field.IsConditionField,
                TenantId = tenantId ?? string.Empty
            });
        }

        return fields;
    }

    public static Dictionary<string, BpmnConfLfFormdataField> ToFieldMap(
        long confId,
        long formDataId,
        LowCodeFormConfig? lowCodeFormConfig)
    {
        var result = new Dictionary<string, BpmnConfLfFormdataField>();
        if (lowCodeFormConfig?.Fields == null)
        {
            return result;
        }

        foreach (var field in lowCodeFormConfig.Fields.Where(x => !string.IsNullOrWhiteSpace(x.FieldId)))
        {
            result[field.FieldId!] = new BpmnConfLfFormdataField
            {
                BpmnConfId = confId,
                FormDataId = formDataId,
                FieldId = field.FieldId!,
                FieldName = field.FieldName ?? string.Empty,
                FieldType = field.FieldType,
                IsConditionField = field.IsConditionField
            };
        }

        return result;
    }

    private static List<ViewPageButtonItem>? BuildViewPageButtons(BpmnViewPageButtonBaseVo? viewPageButtons)
    {
        if (viewPageButtons == null)
        {
            return null;
        }

        var result = new List<ViewPageButtonItem>();
        AddViewPageButtons(result, viewPageButtons.ViewPageStart, ViewPageTypeEnum.VIEW_PAGE_TYPE_START);
        AddViewPageButtons(result, viewPageButtons.ViewPageOther, ViewPageTypeEnum.VIEW_PAGE_TYPE_OTHER);
        return result.Count == 0 ? null : result;
    }

    private static void AddViewPageButtons(
        List<ViewPageButtonItem> result,
        List<int>? buttons,
        ViewPageTypeEnum viewPageTypeEnum)
    {
        if (buttons == null || buttons.Count == 0)
        {
            return;
        }

        result.AddRange(buttons.Select(buttonType => new ViewPageButtonItem
        {
            ViewType = (int)viewPageTypeEnum,
            ButtonType = buttonType,
            ButtonName = ButtonTypeEnumExtensions.GetDescByCode(buttonType)
        }));
    }

    private static LowCodeFormConfig? BuildLowCodeFormConfig(BpmnConfVo confVo)
    {
        // Java version only stores formdata; fields are built by LFFormDataPreProcessor separately
        if (string.IsNullOrWhiteSpace(confVo.LfFormData))
        {
            return null;
        }

        return new LowCodeFormConfig
        {
            Formdata = confVo.LfFormData
        };
    }

    /// <summary>
    /// Build notice template config from MsgNoticeTypeEnum defaults, matching Java
    /// </summary>
    private static NoticeTemplateConfig BuildNoticeTemplateConfig()
    {
        var details = new List<NoticeTemplateDetailItem>();
        foreach (var msgType in Enum.GetValues<MsgNoticeTypeEnum>())
        {
            details.Add(new NoticeTemplateDetailItem
            {
                NoticeTemplateType = (int)msgType,
                NoticeTemplateDetailContent = MsgNoticeTypeEnumExtensions.GetDefaultValueByCode((int)msgType)
            });
        }
        return new NoticeTemplateConfig
        {
            Details = details
        };
    }

    /// <summary>
    /// </summary>
    public static List<ConfTemplateConf>? BuildConfTemplates(List<BpmnTemplateVo>? templateVos, string? formCode)
    {
        if (templateVos == null || templateVos.Count == 0)
        {
            return null;
        }
        return templateVos.Select(t => new ConfTemplateConf
        {
            Event = t.Event,
            InformIdList = t.InformIdList,
            EmpList = t.EmpList,
            RoleList = t.RoleList,
            FuncList = t.FuncList,
            TemplateId = t.TemplateId > 0 ? t.TemplateId : null,
            MessageSendType = ConvertMessageSendTypeList(t.MessageSendTypeList),
            FormCode = formCode
        }).ToList();
    }

    private static string? ConvertMessageSendTypeList(List<BaseNumIdStruVo>? list)
    {
        if (list == null || list.Count == 0) return null;
        return string.Join(",", list.Select(o => o.Id.ToString()));
    }

    private static List<LowCodeFormField> BuildLowCodeFormFields(string? lfFormData)
    {
        if (string.IsNullOrWhiteSpace(lfFormData))
        {
            return new List<LowCodeFormField>();
        }

        var wrapper = JsonSerializer.Deserialize<FormConfigWrapper>(lfFormData, JsonConfUtil.Options);
        if (wrapper?.WidgetList == null || wrapper.WidgetList.Count == 0)
        {
            throw new AFBizException("Low-code form has no widget");
        }

        var result = new List<LowCodeFormField>();
        ParseWidgetListRecursively(wrapper.WidgetList, result);
        return result;
    }

    private static void ParseWidgetListRecursively(
        IEnumerable<FormConfigWrapper.LFWidget>? widgetList,
        List<LowCodeFormField> result)
    {
        if (widgetList == null)
        {
            return;
        }

        foreach (var lfWidget in widgetList)
        {
            if (!StringConstants.LOWFLOW_FORM_CONTAINER_TYPE.Equals(lfWidget.Category))
            {
                var lfOption = lfWidget.Options;
                if (lfOption == null)
                {
                    continue;
                }

                result.Add(new LowCodeFormField
                {
                    FieldId = lfOption.Name,
                    FieldName = lfOption.Label,
                    FieldType = lfOption.FieldType
                });
                continue;
            }

            var containerTypeEnum = VariantFormContainerTypeEnumExtensions.GetByTypeName(lfWidget.Type);
            if (containerTypeEnum == null)
            {
                throw new AFBizException("Undefined container type!");
            }

            if (containerTypeEnum == VariantFormContainerTypeEnum.CARD)
            {
                ParseWidgetListRecursively(lfWidget.WidgetList, result);
            }
            else if (containerTypeEnum == VariantFormContainerTypeEnum.TAB)
            {
                foreach (var tab in lfWidget.Tabs ?? new List<FormConfigWrapper.LFWidget>())
                {
                    ParseWidgetListRecursively(tab.WidgetList, result);
                }
            }
            else
            {
                var rows = lfWidget.Rows ?? new List<FormConfigWrapper.TableRow>();
                if (rows.Count > 0)
                {
                    foreach (var row in rows)
                    {
                        foreach (var col in row.Cols ?? new List<FormConfigWrapper.LFWidget>())
                        {
                            ParseWidgetListRecursively(col.WidgetList, result);
                        }
                    }
                }
                else
                {
                    foreach (var col in lfWidget.Cols ?? new List<FormConfigWrapper.LFWidget>())
                    {
                        ParseWidgetListRecursively(col.WidgetList, result);
                    }
                }
            }
        }
    }
}
