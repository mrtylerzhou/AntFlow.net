using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Util;
using AntFlowException = AntFlow.Core.Exception;

namespace AntFlow.Core.Entity;

public class LFMainField
{
    /// <summary>
    ///     ????ID
    /// </summary>
    public long Id { get; set; }

    public long MainId { get; set; }

    public string FormCode { get; set; }

    public string FieldId { get; set; }

    public string FieldName { get; set; }

    public string ParentFieldId { get; set; }

    public string ParentFieldName { get; set; }

    public string FieldValue { get; set; }

    public double? FieldValueNumber { get; set; }

    public DateTime? FieldValueDt { get; set; }

    public string FieldValueText { get; set; }

    public int Sort { get; set; }

    /// <summary>
    ///     ??????????0??��?????1?????????
    /// </summary>
    public int IsDel { get; set; }

    public string TenantId { get; set; }

    /// <summary>
    ///     ??????
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    ///     ???????
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    ///     ??????
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    ///     ???????
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    ///     ?? Map ??????????��?
    /// </summary>
    public static List<LFMainField> ParseFromMap(Dictionary<string, object> fieldMap,
        Dictionary<string, BpmnConfLfFormdataField> fieldConfigMap, long mainId, string formCode)
    {
        if (fieldMap == null || fieldMap.Count == 0)
        {
            throw new System.Exception("form data has no value");
        }

        if (fieldConfigMap == null || fieldConfigMap.Count == 0)
        {
            throw new System.Exception("field configs are empty, please check your logic");
        }

        List<LFMainField>? mainFields = new();

        foreach (KeyValuePair<string, object> fieldEntry in fieldMap)
        {
            string? fieldId = fieldEntry.Key;
            if (!fieldConfigMap.TryGetValue(fieldId, out BpmnConfLfFormdataField? fieldConfig))
            {
                continue; // ????��?????????
            }

            object? value = fieldEntry.Value;
            LFMainField? mainField = BuildMainField(value, mainId, 0, fieldConfig);
            mainField.FormCode = formCode;
            mainFields.Add(mainField);
        }

        return mainFields;
    }

    public static LFMainField BuildMainField(object fieldValue, long mainId, int sort,
        BpmnConfLfFormdataField fieldConfig)
    {
        string fieldValueStr = fieldValue?.ToString();

        LFMainField? mainField = new()
        {
            Id = SnowFlake.NextId(),
            MainId = mainId,
            FieldId = fieldConfig.FieldId,
            FieldName = fieldConfig.FieldName,
            Sort = sort
        };

        if (fieldConfig.FieldType == null)
        {
            throw new AntFlowException.AFBizException("???????��????");
        }

        LFFieldTypeEnum? fieldTypeEnum = LFFieldTypeEnum.GetByType(fieldConfig.FieldType.Value);
        if (fieldTypeEnum == null)
        {
            throw new System.Exception($"field type can not be empty, {fieldConfig}");
        }

        switch (fieldTypeEnum)
        {
            case var fieldType when fieldType == LFFieldTypeEnum.STRING:
                mainField.FieldValue = fieldValueStr;
                break;
            case var fieldType when fieldType == LFFieldTypeEnum.NUMBER:
                if (LFControlTypeEnum.SELECT.Name == fieldConfig.FieldName)
                {
                    mainField.FieldValue = fieldValueStr;
                }
                else
                {
                    mainField.FieldValueNumber = double.TryParse(fieldValueStr, out double fieldValueNumber)
                        ? fieldValueNumber
                        : null;
                }

                break;
            case var fieldType when fieldType == LFFieldTypeEnum.DATE || fieldType == LFFieldTypeEnum.DATE_TIME:
                mainField.FieldValueDt = DateTime.TryParse(fieldValueStr, out DateTime dt) ? dt : null;
                break;
            case var fieldType when fieldType == LFFieldTypeEnum.TEXT:
                mainField.FieldValueText = fieldValueStr;
                break;
            case var fieldType when fieldType == LFFieldTypeEnum.BOOLEAN:
                mainField.FieldValue = bool.TryParse(fieldValueStr, out bool boolean) ? boolean.ToString() : null;
                break;
        }

        return mainField;
    }
}