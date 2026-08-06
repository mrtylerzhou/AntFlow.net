using System.Text.Json;
using AntFlowCore.Base.constant;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Engine.service.processor.lowcodeflow;

/// <summary>
/// 解析低代码表单 widgetList，提取字段元数据。
/// 供流程内联表单保存(LFFormDataPreProcessor)和独立表单管理模块共用。
/// </summary>
public static class LfFormWidgetParser
{
    /// <summary>
    /// 解析 formdata JSON，返回字段元数据列表。
    /// </summary>
    /// <param name="formdataJson">表单 JSON</param>
    /// <param name="confId">所属流程配置ID（独立表单可传 null）</param>
    /// <param name="formDataId">表单版本ID(t_bpmn_conf_lf_formdata.id)</param>
    public static List<BpmnConfLfFormdataField> ParseFields(string formdataJson, long? confId, long formDataId)
    {
        if (string.IsNullOrEmpty(formdataJson))
        {
            throw new AFBizException("lowcode formdata is empty");
        }

        FormConfigWrapper? formConfigWrapper = JsonSerializer.Deserialize<FormConfigWrapper>(formdataJson);
        var lfWidgetList = formConfigWrapper?.WidgetList;
        if (lfWidgetList == null || !lfWidgetList.Any())
        {
            throw new AFBizException($"lowcode form has no widget,confId:{confId},formDataId:{formDataId}");
        }

        var formdataFields = new List<BpmnConfLfFormdataField>();
        ParseWidgetListRecursively(lfWidgetList, confId, formDataId, formdataFields);
        if (!formdataFields.Any())
        {
            throw new AFBizException($"lowcode form fields can not be empty,confId:{confId},formDataId:{formDataId}");
        }

        return formdataFields;
    }

    private static void ParseWidgetListRecursively(
        IEnumerable<FormConfigWrapper.LFWidget> widgetList,
        long? confId,
        long formDataId,
        List<BpmnConfLfFormdataField> result)
    {
        foreach (var lfWidget in widgetList)
        {
            if (!StringConstants.LOWFLOW_FORM_CONTAINER_TYPE.Equals(lfWidget.Category))
            {
                var lfOption = lfWidget.Options;
                var formdataField = new BpmnConfLfFormdataField
                {
                    BpmnConfId = confId ?? 0,
                    FormDataId = formDataId,
                    FieldType = GetFieldTypeByTypeString(lfWidget.Type),
                    FieldId = lfOption?.Name,
                    FieldName = lfOption?.Label,
                };
                result.Add(formdataField);
            }
            else
            {
                var containerTypeEnum = VariantFormContainerTypeEnumExtensions.GetByTypeName(lfWidget.Type);
                if (containerTypeEnum == null)
                {
                    continue;
                }

                if (containerTypeEnum == VariantFormContainerTypeEnum.CARD)
                {
                    ParseWidgetListRecursively(lfWidget.WidgetList ?? new List<FormConfigWrapper.LFWidget>(), confId, formDataId, result);
                }
                else if (containerTypeEnum == VariantFormContainerTypeEnum.TAB)
                {
                    var tabs = lfWidget.Tabs ?? new List<FormConfigWrapper.LFWidget>();
                    foreach (var tab in tabs)
                    {
                        ParseWidgetListRecursively(tab.WidgetList ?? new List<FormConfigWrapper.LFWidget>(), confId, formDataId, result);
                    }
                }
                else
                {
                    var rows = lfWidget.Rows ?? new List<FormConfigWrapper.TableRow>();
                    if (rows.Count > 0)
                    {
                        foreach (var row in rows)
                        {
                            var cols = row.Cols ?? new List<FormConfigWrapper.LFWidget>();
                            foreach (var col in cols)
                            {
                                var subWidgetList = col.WidgetList;
                                if (subWidgetList == null || !subWidgetList.Any())
                                {
                                    continue;
                                }
                                ParseWidgetListRecursively(subWidgetList, confId, formDataId, result);
                            }
                        }
                    }
                    else
                    {
                        var cols = lfWidget.Cols ?? new List<FormConfigWrapper.LFWidget>();
                        foreach (var col in cols)
                        {
                            var subWidgetList = col.WidgetList;
                            if (subWidgetList == null || !subWidgetList.Any())
                            {
                                continue;
                            }
                            ParseWidgetListRecursively(subWidgetList, confId, formDataId, result);
                        }
                    }
                }
            }
        }
    }

    private static int GetFieldTypeByTypeString(string typeString)
    {
        switch (typeString)
        {
            // NUMBER
            case "number":
            case "slider":
                return LFFieldTypeEnum.NUMBER.Type;
            // DATE
            case "date":
                return LFFieldTypeEnum.DATE.Type;
            // DATE_TIME
            case "date-range":
            case "time":
            case "time-range":
                return LFFieldTypeEnum.DATE_TIME.Type;
            // BOOLEAN
            case "switch":
                return LFFieldTypeEnum.BOOLEAN.Type;
            // TEXT (long text)
            case "textarea":
            case "richtext-editor":
                return LFFieldTypeEnum.TEXT.Type;
            // STRING (short text) - default for most form fields
            default:
                return LFFieldTypeEnum.STRING.Type;
        }
    }
}
