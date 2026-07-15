using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Engine.factory;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.processor.lowcodeflow;

/**
 * 低(无)代码流程
 */
[DIYFormServiceAnno(SvcName = "LF", Desc = "")]
public class LowFlowApprovalService : IFormOperationAdaptor<UDLFApplyVo>
{
    private readonly ILogger<LowFlowApprovalService> _logger;
    private readonly ILFMainService _mainService;
    private readonly ILFMainFieldService _lfMainFieldService;
    private readonly IBpmnConfLfFormdataService _lfformdataService;
    private readonly IBpmnConfLfFormdataFieldService _lfformdataFieldService;
    private readonly IBpmnConfService _bpmnConfService;
    private readonly IBpmnNodeService _bpmnNodeService;
    private readonly IBpmnConfLfFormdataRepository _lfFormdataRepository;

    private static Dictionary<long, List<String>> conditionFieldNameMap = new Dictionary<long, List<string>>();

    // key is confid,value is a map of field's name and its self
    private static Dictionary<long, Dictionary<String, BpmnConfLfFormdataField>> allFieldConfMap =
        new Dictionary<long, Dictionary<string, BpmnConfLfFormdataField>>();

    // key is formdataId,value is a map of field's name and its self (external form mode)
    private static Dictionary<long, Dictionary<String, BpmnConfLfFormdataField>> allFieldConfMapByFormdataId =
        new Dictionary<long, Dictionary<string, BpmnConfLfFormdataField>>();

    public LowFlowApprovalService(ILogger<LowFlowApprovalService> logger, ILFMainService mainService,
        ILFMainFieldService lfMainFieldService,
        IBpmnConfLfFormdataService lfformdataService,
        IBpmnConfLfFormdataFieldService lfformdataFieldService,
        IBpmnConfService bpmnConfService,
        IBpmnNodeService bpmnNodeService,
        IBpmnConfLfFormdataRepository lfFormdataRepository)
    {
        _logger = logger;
        _mainService = mainService;
        _lfMainFieldService = lfMainFieldService;
        _lfformdataService = lfformdataService;
        _lfformdataFieldService = lfformdataFieldService;
        _bpmnConfService = bpmnConfService;
        _bpmnNodeService = bpmnNodeService;
        _lfFormdataRepository = lfFormdataRepository;
    }

    public BpmnStartConditionsVo PreviewSetCondition(UDLFApplyVo vo)
    {
        FlattenLfFieldsMultiIfNeeded(vo);

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
            startConditionsVo.LfConditions = vo.LfFields;
        }

        BpmnConfVo bpmnConfVo = vo.BpmnConfVo;
        ProcessFormRelatedUserConf(bpmnConfVo, vo);
        startConditionsVo.BusinessDataVo = vo;

