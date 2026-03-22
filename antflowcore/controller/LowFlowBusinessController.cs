using System.Text.Json;
using antflowcore.constant.enus;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using Microsoft.AspNetCore.Mvc;

namespace antflowcore.controller;

/// <summary>
/// 低代码流程业务数据查询接口
/// </summary>
[Route("lowFlowBusiness")]
public class LowFlowBusinessController
{
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly LFMainService _lfMainService;
    private readonly LFMainFieldService _lfMainFieldService;
    private readonly BpmnConfLfFormdataFieldService _lfFormdataFieldService;

    public LowFlowBusinessController(
        BpmBusinessProcessService bpmBusinessProcessService,
        LFMainService lfMainService,
        LFMainFieldService lfMainFieldService,
        BpmnConfLfFormdataFieldService lfFormdataFieldService)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _lfMainService = lfMainService;
        _lfMainFieldService = lfMainFieldService;
        _lfFormdataFieldService = lfFormdataFieldService;
    }

    /// <summary>
    /// 根据流程编号查询低代码流程的业务数据
    /// </summary>
    /// <param name="processNumber">流程编号</param>
    /// <returns>低代码流程业务数据</returns>
    [HttpGet("getBusinessData")]
    public Result<LowFlowBusinessDataVo> GetBusinessData([FromQuery] string processNumber)
    {
        // 1. 根据流程编号查询BpmBusinessProcess表
        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
        if (bpmBusinessProcess == null)
        {
            throw new AFBizException($"未找到流程编号为 {processNumber} 的流程数据");
        }

        string bpmnCode = bpmBusinessProcess.Version;
        if (string.IsNullOrEmpty(bpmnCode))
        {
            throw new AFBizException($"流程编号 {processNumber} 的VERSION字段为空");
        }

        // 2. 根据businessId查询LFMain表
        LFMain lfMain = _lfMainService.baseRepo
            .Where(a => a.Id == long.Parse(bpmBusinessProcess.BusinessId))
            .First();

        if (lfMain == null)
        {
            throw new AFBizException($"未找到流程编号 {processNumber} 的低代码流程数据");
        }

        long mainId = lfMain.Id;
        string formCode = lfMain.FormCode;
        long? confId = lfMain.ConfId;

        // 3. 查询字段配置信息
        Dictionary<string, BpmnConfLfFormdataField> fieldConfigMap = _lfFormdataFieldService.QryFormDataFieldMap(confId.Value);
        if (fieldConfigMap == null || fieldConfigMap.Count == 0)
        {
            throw new AFBizException($"流程配置ID {confId} 没有字段配置");
        }

        // 4. 查询字段值数据
        List<LFMainField> lfMainFields = _lfMainFieldService.baseRepo
            .Where(a => a.MainId == mainId)
            .ToList();

        if (lfMainFields == null || lfMainFields.Count == 0)
        {
            throw new AFBizException($"低代码流程 formCode:{formCode}, confId:{confId} 没有表单数据");
        }

        // 5. 按fieldId分组处理多值字段
        var fieldId2FieldsMap = lfMainFields
            .GroupBy(f => f.FieldId)
            .ToDictionary(g => g.Key, g => g.ToList());

        // 6. 构建返回数据
        List<LowFlowBusinessDataVo.FieldInfo> fieldInfoList = new List<LowFlowBusinessDataVo.FieldInfo>();

        foreach (var entry in fieldId2FieldsMap)
        {
            string fieldId = entry.Key;
            List<LFMainField> fields = entry.Value;

            if (!fieldConfigMap.TryGetValue(fieldId, out var fieldConfig))
            {
                continue;
            }

            // 获取字段值
            object fieldValue = ParseFieldValue(fields, fieldConfig, formCode, confId.Value);

            var fieldInfo = new LowFlowBusinessDataVo.FieldInfo
            {
                FieldId = fieldId,
                FieldName = fieldConfig.FieldName,
                FieldLabel = fieldConfig.FieldName,
                FieldType = fieldConfig.FieldType,
                FieldValue = fieldValue
            };

            fieldInfoList.Add(fieldInfo);
        }

        // 7. 构建最终返回对象
        var result = new LowFlowBusinessDataVo
        {
            MainId = mainId,
            ConfId = confId,
            FormCode = formCode,
            CreateUser = lfMain.CreateUser,
            Fields = fieldInfoList
        };

        return ResultHelper.Success(result);
    }

    /// <summary>
    /// 解析字段值
    /// </summary>
    private object ParseFieldValue(List<LFMainField> fields, BpmnConfLfFormdataField fieldConfig,
        string formCode, long confId)
    {
        if (fields == null || fields.Count == 0)
        {
            return null;
        }

        int? fieldType = fieldConfig.FieldType;
        LFFieldTypeEnum fieldTypeEnum = LFFieldTypeEnum.GetByType(fieldType.Value);

        if (fieldTypeEnum == null)
        {
            throw new AFBizException($"未识别的字段类型,fieldName:{fieldConfig.FieldName}, formCode:{formCode}, confId:{confId}");
        }

        // 如果只有一个值,直接返回
        if (fields.Count == 1)
        {
            return ParseSingleFieldValue(fields[0], fieldTypeEnum);
        }

        // 如果有多个值,返回数组
        List<object> multiValues = new List<object>(fields.Count);
        foreach (LFMainField field in fields)
        {
            multiValues.Add(ParseSingleFieldValue(field, fieldTypeEnum));
        }
        return multiValues;
    }

    /// <summary>
    /// 解析单个字段的值
    /// </summary>
    private object ParseSingleFieldValue(LFMainField field, LFFieldTypeEnum fieldTypeEnum)
    {
        if (fieldTypeEnum == LFFieldTypeEnum.STRING)
        {
            string value = field.FieldValue;
            if (value != null)
            {
                // 尝试解析JSON对象或数组
                if (value.StartsWith("{"))
                {
                    try
                    {
                        return JsonSerializer.Deserialize<Dictionary<string, object>>(value);
                    }
                    catch
                    {
                        // 解析失败,返回原字符串
                    }
                }
                else if (value.StartsWith("["))
                {
                    try
                    {
                        return JsonSerializer.Deserialize<List<object>>(value);
                    }
                    catch
                    {
                        // 解析失败,返回原字符串
                    }
                }
            }
            return value;
        }
        else if (fieldTypeEnum == LFFieldTypeEnum.NUMBER)
        {
            return field.FieldValueNumber;
        }
        else if (fieldTypeEnum == LFFieldTypeEnum.DATE_TIME)
        {
            if (field.FieldValueDt != null)
            {
                return field.FieldValueDt.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return null;
        }
        else if (fieldTypeEnum == LFFieldTypeEnum.DATE)
        {
            if (field.FieldValueDt != null)
            {
                return field.FieldValueDt.Value.ToString("yyyy-MM-dd");
            }
            return null;
        }
        else if (fieldTypeEnum == LFFieldTypeEnum.TEXT)
        {
            return field.FieldValueText;
        }
        else if (fieldTypeEnum == LFFieldTypeEnum.BOOLEAN)
        {
            return bool.TryParse(field.FieldValue, out var boolean) && boolean;
        }
        else
        {
            return field.FieldValue;
        }
    }
}
