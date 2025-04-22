using antflowcore.adaptor.formoperation;
using antflowcore.constant.enums;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Entity;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace antflowcore.service.processor.lowcodeflow;

[AfFormServiceAnno(SvcName = "LF", Desc = "低(无)代码流程")]
public class LowFlowApprovalService : IFormOperationAdaptor<UDLFApplyVo>
{
    private readonly ILogger<LowFlowApprovalService> _logger;
    private readonly LFMainService _mainService;
    private readonly LFMainFieldService _lfMainFieldService;
    private readonly BpmnConfLfFormdataService _lfformdataService;
    private readonly BpmnConfLfFormdataFieldService _lfformdataFieldService;

    private static Dictionary<long, List<String>> conditionFieldNameMap = new Dictionary<long, List<string>>();

    // key is confid,value is a map of field's name and its self
    private static Dictionary<long, Dictionary<String, BpmnConfLfFormdataField>> allFieldConfMap =
        new Dictionary<long, Dictionary<string, BpmnConfLfFormdataField>>();

    public LowFlowApprovalService(ILogger<LowFlowApprovalService> logger, LFMainService mainService,
        LFMainFieldService lfMainFieldService,
        BpmnConfLfFormdataService lfformdataService,
        BpmnConfLfFormdataFieldService lfformdataFieldService)
    {
        _logger = logger;
        _mainService = mainService;
        _lfMainFieldService = lfMainFieldService;
        _lfformdataService = lfformdataService;
        _lfformdataFieldService = lfformdataFieldService;
    }

    public BpmnStartConditionsVo PreviewSetCondition(UDLFApplyVo vo)
    {
        String userId = vo.StartUserId;

        BpmnStartConditionsVo startConditionsVo = new BpmnStartConditionsVo
        {
            IsLowCodeFlow = true,
            StartUserId = userId,
        };
        if (vo.LfConditions != null && vo.LfConditions.Any())
        {
            startConditionsVo.LfConditions = vo.LfConditions;
        }
        else
        {
            startConditionsVo.LfConditions = vo.lfFields;
        }

        return startConditionsVo;
    }

    public BpmnStartConditionsVo LaunchParameters(UDLFApplyVo vo)
    {
        String userId = vo.StartUserId;

        BpmnStartConditionsVo startConditionsVo = new BpmnStartConditionsVo
        {
            IsLowCodeFlow = true,
            StartUserId = userId,
        };
        if (vo.LfConditions != null && vo.LfConditions.Any())
        {
            startConditionsVo.LfConditions = vo.LfConditions;
        }
        else
        {
            startConditionsVo.LfConditions = vo.lfFields;
        }

        return startConditionsVo;
    }

    public void OnInitData(UDLFApplyVo vo)
    {
        return;
    }

    public void OnQueryData(UDLFApplyVo vo)
    {
        LFMain lfMain = _mainService.baseRepo.Where(a => a.Id == long.Parse(vo.BusinessId)).First();
        if (lfMain == null)
        {
            _logger.LogError("can not get lowcode from data by specified Id:{0}", vo.BusinessId);
            throw new AFBizException("can not get lowcode form data by specified id");
        }

        long mainId = lfMain.Id;
        long confId = lfMain.ConfId.Value;
        string formCode = lfMain.FormCode;

        if (!allFieldConfMap.TryGetValue(confId, out var lfFormdataFieldMap) || lfFormdataFieldMap == null)
        {
            lfFormdataFieldMap = _lfformdataFieldService.QryFormDataFieldMap(confId);
            allFieldConfMap[confId] = lfFormdataFieldMap;
        }

        List<LFMainField> lfMainFields = _lfMainFieldService.baseRepo.Where(x => x.MainId == mainId).ToList();
        if (lfMainFields == null || !lfMainFields.Any())
        {
            throw new AFBizException($"lowcode form with formcode:{formCode}, confid:{confId} has no formdata");
        }

        // 返回到页面用于显示
        Dictionary<string, object> fieldVoMap = new Dictionary<string, object>();
        Dictionary<String, List<LFMainField>> fieldName2SelfMap =
            lfMainFields.GroupBy(x => x.FieldId).ToDictionary(g => g.Key, g => g.ToList());

        foreach (var id2SelfEntry in fieldName2SelfMap)
        {
            string fieldName = id2SelfEntry.Key;
            if (!lfFormdataFieldMap.TryGetValue(fieldName, out var currentFieldProp))
            {
                throw new AFBizException($"field with name:{fieldName} has no property");
            }

            var fields = id2SelfEntry.Value;
            int valueLen = fields.Count;
            List<object> actualMultiValue = valueLen == 1 ? null : new List<object>(valueLen);

            foreach (var field in fields)
            {
                int fieldType = currentFieldProp.FieldType.Value;
                var fieldTypeEnum = LFFieldTypeEnum.GetByType(fieldType);
                if (fieldTypeEnum == null)
                {
                    throw new AFBizException(
                        $"unrecognized field type, name:{fieldName}, formcode:{formCode}, confId:{confId}");
                }

                object actualValue = null;
                switch (fieldTypeEnum)
                {
                    case var ftype when ftype == LFFieldTypeEnum.STRING:
                        actualValue = field.FieldValue;
                        if (actualValue != null)
                        {
                            string actualValueString = actualValue.ToString();
                            if (actualValueString.StartsWith("{"))
                            {
                                actualValue = JsonSerializer.Deserialize<Dictionary<string, object>>(actualValueString);
                            }
                            else if (actualValueString.StartsWith("["))
                            {
                                actualValue = JsonSerializer.Deserialize<List<object>>(actualValueString);
                            }
                        }

                        break;

                    case var ftype when ftype == LFFieldTypeEnum.NUMBER:
                        actualValue = int.Parse(field.FieldValue);
                        break;

                    case var ftype when ftype == LFFieldTypeEnum.DATE_TIME:
                        actualValue = field.FieldValueDt?.ToString("yyyy-MM-dd HH:mm:ss");
                        break;

                    case var ftype when ftype == LFFieldTypeEnum.DATE:
                        actualValue = field.FieldValueDt?.ToString("yyyy-MM-dd");
                        break;

                    case var ftype when ftype == LFFieldTypeEnum.TEXT:
                        actualValue = field.FieldValueText;
                        break;

                    case var ftype when ftype == LFFieldTypeEnum.BOOLEAN:
                        actualValue = bool.Parse(field.FieldValue);
                        break;
                }

                if (valueLen == 1)
                {
                    fieldVoMap[fieldName] = actualValue;
                    break;
                }

                actualMultiValue.Add(actualValue);
            }

            if (actualMultiValue != null && actualMultiValue.Any())
            {
                fieldVoMap[fieldName] = actualMultiValue;
            }
        }

        vo.lfFields = fieldVoMap;

        List<BpmnConfLfFormdata> bpmnConfLfFormdataList =
            _lfformdataService.baseRepo.Where(x => x.BpmnConfId == confId).ToList();
        if (bpmnConfLfFormdataList == null || !bpmnConfLfFormdataList.Any())
        {
            throw new AFBizException($"can not get lowcode flow formdata by confId:{confId}");
        }

        var lfFormdata = bpmnConfLfFormdataList.First();
        vo.LfFormData = lfFormdata.Formdata;
    }