        return startConditionsVo;
    }

    public BpmnStartConditionsVo LaunchParameters(UDLFApplyVo vo)
    {
        FlattenLfFieldsMultiIfNeeded(vo);

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
            startConditionsVo.LfConditions = vo.LfFields;
        }

        return startConditionsVo;
    }

    public void OnInitData(UDLFApplyVo vo)
    {
        IEnumerable<ILFFormOperationAdaptor> lfFormOperationAdaptors = ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
        foreach (ILFFormOperationAdaptor o in lfFormOperationAdaptors)
        {
            LFFormServiceAnnoAttribute? lfFormServiceAnnoAttribute = o.GetType().GetCustomAttribute<LFFormServiceAnnoAttribute>();

            if (lfFormServiceAnnoAttribute != null && lfFormServiceAnnoAttribute.SvcName.Equals(vo.FormCode))
            {
                o.OnInitData(vo);
            }
        }
    }

    public void OnQueryData(UDLFApplyVo vo)
    {
        LFMain lfMain = _mainService._repository.FirstOrDefault(a => a.Id == long.Parse(vo.BusinessId));
        if (lfMain == null)
        {
            _logger.LogError("can not get lowcode from data by specified Id:{0}", vo.BusinessId);
            throw new AFBizException("can not get lowcode form data by specified id");
        }

        long mainId = lfMain.Id;
        long confId = lfMain.ConfId.Value;
        string formCode = lfMain.FormCode;

        // 外部表单模式: 按 lf_formdata_ids 加载多表单
        BpmnConf? bpmnConf = _bpmnConfService._repository.FirstOrDefault(a => a.Id == confId);
        if (bpmnConf != null && BpmnConfFlagsEnum.HasFlag(bpmnConf.ExtraFlags, BpmnConfFlagsEnum.USE_EXTERNAL_FORM))
        {
            QueryDataExternal(vo, bpmnConf, mainId, confId);
            return;
        }

        // 内联表单模式: 兼容旧逻辑,加载单个表单
        if (!allFieldConfMap.TryGetValue(confId, out var lfFormdataFieldMap) || lfFormdataFieldMap == null)
        {
            lfFormdataFieldMap = _lfformdataFieldService.QryFormDataFieldMap(confId);
            allFieldConfMap[confId] = lfFormdataFieldMap;
        }

        List<LFMainField> lfMainFields = _lfMainFieldService._repository.Find(x => x.MainId == mainId);
        if (lfMainFields == null || !lfMainFields.Any())
        {
            throw new AFBizException($"lowcode form with formcode:{formCode}, confid:{confId} has no formdata");
        }

        Dictionary<string, object> fieldVoMap = BuildFieldVoMap(lfMainFields, lfFormdataFieldMap, formCode, confId);
        vo.LfFields = fieldVoMap;

        string? lfFormData = GetLfFormDataFromJson(confId);
        if (string.IsNullOrWhiteSpace(lfFormData))
        {
            List<BpmnConfLfFormdata> bpmnConfLfFormdataList =
                _lfformdataService._repository.Find(x => x.BpmnConfId == confId);
            if (bpmnConfLfFormdataList == null || !bpmnConfLfFormdataList.Any())
            {
                throw new AFBizException($"can not get lowcode flow formdata by confId:{confId}");
            }

            lfFormData = bpmnConfLfFormdataList.First().Formdata;
        }

        vo.LfFormData = lfFormData;
        IEnumerable<ILFFormOperationAdaptor> lfFormOperationAdaptors = ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
        foreach (var o in lfFormOperationAdaptors)
        {
            LFFormServiceAnnoAttribute? lfFormServiceAnnoAttribute = o.GetType().GetCustomAttribute<LFFormServiceAnnoAttribute>();

            if (lfFormServiceAnnoAttribute != null && lfFormServiceAnnoAttribute.SvcName.Equals(vo.FormCode))
            {
                o.OnQueryData(vo);
            }
        }
    }

    /// <summary>
    /// 外部表单模式 queryData: 按 lf_formdata_ids 加载多表单定义及字段值
    /// </summary>
    private void QueryDataExternal(UDLFApplyVo vo, BpmnConf bpmnConf, long mainId, long confId)
    {
        string lfFormdataIds = bpmnConf.LfFormdataIds;
        if (string.IsNullOrEmpty(lfFormdataIds))
        {
            throw new AFBizException($"external form mode but lf_formdata_ids is empty, confId:{confId}");
        }

        List<long> ids = lfFormdataIds.Split(',')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(long.Parse)
            .ToList();

        List<BpmnConfLfFormdata> forms = _lfFormdataRepository.ListByIdsIgnoreDeleted(ids);
        if (forms == null || !forms.Any())
        {
            throw new AFBizException($"can not get external forms by ids:{lfFormdataIds}");
        }

        List<LFMainField> allMainFields = _lfMainFieldService._repository.Find(x => x.MainId == mainId);
        var fieldsByFormdataId = allMainFields?
            .GroupBy(f => f.FormdataId ?? -1L)
            .ToDictionary(g => g.Key, g => g.ToList())
            ?? new Dictionary<long, List<LFMainField>>();

        var lfFieldsMulti = new Dictionary<string, Dictionary<string, object>>();
        var flatFields = new Dictionary<string, object>();

        foreach (var form in forms)
        {
            long formdataId = form.Id;
            if (!fieldsByFormdataId.TryGetValue(formdataId, out var formFields) || formFields == null)
            {
                // 该表单无数据(可能后加的表单),给空Map
                lfFieldsMulti[formdataId.ToString()] = new Dictionary<string, object>();
                continue;
            }

            if (!allFieldConfMapByFormdataId.TryGetValue(formdataId, out var fieldConfMap) || fieldConfMap == null)
            {
                fieldConfMap = _lfformdataFieldService.QryFieldMapByFormdataId(formdataId);
                allFieldConfMapByFormdataId[formdataId] = fieldConfMap;
            }

            var fieldVoMap = BuildFieldVoMap(formFields, fieldConfMap, form.FormCode, confId);
            lfFieldsMulti[formdataId.ToString()] = fieldVoMap;
            foreach (var kv in fieldVoMap)
            {
                flatFields[kv.Key] = kv.Value;
            }
        }

        vo.LfFieldsMulti = lfFieldsMulti;
        vo.LfFields = flatFields;
        vo.LfFormdataList = forms;
    }

    public void OnSubmitData(UDLFApplyVo vo)
    {
        BpmnConfVo bpmnConfVo = vo.BpmnConfVo;

        // 外部表单模式
        if (BpmnConfFlagsEnum.HasFlag(bpmnConfVo.ExtraFlags, BpmnConfFlagsEnum.USE_EXTERNAL_FORM))
        {
            SubmitDataExternal(vo, bpmnConfVo);
            return;
        }

        // 内联表单模式
        var lfFields = vo.LfFields;
        if (lfFields == null || lfFields.Count == 0)
        {
            throw new AFBizException("form data does not contain any field");
        }

        //判断字段值是否超长
        foreach (var key in lfFields.Keys.ToList())
        {
            var val = lfFields[key];
            string valueStr = val == null ? "" : val.ToString();
            if (valueStr.Length > 2000)
            {
                lfFields[key] = "该字段超出了表字段设计的最大长度，不做存储，防止antflow表字段长度溢出";
            }
        }

        long confId = bpmnConfVo.Id;
        string formCode = vo.FormCode;

        // 发起人节点字段权限校验: 过滤掉隐藏(H)字段,防止前端绕过
        var startLowCodeConf = GetLowCodeConfJson(confId, ProcessNodeEnum.START_TASK_KEY.Description);
        if (startLowCodeConf?.FieldControls != null && startLowCodeConf.FieldControls.Count > 0)
        {
            foreach (var key in lfFields.Keys.ToList())
            {
                var ctrl = startLowCodeConf.FieldControls.FirstOrDefault(c => c.FieldId == key);
                if (ctrl != null && StringConstants.HIDDEN_FIELD_PERMISSION.Equals(ctrl.Perm))
                {
                    lfFields.Remove(key);
                }
            }
        }

        var main = new LFMain
        {
            Id = SnowFlake.NextId(),
            ConfId = confId,
            FormCode = formCode,
            CreateUser = SecurityUtils.GetLogInEmpName(),
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };
        _mainService._repository.Add(main);
        long mainId = main.Id;

        if (!allFieldConfMap.TryGetValue(confId, out var lfFormdataFieldMap) || lfFormdataFieldMap == null || lfFormdataFieldMap.Count == 0)
        {
            Dictionary<string,BpmnConfLfFormdataField> name2SelfMap = _lfformdataFieldService.QryFormDataFieldMap(confId);
            allFieldConfMap[confId] = name2SelfMap;
        }

        if (!allFieldConfMap.TryGetValue(confId, out var fieldConfMap) || fieldConfMap == null || fieldConfMap.Count == 0)
        {
            throw new AFBizException($"confId {confId}, formCode:{vo.FormCode} does not have a field config");
        }

        var mainFields = LFMainField.ParseFromMap(lfFields, fieldConfMap, mainId,formCode);
        _lfMainFieldService._repository.AddRange(mainFields);

        vo.BusinessId = mainId.ToString();
        vo.ProcessDigest = vo.Remark;
        vo.EntityName = nameof(LowFlowApprovalService);
        ProcessFormRelatedUserConf(vo.BpmnConfVo, vo);
        IEnumerable<ILFFormOperationAdaptor> lfFormOperationAdaptors = ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
        foreach (var o in lfFormOperationAdaptors)
        {
            LFFormServiceAnnoAttribute? lfFormServiceAnnoAttribute = o.GetType().GetCustomAttribute<LFFormServiceAnnoAttribute>();

            if (lfFormServiceAnnoAttribute != null && lfFormServiceAnnoAttribute.SvcName.Equals(vo.FormCode))
            {
                o.OnSubmitData(vo);
            }
        }
    }

    /// <summary>
    /// 外部表单模式 submitData: 按 formdataId 分组保存字段值
    /// </summary>
    private void SubmitDataExternal(UDLFApplyVo vo, BpmnConfVo bpmnConfVo)
    {
        var lfFieldsMulti = vo.LfFieldsMulti;
        if (lfFieldsMulti == null || lfFieldsMulti.Count == 0)
        {
            throw new AFBizException("form data does not contains any field");
        }

        //判断字段值是否超长
        foreach (var formFields in lfFieldsMulti.Values)
        {
            if (formFields == null) continue;
            foreach (var key in formFields.Keys.ToList())
            {
                var val = formFields[key];
                string valueStr = val == null ? "" : val.ToString();
                if (valueStr.Length > 2000)
                {
                    formFields[key] = "该字段超出了表字段设计的最大长度，不做存储，防止antflow表字段长度溢出";
                }
            }
        }

        long confId = bpmnConfVo.Id;
        string formCode = vo.FormCode;
        string currentTenantId = MultiTenantUtil.GetCurrentTenantId();

        var main = new LFMain
        {
            Id = SnowFlake.NextId(),
            ConfId = confId,
            FormCode = formCode,
            CreateUser = SecurityUtils.GetLogInEmpName(),
            TenantId = currentTenantId,
        };
        _mainService._repository.Add(main);
        long mainId = main.Id;

        var allMainFields = new List<LFMainField>();

        // 发起人节点字段权限校验: 过滤隐藏表单和隐藏字段
        var startLowCodeConf = GetLowCodeConfJson(confId, ProcessNodeEnum.START_TASK_KEY.Description);
        var startFormHidden = startLowCodeConf?.FormHidden;
        var startFieldControls = (startLowCodeConf?.FieldControls != null) ? startLowCodeConf.FieldControls : new List<LFFieldControlVO>();

        foreach (var entry in lfFieldsMulti)
        {
            long formdataId = long.Parse(entry.Key);

            // 整表隐藏: 跳过该表单
            if (startFormHidden != null && startFormHidden.TryGetValue(formdataId.ToString(), out var isHidden) && isHidden)
            {
                continue;
            }

            var fields = entry.Value;
            if (fields == null || fields.Count == 0)
            {
                continue;
            }

            // 过滤隐藏字段
            if (startFieldControls.Count > 0)
            {
                foreach (var key in fields.Keys.ToList())
                {
                    var ctrl = startFieldControls.FirstOrDefault(c => c.FormdataId == formdataId && c.FieldId == key);
                    if (ctrl != null && StringConstants.HIDDEN_FIELD_PERMISSION.Equals(ctrl.Perm))
                    {
                        fields.Remove(key);
                    }
                }
            }

            if (!allFieldConfMapByFormdataId.TryGetValue(formdataId, out var fieldConfMap) || fieldConfMap == null)
            {
                fieldConfMap = _lfformdataFieldService.QryFieldMapByFormdataId(formdataId);
                allFieldConfMapByFormdataId[formdataId] = fieldConfMap;
            }

            List<LFMainField> mainFields = LFMainField.ParseFromMap(fields, fieldConfMap, mainId, formCode, formdataId);
            allMainFields.AddRange(mainFields);
        }

        if (allMainFields.Any())
        {
            _lfMainFieldService._repository.AddRange(allMainFields);
        }

        vo.BusinessId = mainId.ToString();
        vo.ProcessDigest = vo.Remark;
        vo.EntityName = nameof(LowFlowApprovalService);
    }

    public void OnConsentData(UDLFApplyVo vo)
    {
        if (vo.OperationType != (int)ButtonTypeEnum.BUTTON_TYPE_RESUBMIT&&vo.OperationType!=(int)ButtonTypeEnum.BUTTON_TYPE_AGREE)
        {
            return ;
        }

        BpmnConfVo bpmnConfVo = vo.BpmnConfVo;

        // 外部表单模式
        if (BpmnConfFlagsEnum.HasFlag(bpmnConfVo.ExtraFlags, BpmnConfFlagsEnum.USE_EXTERNAL_FORM))
        {
            ConsentDataExternal(vo, bpmnConfVo);
            return;
        }

        // 内联表单模式
        var lfFields = vo.LfFields;
        if (lfFields == null || lfFields.Count == 0)
        {
            throw new AFBizException("form data does not contain any field");
        }

        var lfMain = _mainService._repository.FirstOrDefault(a => a.Id == long.Parse(vo.BusinessId));
        if (lfMain == null)
        {
            _logger.LogError($"can not get lowcode from data by specified Id:{vo.BusinessId}");
            throw new AFBizException("can not get lowcode form data by specified id");
        }

        long mainId = lfMain.Id;
        string formCode = vo.FormCode;
        long confId = bpmnConfVo.Id;

        List<LFMainField> lfMainFields = _lfMainFieldService._repository.Find(a=>a.MainId==mainId);
        if (lfMainFields == null || lfMainFields.Count == 0)
        {
            throw new AFBizException($"lowcode form with formcode:{formCode}, confId:{confId} has no formdata");
        }

        Dictionary<string,object> submitLfFields = vo.LfFields;
        if (submitLfFields != null && submitLfFields.Any())
        {
            if (!allFieldConfMap.TryGetValue(confId, out var lfFormdataFieldMap))
            {
                if (lfFormdataFieldMap == null || lfFormdataFieldMap.Count == 0)
                {
                    Dictionary<string,BpmnConfLfFormdataField> name2SelfMap = _lfformdataFieldService.QryFormDataFieldMap(confId);
                    allFieldConfMap.Add(confId,name2SelfMap);
                }
            }
            if (allFieldConfMap.TryGetValue(confId,out var fieldConfMap))
            {
                List<LFMainField> mainFields = LFMainField.ParseFromMap(submitLfFields, fieldConfMap, mainId, vo.FormCode);
                if (mainFields != null && mainFields.Count > 0)
                {
                    // 根据fieldId过滤掉已存在表里的数据lfMainFields
                    mainFields.RemoveAll(mainField=>lfMainFields.Any(a=>a.FieldId==mainField.FieldId));
                    if(mainFields.Any())
                    {
                        _lfMainFieldService._repository.AddRange(mainFields);
                    }
                }
            }
            else
            {
                throw new AFBizException($"confId {confId}, formCode:{vo.FormCode} does not have a field config");
            }
        }
        // IBpmnNodeLfFormdataFieldControlService has been removed; field control check is no longer supported
        foreach (LFMainField field in lfMainFields)
        {
            string fValue = lfFields[field.FieldId]?.ToString()??null;
            if (!StringConstants.HIDDEN_FIELD_VALUE.Equals(fValue))//如果是******,实际上是隐藏字段,不更新
            {
                field.FieldValue = fValue;
            }
        }
        _lfMainFieldService._repository.UpdateRange(lfMainFields);
        IEnumerable<ILFFormOperationAdaptor> lfFormOperationAdaptors = ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
        foreach (ILFFormOperationAdaptor o in lfFormOperationAdaptors)
        {

            LFFormServiceAnnoAttribute? lfFormServiceAnnoAttribute = o.GetType().GetCustomAttribute<LFFormServiceAnnoAttribute>();

            if (lfFormServiceAnnoAttribute != null && lfFormServiceAnnoAttribute.SvcName.Equals(vo.FormCode))
            {
                o.OnConsentData(vo);
            }
        }
    }

    /// <summary>
    /// 外部表单模式 consentData: 尊重 formHidden(整表隐藏) + 按 formdataId 匹配字段权限
    /// </summary>
    private void ConsentDataExternal(UDLFApplyVo vo, BpmnConfVo bpmnConfVo)
    {
        var lfFieldsMulti = vo.LfFieldsMulti;
        if (lfFieldsMulti == null || lfFieldsMulti.Count == 0)
        {
            throw new AFBizException("form data does not contains any field");
        }

        LFMain lfMain = _mainService._repository.FirstOrDefault(a => a.Id == long.Parse(vo.BusinessId));
        if (lfMain == null)
        {
            _logger.LogError($"can not get lowcode from data by specified Id:{vo.BusinessId}");
            throw new AFBizException("can not get lowcode form data by specified id");
        }

        long mainId = lfMain.Id;
        string formCode = vo.FormCode;
        long confId = bpmnConfVo.Id;

        List<LFMainField> allMainFields = _lfMainFieldService._repository.Find(a => a.MainId == mainId);
        if (allMainFields == null || !allMainFields.Any())
        {
            throw new AFBizException($"lowcode form with formcode:{formCode}, confId:{confId} has no formdata");
        }

        // 获取节点级配置: formHidden + fieldControls
        BpmnNodeLowCodeConfJson? lowCodeConf = GetLowCodeConfJson(confId, vo.TaskDefKey);
        Dictionary<string, bool>? formHidden = lowCodeConf?.FormHidden;
        List<LFFieldControlVO> fieldControls =
            (lowCodeConf?.FieldControls != null) ? lowCodeConf.FieldControls : new List<LFFieldControlVO>();

        // 保存新增字段(提交数据中有但DB中没有的),按 formdataId 分组
        var existingByFormdataId = allMainFields
            .GroupBy(f => f.FormdataId ?? -1L)
            .ToDictionary(g => g.Key, g => g.ToList());

        var newFields = new List<LFMainField>();
        foreach (var entry in lfFieldsMulti)
        {
            long formdataId = long.Parse(entry.Key);
            var submitFields = entry.Value;
            if (submitFields == null || submitFields.Count == 0)
            {
                continue;
            }

            List<LFMainField> existingFields;
            if (!existingByFormdataId.TryGetValue(formdataId, out existingFields))
            {
                existingFields = new List<LFMainField>();
            }

            if (!allFieldConfMapByFormdataId.TryGetValue(formdataId, out var fieldConfMap) || fieldConfMap == null)
            {
                fieldConfMap = _lfformdataFieldService.QryFieldMapByFormdataId(formdataId);
                allFieldConfMapByFormdataId[formdataId] = fieldConfMap;
            }

            List<LFMainField> parsed = LFMainField.ParseFromMap(submitFields, fieldConfMap, mainId, formCode, formdataId);
            // 过滤掉已存在的fieldId
            parsed.RemoveAll(nf => existingFields.Any(ori => ori.FieldId == nf.FieldId));
            newFields.AddRange(parsed);
        }

        if (newFields.Any())
        {
            _lfMainFieldService._repository.AddRange(newFields);
            allMainFields.AddRange(newFields);
        }

        // 更新已有字段值,尊重 formHidden 和字段级权限
        foreach (LFMainField field in allMainFields)
        {
            long? formdataId = field.FormdataId;
            // 整表隐藏的表单不更新
            if (formHidden != null && formdataId.HasValue && formHidden.TryGetValue(formdataId?.ToString(), out var isHidden) && isHidden)
            {
                continue;
            }

            // 字段级权限检查: 同时匹配 formdataId 和 fieldId
            if (fieldControls != null && fieldControls.Any())
            {
                long fdId = formdataId ?? 0;
                var ctrl = fieldControls.FirstOrDefault(c => c.FormdataId == fdId && c.FieldId == field.FieldId);
                if (ctrl != null
                    && (StringConstants.HIDDEN_FIELD_PERMISSION.Equals(ctrl.Perm)
                        || StringConstants.READ_ONLY_FIELD_PERMISSION.Equals(ctrl.Perm)))
                {
                    continue;
                }
            }

            if (formdataId.HasValue)
            {
                string key = formdataId.Value.ToString();
                if (lfFieldsMulti.TryGetValue(key, out var formFields)
                    && formFields != null
                    && formFields.TryGetValue(field.FieldId, out var fieldValue)
                    && fieldValue != null)
                {
                    string fValue = fieldValue.ToString();
                    if (!"******".Equals(fValue))
                    {
                        field.FieldValue = fValue;
                    }
                    _lfMainFieldService._repository.Update(field);
                }
            }
        }
    }

    public void OnBackToModifyData(UDLFApplyVo vo)
    {
        IEnumerable<ILFFormOperationAdaptor> lfFormOperationAdaptors = ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
        foreach (ILFFormOperationAdaptor o in lfFormOperationAdaptors)
        {

            LFFormServiceAnnoAttribute? lfFormServiceAnnoAttribute = o.GetType().GetCustomAttribute<LFFormServiceAnnoAttribute>();

            if (lfFormServiceAnnoAttribute != null && lfFormServiceAnnoAttribute.SvcName.Equals(vo.FormCode))
            {
                o.OnBackToModifyData(vo);
            }
        }
    }

    public void OnCancellationData(UDLFApplyVo vo)
    {
        IEnumerable<ILFFormOperationAdaptor> lfFormOperationAdaptors = ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
        foreach (ILFFormOperationAdaptor o in lfFormOperationAdaptors)
        {
            LFFormServiceAnnoAttribute? lfFormServiceAnnoAttribute = o.GetType().GetCustomAttribute<LFFormServiceAnnoAttribute>();

            if (lfFormServiceAnnoAttribute != null && lfFormServiceAnnoAttribute.SvcName.Equals(vo.FormCode))
            {
                o.OnCancellationData(vo);
            }
        }
    }

    public void OnFinishData(BusinessDataVo vo)
    {
        IEnumerable<ILFFormOperationAdaptor> lfFormOperationAdaptors = ServiceProviderUtils.GetServices<ILFFormOperationAdaptor>();
        foreach (ILFFormOperationAdaptor o in lfFormOperationAdaptors)
        {
            LFFormServiceAnnoAttribute? lfFormServiceAnnoAttribute = o.GetType().GetCustomAttribute<LFFormServiceAnnoAttribute>();

            if (lfFormServiceAnnoAttribute != null && lfFormServiceAnnoAttribute.SvcName.Equals(vo.FormCode))
            {
                o.OnFinishData(vo);
            }
        }
    }

    // ===================== 自动节点条件判断 =====================

    /// <summary>
    /// 自动节点条件判断.
    /// 从节点配置的 autoNodeConf 中读取条件, 对 lfFields 进行基础评估.
    /// 如果没有配置条件, 返回 null (无条件执行 automaticAction).
    /// 对应 Java AbstractFormOperationAdaptor.automaticCondition.
    /// </summary>
    public bool? AutomaticCondition(UDLFApplyVo vo)
    {
        try
        {
            BpmnNodeAutoNodeConfJson? autoNodeConf = LoadAutoNodeConf(vo);
            if (autoNodeConf == null || autoNodeConf.ConditionList == null || autoNodeConf.ConditionList.Count == 0)
            {
                return null;
            }
            Dictionary<string, object>? lfFields = vo.LfFields;
            if (lfFields == null || lfFields.Count == 0)
            {
                return false;
            }
            return EvaluateConditions(autoNodeConf, lfFields);
        }
        catch (AFBizException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "automaticCondition evaluation failed, returning null");
            return null;
        }
    }

    /// <summary>
    /// 自动节点动作执行. 默认不做任何操作.
    /// 对应 Java AbstractFormOperationAdaptor.automaticAction.
    /// </summary>
    public void AutomaticAction(UDLFApplyVo vo, bool? conditionResult)
    {
        // 默认不执行任何动作, 子类可重写
    }

    /// <summary>
    /// 从数据库加载自动节点条件配置.
    /// 对应 Java AbstractFormOperationAdaptor.loadAutoNodeConf.
    /// </summary>
    private BpmnNodeAutoNodeConfJson? LoadAutoNodeConf(BusinessDataVo vo)
    {
        string? processNumber = vo.ProcessNumber;
        string? taskDefKey = vo.TaskDefKey;
        if (string.IsNullOrEmpty(processNumber) || string.IsNullOrEmpty(taskDefKey))
        {
            return null;
        }

        BpmnConf bpmnConf = _bpmnConfService._repository.GetBpmnConfByFormCode(vo.FormCode);
        if (bpmnConf == null || bpmnConf.Id == 0)
        {
            throw new AFBizException("cant not get bpmnconf by formcode:" + vo.FormCode);
        }

        IBpmVariableService bpmVariableService = ServiceProviderUtils.GetService<IBpmVariableService>();
        NodeElementDto? nodeElementDto = bpmVariableService._repository.GetNodeIdByElementId(processNumber, taskDefKey);
        if (nodeElementDto == null || string.IsNullOrEmpty(nodeElementDto.NodeId))
        {
            return null;
        }
        long longId = long.Parse(nodeElementDto.NodeId);

        BpmnNode? bpmnNode = _bpmnNodeService._repository
            .FirstOrDefault(a => a.ConfId == bpmnConf.Id && a.Id == longId && a.IsDel == 0);
        if (bpmnNode == null || string.IsNullOrEmpty(bpmnNode.NodeConfigJson))
        {
            return null;
        }

        BpmnNodeConfigJson? configJson = JsonConfUtil.ParseNodeConfig(bpmnNode.NodeConfigJson);
        return configJson?.AutoNodeConf;
    }

    /// <summary>
    /// 评估自动节点条件.
    /// groupRelation: false=组间AND, true=组间OR
    /// 对应 Java AbstractFormOperationAdaptor.evaluateConditions.
    /// </summary>
    private bool EvaluateConditions(BpmnNodeAutoNodeConfJson autoNodeConf, Dictionary<string, object> formFields)
    {
        List<List<BpmnNodeConditionsConfVueVo>>? conditionList = autoNodeConf.ConditionList;
        if (conditionList == null || conditionList.Count == 0)
        {
            return false;
        }

        bool isOrBetweenGroups = autoNodeConf.GroupRelation ?? false;
        bool overallResult = !isOrBetweenGroups; // AND starts true, OR starts false

        foreach (var group in conditionList)
        {
            if (group == null || group.Count == 0)
            {
                continue;
            }
            bool groupResult = EvaluateConditionGroup(group, formFields);

            if (isOrBetweenGroups)
            {
                overallResult = overallResult || groupResult;
                if (overallResult) break; // OR: first true wins
            }
            else
            {
                overallResult = overallResult && groupResult;
                if (!overallResult) break; // AND: first false wins
            }
        }
        return overallResult;
    }

    /// <summary>
    /// 评估单个条件组.
    /// condRelation (取组内第一个条件的值): false=组内AND, true=组内OR
    /// </summary>
    private bool EvaluateConditionGroup(List<BpmnNodeConditionsConfVueVo> group, Dictionary<string, object> formFields)
    {
        bool isOrWithinGroup = group[0].CondRelation;
        bool groupResult = !isOrWithinGroup;

        foreach (var item in group)
        {
            bool itemResult = EvaluateSingleCondition(item, formFields);
            if (isOrWithinGroup)
            {
                groupResult = groupResult || itemResult;
                if (groupResult) break;
            }
            else
            {
                groupResult = groupResult && itemResult;
                if (!groupResult) break;
            }
        }
        return groupResult;
    }

    /// <summary>
    /// 评估单个条件项.
    /// </summary>
    private bool EvaluateSingleCondition(BpmnNodeConditionsConfVueVo item, Dictionary<string, object> formFields)
    {
        string? fieldName = item.ColumnDbname;
        if (string.IsNullOrEmpty(fieldName))
        {
            return false;
        }
        formFields.TryGetValue(fieldName, out var formValue);
        string formValueStr = formValue != null ? formValue.ToString() : "";
        string targetValue = item.Zdy1 ?? "";

        string? fieldTypeName = item.FieldTypeName;
        int? optType = item.OptType;

        // switch: 比较布尔值
        if ("switch".Equals(fieldTypeName))
        {
            return "1".Equals(formValueStr) == "1".Equals(targetValue);
        }

        // select / radio: 等值判断
        if ("select".Equals(fieldTypeName) || "radio".Equals(fieldTypeName))
        {
            return targetValue.Equals(formValueStr);
        }

        // checkbox: 检查表单值集合是否包含目标元素
        if ("checkbox".Equals(fieldTypeName))
        {
            if (string.IsNullOrEmpty(formValueStr) || string.IsNullOrEmpty(targetValue))
            {
                return false;
            }
            return formValueStr.Split(',').Contains(targetValue);
        }

        // 数字 / 日期 / 时间比较
        try
        {
            if ("number".Equals(fieldTypeName) || "date".Equals(fieldTypeName) || "time".Equals(fieldTypeName))
            {
                return CompareNumeric(formValueStr, targetValue, optType, item.Zdy2, item.Opt1, item.Opt2);
            }
        }
        catch (FormatException)
        {
            _logger.LogDebug("Numeric comparison failed for field {FieldName}", fieldName);
        }

        // 默认: 字符串等值
        return targetValue.Equals(formValueStr);
    }

    /// <summary>
    /// 数字比较, 支持: >=, >, <=, <, ==, between.
    /// optType: 1=>=, 2=>, 3=<=, 4=<, 5===, 6~9=between(zdy1 opt1 x opt2 zdy2)
    /// </summary>
    private bool CompareNumeric(string formValueStr, string targetValue, int? optType,
        string? zdy2, string? opt1, string? opt2)
    {
        if (string.IsNullOrEmpty(formValueStr) || string.IsNullOrEmpty(targetValue))
        {
            return false;
        }
        double formVal = double.Parse(formValueStr);
        double target = double.Parse(targetValue);

        if (optType == null) return formVal == target;

        switch (optType.Value)
        {
            case 1: return formVal >= target;
            case 2: return formVal > target;
            case 3: return formVal <= target;
            case 4: return formVal < target;
            case 5: return formVal == target;
            case 6:
            case 7:
            case 8:
            case 9:
                // Between: zdy1 opt1 x opt2 zdy2
                if (string.IsNullOrEmpty(zdy2)) return false;
                double target2 = double.Parse(zdy2);
                bool leftBound = "<".Equals(opt1) ? formVal > target : formVal >= target;
                bool rightBound = "<".Equals(opt2) ? formVal < target2 : formVal <= target2;
                return leftBound && rightBound;
            default:
                return formVal == target;
        }
    }

    // ===================== 多表单支持: 辅助方法 =====================

    /// <summary>
    /// 外部表单模式: 将 lfFieldsMulti 展平到 lfFields, 使既有的条件求值/表单取人逻辑无需改动
    /// </summary>
    private void FlattenLfFieldsMultiIfNeeded(UDLFApplyVo vo)
    {
        var multi = vo.LfFieldsMulti;
        if (multi != null && multi.Count > 0)
        {
            var flat = new Dictionary<string, object>();
            foreach (var formFields in multi.Values)
            {
                if (formFields != null)
                {
                    foreach (var kv in formFields)
                    {
                        flat[kv.Key] = kv.Value;
                    }
                }
            }
            vo.LfFields = flat;
        }
    }

    /// <summary>
    /// 从节点配置JSON中读取低代码表单配置(formHidden + fieldControls)
    /// </summary>
    private BpmnNodeLowCodeConfJson? GetLowCodeConfJson(long confId, string? elementId)
    {
        if (string.IsNullOrEmpty(elementId))
        {
            return null;
        }

        BpmnNode? node = _bpmnNodeService._repository
            .FirstOrDefault(a => a.ConfId == confId && a.NodeId == elementId && a.IsDel == 0);
        if (node == null || string.IsNullOrEmpty(node.NodeConfigJson))
        {
            return null;
        }

        BpmnNodeConfigJson? nodeConfig = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
        return nodeConfig?.LowCodeConf;
    }

    /// <summary>
    /// 将 LFMainField 列表转换为前端展示用的字段值Map
    /// 从 queryData 中抽取,供内联模式和外部模式共用
    /// </summary>
    private Dictionary<string, object> BuildFieldVoMap(
        List<LFMainField> lfMainFields,
        Dictionary<string, BpmnConfLfFormdataField> lfFormdataFieldMap,
        string formCode, long confId)
    {
        var fieldVoMap = new Dictionary<string, object>();
        var fieldName2SelfMap = lfMainFields
            .GroupBy(x => x.FieldId)
            .ToDictionary(g => g.Key, g => g.ToList());

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
                        if (LFControlTypeEnum.SELECT.Name == currentFieldProp.FieldName)
                        {
                            try
                            {
                                JsonNode? jsonNode = JsonNode.Parse(field.FieldValue);
                                if (jsonNode == null)
                                {
                                    actualValue = "";//select默认值为空字符串
                                }
                                else if (jsonNode is JsonArray jsonArray)
                                {
                                    actualValue = jsonArray.ToString();
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogWarning($"field value can not be parsed to number,fieldName:{fieldName},formCode:{formCode},confId:{confId}", e);
                                actualValue = field.FieldValue;
                            }
                        }
                        else
                        {
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

        return fieldVoMap;
    }

    /// <summary>
    /// Extracts form-related assignee ids from the submitted form data for all nodes
    /// whose nodeProperty is NODE_PROPERTY_FORM_RELATED (16).
    /// </summary>
    private void ProcessFormRelatedUserConf(BpmnConfVo bpmnConfVo, UDLFApplyVo vo)
    {
        if (bpmnConfVo == null)
        {
            return;
        }

        long confId = bpmnConfVo.Id;
        Dictionary<string, object> lfFields = vo.LfFields;
        int? extraFlags = bpmnConfVo.ExtraFlags;

        if (extraFlags != null && BpmnConfFlagsEnum.HasFlag(extraFlags, BpmnConfFlagsEnum.HAS_FORM_RELATED_ASSIGNEES))
        {
            List<BpmnNode> formRelatedNodes = _bpmnNodeService._repository
                .Find(a => a.ConfId == confId && a.NodeProperty == (int)NodePropertyEnum.NODE_PROPERTY_FORM_RELATED)
                .ToList();

            Dictionary<string, List<string>> node2formRelatedAssignees = new Dictionary<string, List<string>>();

            if (formRelatedNodes != null && formRelatedNodes.Count > 0)
            {
                foreach (BpmnNode node in formRelatedNodes)
                {
                    List<ApproverFormRelatedUserConf> formRelatedConfs = GetFormRelatedConfsFromNode(node);
                    foreach (ApproverFormRelatedUserConf formRelatedConf in formRelatedConfs)
                    {
                        string valueJson = formRelatedConf.ValueJson;
                        if (string.IsNullOrEmpty(valueJson))
                        {
                            throw new AFBizException("表单中选取人员配置的valueJson不能为空!");
                        }

                        List<BaseIdTranStruVo> formInfos = JsonSerializer.Deserialize<List<BaseIdTranStruVo>>(valueJson) ?? new List<BaseIdTranStruVo>();
                        List<string> formValues = new List<string>();

                        foreach (BaseIdTranStruVo formInfo in formInfos)
                        {
                            string formName = formInfo.Id;
                            if (formName == null || lfFields == null || !lfFields.TryGetValue(formName, out var formVal) || formVal == null)
                            {
                                continue;
                            }

                            if (formVal is System.Collections.IEnumerable iterable && !(formVal is string))
                            {
                                foreach (var bValue in iterable)
                                {
                                    formValues.Add(bValue?.ToString());
                                }
                            }
                            else
                            {
                                formValues.Add(formVal.ToString());
                            }
                        }

                        node2formRelatedAssignees[node.Id.ToString()] = formValues;
                    }
                }
            }

            if (node2formRelatedAssignees.Count == 0)
            {
                throw new AFBizException("migration error,please contact the author");
            }

            vo.Node2formRelatedAssignees = node2formRelatedAssignees;
        }
    }

    /// <summary>
    /// Extracts the FormRelatedUserConfList from a node's node config JSON.
    /// </summary>
    private List<ApproverFormRelatedUserConf> GetFormRelatedConfsFromNode(BpmnNode node)
    {
        if (string.IsNullOrEmpty(node.NodeConfigJson))
        {
            return new List<ApproverFormRelatedUserConf>();
        }

        BpmnNodeConfigJson nodeConfig = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
        if (nodeConfig?.ApproverConf?.FormRelatedUserConfList == null)
        {
            return new List<ApproverFormRelatedUserConf>();
        }

        return nodeConfig.ApproverConf.FormRelatedUserConfList;
    }

    private string? GetLfFormDataFromJson(long confId)
    {
        BpmnConf? bpmnConf = _bpmnConfService._repository.FirstOrDefault(a => a.Id == confId);
        BpmnConfConfigJson? confConfig = JsonConfUtil.ParseConfConfig(bpmnConf?.ConfConfigJson);
        return confConfig?.LowCodeFormConfig?.Formdata;
    }
}
