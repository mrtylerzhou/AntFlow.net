using AntFlow.Core.Adaptor;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Factory;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Text.Json;
using System.Text.Json.Nodes;
using AntFlowException = AntFlow.Core.Exception;

namespace AntFlow.Core.Service.Processor.LowCodeFlow;

/**
 * 低代码(表单)流程服务
 */
[AfFormServiceAnno(SvcName = "LF", Desc = "")]
public class LowFlowApprovalService : IFormOperationAdaptor<UDLFApplyVo>
{
    private static Dictionary<long, List<string>> conditionFieldNameMap = new();

    // key is confid,value is a map of field's name and its self
    private static readonly Dictionary<long, Dictionary<string, BpmnConfLfFormdataField>> allFieldConfMap = new();

    private readonly BpmnConfLfFormdataFieldService _lfformdataFieldService;
    private readonly BpmnConfLfFormdataService _lfformdataService;
    private readonly LFMainFieldService _lfMainFieldService;
    private readonly ILogger<LowFlowApprovalService> _logger;
    private readonly LFMainService _mainService;

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
        string userId = vo.StartUserId;

        BpmnStartConditionsVo startConditionsVo = new() { IsLowCodeFlow = true, StartUserId = userId };
        if (vo.LfConditions != null && vo.LfConditions.Any())
        {
            startConditionsVo.LfConditions = vo.LfConditions;
        }
        else
        {
            startConditionsVo.LfConditions = vo.LfFields;
        }

