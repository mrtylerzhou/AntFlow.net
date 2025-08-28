using AntFlow.Core.Constant;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Text.Json;

namespace AntFlow.Core.Service.Processor.LowCodeFlow;

public class LFFormDataPreProcessor : IAntFlowOrderPreProcessor<BpmnConfVo>
{
    private readonly BpmnConfLfFormdataFieldService _lfFormdataFieldService;
    private readonly BpmnConfLfFormdataService _lfFormdataService;

    public LFFormDataPreProcessor(BpmnConfLfFormdataService lfFormdataService,
        BpmnConfLfFormdataFieldService lfFormdataFieldService)
    {
        _lfFormdataService = lfFormdataService;
        _lfFormdataFieldService = lfFormdataFieldService;
    }

    public void PreWriteProcess(BpmnConfVo confVo)
    {
        if (confVo == null)
        {
            return;
        }

        bool isLowCodeFlow = confVo.IsLowCodeFlow == 1;
        if (!isLowCodeFlow)
        {
            return;
        }

        long confId = confVo.Id;
        string? lfForm = confVo.LfFormData;

        BpmnConfLfFormdata? lfFormdata = new()
        {
            BpmnConfId = confId, Formdata = lfForm, CreateUser = SecurityUtils.GetLogInEmpName()
        };
        _lfFormdataService.baseRepo.Insert(lfFormdata);
        confVo.LfFormDataId = lfFormdata.Id;

        FormConfigWrapper formConfigWrapper = JsonSerializer.Deserialize<FormConfigWrapper>(lfForm);
        List<FormConfigWrapper.LFWidget>? lfWidgetList = formConfigWrapper.WidgetList;

        if (lfWidgetList == null || !lfWidgetList.Any())
        {
            throw new AFBizException($"Low-code form has no widget, confId: {confId}, formCode: {confVo.FormCode}");
        }

        List<BpmnConfLfFormdataField>? formdataFields = new();
        ParseWidgetListRecursively(lfWidgetList, confId, lfFormdata.Id, formdataFields);

        if (!formdataFields.Any())
        {
            throw new AFBizException(
                $"Low-code form fields cannot be empty, confId: {confId}, formCode: {confVo.FormCode}");
        }

        int affrows = _lfFormdataFieldService.Frsql.Insert(formdataFields).ExecuteAffrows();
        if (affrows <= 0)
        {
            throw new AFBizException("插入表单字段失败!");
        }
    }

    public void PreReadProcess(BpmnConfVo confVo)
    {
        if (confVo == null)
        {
            return;
        }

        bool isLowCodeFlow = confVo.IsLowCodeFlow == 1;
        if (!isLowCodeFlow)
        {
            return;
        }

        long confId = confVo.Id;

        List<BpmnConfLfFormdata>? bpmnConfLfFormdataList = _lfFormdataService.ListByConfId(confId);

        if (bpmnConfLfFormdataList == null || !bpmnConfLfFormdataList.Any())
        {
            throw new AFBizException($"Cannot get low-code flow formdata by confId: {confId}");
        }

        BpmnConfLfFormdata? lfFormdata = bpmnConfLfFormdataList.First();
        confVo.LfFormData = lfFormdata.Formdata;
        confVo.LfFormDataId = lfFormdata.Id;
    }

    public int Order()
    {
        return 0;
    }

    private void ParseWidgetListRecursively(
        IEnumerable<FormConfigWrapper.LFWidget> widgetList,
        long confId,
        long formDataId,
        List<BpmnConfLfFormdataField> result)
    {
        foreach (FormConfigWrapper.LFWidget? lfWidget in widgetList)
        {
            if (!StringConstants.LOWFLOW_FORM_CONTAINER_TYPE.Equals(lfWidget.Category))
            {
                FormConfigWrapper.LFWidget.LFOption? lfOption = lfWidget.Options;
                BpmnConfLfFormdataField formdataField = new()
                {
                    BpmnConfId = confId,
                    FormDataId = formDataId,
                    FieldType = lfOption.FieldType,
                    FieldId = lfOption.Name,
                    FieldName = lfOption.Label,
                    TenantId = MultiTenantUtil.GetCurrentTenantId()
                };
                result.Add(formdataField);
            }
            else
            {
                VariantFormContainerTypeEnum? containerTypeEnum =
                    VariantFormContainerTypeEnumExtensions.GetByTypeName(lfWidget.Type);
                if (containerTypeEnum == null)
                {
                    throw new AFBizException("Undefined container type!");
                }

                if (containerTypeEnum == VariantFormContainerTypeEnum.CARD)
                {
                    ParseWidgetListRecursively(lfWidget.WidgetList, confId, formDataId, result);
                }
                else if (containerTypeEnum == VariantFormContainerTypeEnum.TAB)
                {
                    foreach (FormConfigWrapper.LFWidget? tab in lfWidget.Tabs)
                    {
                        ParseWidgetListRecursively(tab.WidgetList, confId, formDataId, result);
                    }
                }
                else
                {
                    List<FormConfigWrapper.TableRow>? rows = lfWidget.Rows ?? new List<FormConfigWrapper.TableRow>();
                    if (rows.Count > 0)
                    {
                        foreach (FormConfigWrapper.TableRow? row in rows)
                        {
                            foreach (FormConfigWrapper.LFWidget? col in row.Cols)
                            {
                                ParseWidgetListRecursively(col.WidgetList, confId, formDataId, result);
                            }
                        }
                    }
                    else
                    {
                        List<FormConfigWrapper.LFWidget>?
                            cols = lfWidget.Cols ?? new List<FormConfigWrapper.LFWidget>();
                        foreach (FormConfigWrapper.LFWidget? col in cols)
                        {
                            ParseWidgetListRecursively(col.WidgetList, confId, formDataId, result);
                        }
                    }
                }
            }
        }
    }
}