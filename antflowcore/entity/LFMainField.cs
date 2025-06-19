using antflowcore.constant.enums;
using antflowcore.constant.enus;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.util;
using FreeSql.DataAnnotations;

namespace antflowcore.entity;

using FreeSql;
using System;
using System.Collections.Generic;

[Table(Name = "t_lf_main_field")]
public class LFMainField
{
    /// <summary>
    /// 主键ID
    /// </summary>
    [Column(IsPrimary = true, IsIdentity = false)]
    public long Id { get; set; }

    [Column(Name = "main_id")]
    public long MainId { get; set; }

    [Column(Name = "form_code")]
    public string FormCode { get; set; }

    [Column(Name = "field_id")]
    public string FieldId { get; set; }

    [Column(Name = "field_name")]
    public string FieldName { get; set; }

    [Column(Name = "parent_field_id")]
    public string ParentFieldId { get; set; }

    [Column(Name = "parent_field_name")]
    public string ParentFieldName { get; set; }

    [Column(Name = "field_value")]
    public string FieldValue { get; set; }

    [Column(Name = "field_value_number")]
    public double? FieldValueNumber { get; set; }

    [Column(Name = "field_value_dt")]
    public DateTime? FieldValueDt { get; set; }

    [Column(Name = "field_value_text")]
    public string FieldValueText { get; set; }

    public int Sort { get; set; } = 0;

    /// <summary>
    /// 逻辑删除标记（0：未删除，1：已删除）
    /// </summary>
    [Column(Name = "is_del")]
    public int IsDel { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    [Column(Name = "create_user")]
    public string CreateUser { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column(Name = "create_time", CanInsert = true)]
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 更新人
    /// </summary>
    [Column(Name = "update_user")]
    public string UpdateUser { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [Column(Name = "update_time", CanInsert = true, CanUpdate = true)]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 从 Map 解析出字段列表
    /// </summary>
    public static List<LFMainField> ParseFromMap(Dictionary<string, object> fieldMap, Dictionary<string, BpmnConfLfFormdataField> fieldConfigMap, long mainId,string formCode)
    {
        if (fieldMap == null || fieldMap.Count == 0)
            throw new Exception("form data has no value");

        if (fieldConfigMap == null || fieldConfigMap.Count == 0)
            throw new Exception("field configs are empty, please check your logic");

        var mainFields = new List<LFMainField>();

        foreach (var fieldEntry in fieldMap)
        {
            var fieldId = fieldEntry.Key;
            if (!fieldConfigMap.TryGetValue(fieldId, out var fieldConfig))
                continue; // 配置未找到，跳过

            var value = fieldEntry.Value;
            var mainField = BuildMainField(value, mainId, 0, fieldConfig);
            mainField.FormCode= formCode;
            mainFields.Add(mainField);
        }

        return mainFields;
    }

   
    public static LFMainField BuildMainField(object fieldValue, long mainId, int sort, BpmnConfLfFormdataField fieldConfig)
    {
        string fieldValueStr = fieldValue?.ToString();

        var mainField = new LFMainField
        {
            Id = SnowFlake.NextId(),
            MainId = mainId,
            FieldId = fieldConfig.FieldId,
            FieldName = fieldConfig.FieldName,
            Sort = sort
        };
        if (fieldConfig.FieldType == null)
        {
            throw new AFBizException("控件类型未定义!");
        }

      
        var fieldTypeEnum = LFFieldTypeEnum.GetByType(fieldConfig.FieldType.Value);
        if (fieldTypeEnum == null)
            throw new Exception($"field type can not be empty, {fieldConfig}");
       
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
                    mainField.FieldValueNumber = double.TryParse(fieldValueStr, out var fieldValueNumber) ? fieldValueNumber : null;
                }
                break;
            case var fieldType when fieldType == LFFieldTypeEnum.DATE || fieldType == LFFieldTypeEnum.DATE_TIME:
                mainField.FieldValueDt = DateTime.TryParse(fieldValueStr, out var dt) ? dt : null;
                break;
            case var fieldType when fieldType == LFFieldTypeEnum.TEXT:
                mainField.FieldValueText = fieldValueStr;
                break;
            case var fieldType when fieldType == LFFieldTypeEnum.BOOLEAN:
                mainField.FieldValue = bool.TryParse(fieldValueStr, out var boolean) ? boolean.ToString() : null;
                break;
        }
        

        return mainField;
    }
}