        return startConditionsVo;
    }

    public BpmnStartConditionsVo LaunchParameters(UDLFApplyVo vo)
    {
        string userId = vo.StartUserId;

        BpmnStartConditionsVo startConditionsVo = new() { IsLowCodeFlow = true, StartUserId = userId };
        if (vo.LfConditions != null && vo.LfConditions.Any())
        {
            startConditionsVo.LfConditions = vo.LfConditions;
        }
        else
        {
            startConditionsVo.LfConditions = vo.LfFields;
        }

        return startConditionsVo;
    }

    public void OnInitData(UDLFApplyVo vo)
    {
        IEnumerable<ILFFormOperationAdaptor> lfFormOperationAdaptors =
            ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
        foreach (ILFFormOperationAdaptor? o in lfFormOperationAdaptors)
        {
            o.OnInitData(vo);
        }
    }

    public void OnQueryData(UDLFApplyVo vo)
    {
        LFMain lfMain = _mainService.baseRepo.Where(a => a.Id == long.Parse(vo.BusinessId)).First();
        if (lfMain == null)
        {
            _logger.LogError("can not get lowcode from data by specified Id:{0}", vo.BusinessId);
            throw new AntFlowException.AFBizException("can not get lowcode form data by specified id");
        }

        long mainId = lfMain.Id;
        long confId = lfMain.ConfId.Value;
        string formCode = lfMain.FormCode;

        if (!allFieldConfMap.TryGetValue(confId, out Dictionary<string, BpmnConfLfFormdataField>? lfFormdataFieldMap) ||
            lfFormdataFieldMap == null)
        {
            lfFormdataFieldMap = _lfformdataFieldService.QryFormDataFieldMap(confId);
            allFieldConfMap[confId] = lfFormdataFieldMap;
        }

        List<LFMainField> lfMainFields = _lfMainFieldService.baseRepo.Where(x => x.MainId == mainId).ToList();
        if (lfMainFields == null || !lfMainFields.Any())
        {
            throw new AntFlowException.AFBizException(
                $"lowcode form with formcode:{formCode}, confid:{confId} has no formdata");
        }

        // 构建字段值映射
        Dictionary<string, object> fieldVoMap = new();
        Dictionary<string, List<LFMainField>> fieldName2SelfMap =
            lfMainFields.GroupBy(x => x.FieldId).ToDictionary(g => g.Key, g => g.ToList());

        foreach (KeyValuePair<string, List<LFMainField>> id2SelfEntry in fieldName2SelfMap)
        {
            string fieldName = id2SelfEntry.Key;
            if (!lfFormdataFieldMap.TryGetValue(fieldName, out BpmnConfLfFormdataField? currentFieldProp))
            {
                throw new AntFlowException.AFBizException($"field with name:{fieldName} has no property");
            }

            List<LFMainField>? fields = id2SelfEntry.Value;
            int valueLen = fields.Count;
            List<object> actualMultiValue = valueLen == 1 ? null : new List<object>(valueLen);

            foreach (LFMainField? field in fields)
            {
                int fieldType = currentFieldProp.FieldType.Value;
                LFFieldTypeEnum? fieldTypeEnum = LFFieldTypeEnum.GetByType(fieldType);
                if (fieldTypeEnum == null)
                {
                    throw new AntFlowException.AFBizException(
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
                        if (LFControlTypeEnum.SELECT.Name == currentFieldProp.FieldName)
                        {
                            try
                            {
                                JsonNode? jsonNode = JsonNode.Parse(field.FieldValue);
                                if (jsonNode == null)
                                {
                                    actualValue = ""; //select控件为空值
                                }
                                else if (jsonNode is JsonArray jsonArray)
                                {
                                    actualValue = jsonArray.ToString();
                                }
                            }
                            catch (System.Exception e)
                            {
                                //字段值无法解析为数字，记录警告信息，使用原始值
                                _logger.LogWarning(
                                    $"field value can not be parsed to number,fieldName:{fieldName},formCode:{formCode},confId:{confId}",
                                    e);
                                actualValue = field.FieldValue;
                            }
                        }
                        else
                        {
                            //非select控件的数字类型,直接取数字select,字段值
                            actualValue = field.FieldValueNumber;
                        }

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

        vo.LfFields = fieldVoMap;

        List<BpmnConfLfFormdata> bpmnConfLfFormdataList =
            _lfformdataService.baseRepo.Where(x => x.BpmnConfId == confId).ToList();
        if (bpmnConfLfFormdataList == null || !bpmnConfLfFormdataList.Any())
        {
            throw new AntFlowException.AFBizException($"can not get lowcode flow formdata by confId:{confId}");
        }

        BpmnConfLfFormdata? lfFormdata = bpmnConfLfFormdataList.First();
        vo.LfFormData = lfFormdata.Formdata;
        IEnumerable<ILFFormOperationAdaptor> lfFormOperationAdaptors =
            ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
        foreach (ILFFormOperationAdaptor? o in lfFormOperationAdaptors)
        {
            o.OnQueryData(vo);
        }
    }

    public void OnSubmitData(UDLFApplyVo vo)
    {
        Dictionary<string, object>? lfFields = vo.LfFields;
        if (lfFields == null || lfFields.Count == 0)
        {
            throw new AntFlowException.AFBizException("form data does not contain any field");
        }

        long confId = vo.BpmnConfVo.Id;
        string formCode = vo.FormCode;

        LFMain? main = new()
        {
            Id = SnowFlake.NextId(),
            ConfId = confId,
            FormCode = formCode,
            CreateUser = SecurityUtils.GetLogInEmpName(),
            TenantId = MultiTenantUtil.GetCurrentTenantId()
        };
        _mainService.baseRepo.Insert(main);
        long mainId = main.Id;

        if (!allFieldConfMap.TryGetValue(confId, out Dictionary<string, BpmnConfLfFormdataField>? lfFormdataFieldMap) ||
            lfFormdataFieldMap == null || lfFormdataFieldMap.Count == 0)
        {
            Dictionary<string, BpmnConfLfFormdataField> name2SelfMap =
                _lfformdataFieldService.QryFormDataFieldMap(confId);
            allFieldConfMap[confId] = name2SelfMap;
        }

        if (!allFieldConfMap.TryGetValue(confId, out Dictionary<string, BpmnConfLfFormdataField>? fieldConfMap) ||
            fieldConfMap == null || fieldConfMap.Count == 0)
        {
            throw new AntFlowException.AFBizException(
                $"confId {confId}, formCode:{vo.FormCode} does not have a field config");
        }

        List<LFMainField>? mainFields = LFMainField.ParseFromMap(lfFields, fieldConfMap, mainId, formCode);
        _lfMainFieldService.baseRepo.Insert(mainFields);

        vo.BusinessId = mainId.ToString();
        vo.ProcessDigest = vo.Remark;
        vo.EntityName = nameof(LowFlowApprovalService);
        IEnumerable<ILFFormOperationAdaptor> lfFormOperationAdaptors =
            ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
        foreach (ILFFormOperationAdaptor? o in lfFormOperationAdaptors)
        {
            o.OnSubmitData(vo);
        }
    }

    public void OnConsentData(UDLFApplyVo vo)
    {
        if (vo.OperationType != (int)ButtonTypeEnum.BUTTON_TYPE_RESUBMIT &&
            vo.OperationType != (int)ButtonTypeEnum.BUTTON_TYPE_AGREE)
        {
            return;
        }

        Dictionary<string, object>? lfFields = vo.LfFields;
        if (lfFields == null || lfFields.Count == 0)
        {
            throw new AntFlowException.AFBizException("form data does not contain any field");
        }

        LFMain? lfMain = _mainService.baseRepo.Where(a => a.Id == long.Parse(vo.BusinessId)).First();
        if (lfMain == null)
        {
            _logger.LogError($"can not get lowcode from data by specified Id:{vo.BusinessId}");
            throw new AntFlowException.AFBizException("can not get lowcode form data by specified id");
        }

        long mainId = lfMain.Id;
        string formCode = vo.FormCode;
        long confId = vo.BpmnConfVo.Id;

        List<LFMainField> lfMainFields = _lfMainFieldService.baseRepo.Where(a => a.MainId == mainId).ToList();
        if (lfMainFields == null || lfMainFields.Count == 0)
        {
            throw new AntFlowException.AFBizException(
                $"lowcode form with formcode:{formCode}, confId:{confId} has no formdata");
        }

        Dictionary<string, object> submitLfFields = vo.LfFields;
        if (submitLfFields != null && submitLfFields.Any())
        {
            if (!allFieldConfMap.TryGetValue(confId,
                    out Dictionary<string, BpmnConfLfFormdataField>? lfFormdataFieldMap))
            {
                if (lfFormdataFieldMap == null || lfFormdataFieldMap.Count == 0)
                {
                    Dictionary<string, BpmnConfLfFormdataField> name2SelfMap =
                        _lfformdataFieldService.QryFormDataFieldMap(confId);
                    allFieldConfMap.Add(confId, name2SelfMap);
                }
            }

            if (allFieldConfMap.TryGetValue(confId, out Dictionary<string, BpmnConfLfFormdataField>? fieldConfMap))
            {
                List<LFMainField> mainFields =
                    LFMainField.ParseFromMap(submitLfFields, fieldConfMap, mainId, vo.FormCode);
                if (mainFields != null && mainFields.Count > 0)
                {
                    // 根据fieldId去重，避免重复插入lfMainFields
                    mainFields.RemoveAll(mainField => lfMainFields.Any(a => a.FieldId == mainField.FieldId));
                    if (mainFields.Any())
                    {
                        _lfMainFieldService.baseRepo.Insert(mainFields);
                    }
                }
            }
            else
            {
                throw new AntFlowException.AFBizException(
                    $"confId {confId}, formCode:{vo.FormCode} does not have a field config");
            }
        }

        foreach (LFMainField? field in lfMainFields)
        {
            string fValue = lfFields[field.FieldId]?.ToString() ?? null;
            field.FieldValue = fValue;
        }

        _lfMainFieldService.baseRepo.Update(lfMainFields);
        IEnumerable<ILFFormOperationAdaptor> lfFormOperationAdaptors =
            ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
        foreach (ILFFormOperationAdaptor? o in lfFormOperationAdaptors)
        {
            o.OnConsentData(vo);
        }
    }

    public void OnBackToModifyData(UDLFApplyVo vo)
    {
    }

    public void OnCancellationData(UDLFApplyVo vo)
    {
        IEnumerable<ILFFormOperationAdaptor> lfFormOperationAdaptors =
            ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
        foreach (ILFFormOperationAdaptor? o in lfFormOperationAdaptors)
        {
            o.OnCancellationData(vo);
        }
    }

    public void OnFinishData(BusinessDataVo vo)
    {
        IEnumerable<ILFFormOperationAdaptor> lfFormOperationAdaptors =
            ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
        foreach (ILFFormOperationAdaptor? o in lfFormOperationAdaptors)
        {
            o.OnFinishData(vo);
        }
    }
}