    public void OnSubmitData(UDLFApplyVo vo)
    {
        var lfFields = vo.lfFields;
        if (lfFields == null || lfFields.Count == 0)
        {
            throw new AFBizException("form data does not contain any field");
        }

        long confId = vo.BpmnConfVo.Id;
        string formCode = vo.FormCode;

        var main = new LFMain
        {
            Id = SnowFlake.NextId(),
            ConfId = confId,
            FormCode = formCode,
            CreateUser = SecurityUtils.GetLogInEmpName()
        };
        _mainService.baseRepo.Insert(main);
        long mainId = main.Id;

        if (!allFieldConfMap.TryGetValue(confId, out var lfFormdataFieldMap) || lfFormdataFieldMap == null || lfFormdataFieldMap.Count == 0)
        {
            Dictionary<string, BpmnConfLfFormdataField> name2SelfMap = _lfformdataFieldService.QryFormDataFieldMap(confId);
            allFieldConfMap[confId] = name2SelfMap;
        }

        if (!allFieldConfMap.TryGetValue(confId, out var fieldConfMap) || fieldConfMap == null || fieldConfMap.Count == 0)
        {
            throw new AFBizException($"confId {confId}, formCode:{vo.FormCode} does not have a field config");
        }

        var mainFields = LFMainField.ParseFromMap(lfFields, fieldConfMap, mainId);
        _lfMainFieldService.baseRepo.Insert(mainFields);

        vo.BusinessId = mainId.ToString();
        vo.ProcessDigest = vo.Remark;
        vo.EntityName = nameof(LowFlowApprovalService);
    }

    public void OnConsentData(UDLFApplyVo vo)
    {
        if (vo.OperationType != (int)ButtonTypeEnum.BUTTON_TYPE_RESUBMIT)
        {
            return;
        }

        var lfFields = vo.lfFields;
        if (lfFields == null || lfFields.Count == 0)
        {
            throw new AFBizException("form data does not contain any field");
        }

        var lfMain = _mainService.baseRepo.Where(a => a.Id == long.Parse(vo.BusinessId)).First();
        if (lfMain == null)
        {
            _logger.LogError($"can not get lowcode from data by specified Id:{vo.BusinessId}");
            throw new AFBizException("can not get lowcode form data by specified id");
        }

        long mainId = lfMain.Id;
        string formCode = vo.FormCode;
        long confId = vo.BpmnConfVo.Id;

        List<LFMainField> lfMainFields = _lfMainFieldService.baseRepo.Where(a => a.MainId == mainId).ToList();
        if (lfMainFields == null || lfMainFields.Count == 0)
        {
            throw new AFBizException($"lowcode form with formcode:{formCode}, confId:{confId} has no formdata");
        }

        foreach (var field in lfMainFields)
        {
            string fValue = lfFields[field.FieldId].ToString();
            field.FieldValue = fValue;
        }
        _lfMainFieldService.baseRepo.Update(lfMainFields);
    }

    public void OnBackToModifyData(UDLFApplyVo vo)
    {
        throw new NotImplementedException();
    }

    public void OnCancellationData(UDLFApplyVo vo)
    {
        throw new NotImplementedException();
    }

    public void OnFinishData(BusinessDataVo vo)
    {
    }
}