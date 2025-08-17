using System.Text.Json;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor.lowcodeflow;

using System.Collections.Generic;
using System.Linq;

public class LFFormDataPreProcessor : IAntFlowOrderPreProcessor<BpmnConfVo>
    {
        private readonly BpmnConfLfFormdataService _lfFormdataService;
        private readonly BpmnConfLfFormdataFieldService _lfFormdataFieldService;

        public LFFormDataPreProcessor(BpmnConfLfFormdataService lfFormdataService, BpmnConfLfFormdataFieldService lfFormdataFieldService)
        {
            _lfFormdataService = lfFormdataService;
            _lfFormdataFieldService = lfFormdataFieldService;
        }

        public void PreWriteProcess(BpmnConfVo confVo)
        {
            if (confVo == null) return;

            var isLowCodeFlow = confVo.IsLowCodeFlow == 1;
            if (!isLowCodeFlow) return;

            var confId = confVo.Id;
            var lfForm = confVo.LfFormData;

            var lfFormdata = new BpmnConfLfFormdata
            {
                BpmnConfId = confId,
                Formdata = lfForm,
                CreateUser = SecurityUtils.GetLogInEmpName()
            };
            _lfFormdataService.baseRepo.Insert(lfFormdata);
            confVo.LfFormDataId = lfFormdata.Id;

            FormConfigWrapper formConfigWrapper = JsonSerializer.Deserialize<FormConfigWrapper>(lfForm);
            var lfWidgetList = formConfigWrapper.WidgetList;

            if (lfWidgetList == null || !lfWidgetList.Any())
            {
                throw new AFBizException($"Low-code form has no widget, confId: {confId}, formCode: {confVo.FormCode}");
            }

            var formdataFields = new List<BpmnConfLfFormdataField>();
            ParseWidgetListRecursively(lfWidgetList, confId, lfFormdata.Id, formdataFields);

            if (!formdataFields.Any())
            {
                throw new AFBizException($"Low-code form fields cannot be empty, confId: {confId}, formCode: {confVo.FormCode}");
            }

            int affrows = _lfFormdataFieldService.Frsql.Insert(formdataFields).ExecuteAffrows();
            if (affrows <= 0)
            {
                throw new AFBizException("数据插入失败!");
            }
           
        }

        public void PreReadProcess(BpmnConfVo confVo)
        {
            if (confVo == null) return;

            var isLowCodeFlow = confVo.IsLowCodeFlow == 1;
            if (!isLowCodeFlow) return;

            var confId = confVo.Id;

            var bpmnConfLfFormdataList = _lfFormdataService.ListByConfId(confId);

            if (bpmnConfLfFormdataList == null || !bpmnConfLfFormdataList.Any())
            {
                throw new AFBizException($"Cannot get low-code flow formdata by confId: {confId}");
            }

            var lfFormdata = bpmnConfLfFormdataList.First();
            confVo.LfFormData = lfFormdata.Formdata;
            confVo.LfFormDataId = lfFormdata.Id;
        }

        private void ParseWidgetListRecursively(
            IEnumerable<FormConfigWrapper.LFWidget> widgetList,
            long confId,
            long formDataId,
            List<BpmnConfLfFormdataField> result)
        {
            foreach (var lfWidget in widgetList)
            {
                if (!StringConstants.LOWFLOW_FORM_CONTAINER_TYPE.Equals(lfWidget.Category))
                {
                    var lfOption = lfWidget.Options;
                    BpmnConfLfFormdataField formdataField = new BpmnConfLfFormdataField
                    {
                        BpmnConfId = confId,
                        FormDataId = formDataId,
                        FieldType = lfOption.FieldType,
                        FieldId = lfOption.Name,
                        FieldName = lfOption.Label,
                        TenantId = MultiTenantUtil.GetCurrentTenantId(),
                    };
                    result.Add(formdataField);
                }
                else
                {
                    var containerTypeEnum = VariantFormContainerTypeEnumExtensions.GetByTypeName(lfWidget.Type);
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
                        foreach (var tab in lfWidget.Tabs)
                        {
                            ParseWidgetListRecursively(tab.WidgetList, confId, formDataId, result);
                        }
                    }
                    else
                    {
                        var rows = lfWidget.Rows ?? new List<FormConfigWrapper.TableRow>();
                        if (rows.Count > 0)
                        {
                            foreach (var row in rows)
                            {
                                foreach (var col in row.Cols)
                                {
                                    ParseWidgetListRecursively(col.WidgetList, confId, formDataId, result);
                                }
                            }
                        }
                        else
                        {
                           var cols = lfWidget.Cols??new List<FormConfigWrapper.LFWidget>();
                            foreach (var col in cols)
                            {
                                ParseWidgetListRecursively(col.WidgetList,confId,formDataId,result);
                            }
                        }
                    }
                }
            }
        }

        public int Order()
        {
            return 0;
        }
    }


