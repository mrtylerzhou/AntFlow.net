using System.Reflection;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Business.service;

/// <summary>
/// 流程表单字段变更审计.
/// 对齐 Java 版 jimuoffice 的 ProcessAuditBizServiceImpl:
/// - 在 OnConsentData 写入新值之前捕获旧值, 写入 t_bpm_process_audit;
/// - 低代码流程遍历 LfFields / LfFieldsMulti; DIY 流程反射遍历 vo 子类自己声明的属性;
/// - 所有字段都记录(即使未变化), 不做 diff 过滤;
/// - 低代码字段 label 从 t_bpmn_conf_lf_formdata_field 查(fieldId -> fieldName).
/// </summary>
public class BpmProcessAuditService : IBpmProcessAuditService
{
    private readonly IFormFactory _formFactory;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmnConfLfFormdataFieldService _lfFormdataFieldService;
    private readonly IBpmnConfRepository _bpmnConfRepository;
    private readonly ILogger<BpmProcessAuditService> _logger;

    public BpmProcessAuditService(
        IFormFactory formFactory,
        IBpmBusinessProcessService bpmBusinessProcessService,
        IBpmnConfLfFormdataFieldService lfFormdataFieldService,
        IBpmnConfRepository bpmnConfRepository,
        IBpmProcessAuditRepository repository,
        ILogger<BpmProcessAuditService> logger)
    {
        _formFactory = formFactory;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _lfFormdataFieldService = lfFormdataFieldService;
        _bpmnConfRepository = bpmnConfRepository;
        _repository = repository;
        _logger = logger;
    }

    public IBpmProcessAuditRepository _repository { get; }

    public void SaveChanges(BusinessDataVo vo, BpmAfTask task)
    {
        if (vo == null || task == null)
        {
            return;
        }
        string processNumber = vo.ProcessNumber;
        if (string.IsNullOrEmpty(processNumber))
        {
            return;
        }

        try
        {
            string createUser = SecurityUtils.GetLogInEmpIdStr();
            string createUserName = SecurityUtils.GetLogInEmpNameSafe();
            string tenantId = MultiTenantUtil.GetCurrentTenantId();
            string formCode = vo.FormCode;
            var audits = new List<BpmProcessAudit>();
            IFormOperationAdaptor<BusinessDataVo> formAdaptor = _formFactory.GetFormAdaptor(vo);

            if (vo.IsLowCodeFlow == 1)
            {
                // 1) 快照前端提交值 (OnQueryData 会把 vo 改写成数据库旧值, 先把新值留住)
                Dictionary<string, object> currentLfSnapshot = vo.LfFields == null
                    ? null : new Dictionary<string, object>(vo.LfFields);
                UDLFApplyVo udlf = vo as UDLFApplyVo;
                Dictionary<string, Dictionary<string, object>> currentMultiSnapshot = null;
                if (udlf?.LfFieldsMulti != null)
                {
                    currentMultiSnapshot = new Dictionary<string, Dictionary<string, object>>();
                    foreach (var e in udlf.LfFieldsMulti)
                    {
                        currentMultiSnapshot[e.Key] = e.Value == null ? null : new Dictionary<string, object>(e.Value);
                    }
                }

                // 2) 准备 fieldId -> label 映射 (低代码字段定义里查)
                Dictionary<string, string> inlineLabelMap = LoadInlineLabelMap(vo);
                var externalLabelMap = new Dictionary<string, Dictionary<string, string>>();
                if (currentMultiSnapshot != null)
                {
                    foreach (string fdIdStr in currentMultiSnapshot.Keys)
                    {
                        try
                        {
                            long fdId = long.Parse(fdIdStr);
                            Dictionary<string, BpmnConfLfFormdataField> fieldMap = _lfFormdataFieldService.QryFieldMapByFormdataId(fdId);
                            var labels = new Dictionary<string, string>();
                            foreach (var e in fieldMap)
                            {
                                labels[e.Key] = e.Value?.FieldName;
                            }
                            externalLabelMap[fdIdStr] = labels;
                        }
                        catch
                        {
                            // 忽略单个 formdataId 查询失败, 后续降级用 fieldName
                        }
                    }
                }

                // 3) OnQueryData 把数据库旧值写到 vo
                formAdaptor.OnQueryData(vo);
                Dictionary<string, object> oldMap = vo.LfFields;
                UDLFApplyVo udlfAfterQuery = vo as UDLFApplyVo;
                Dictionary<string, Dictionary<string, object>> oldMultiMap = udlfAfterQuery?.LfFieldsMulti;

                // 4) 内联 lfFields: 合并所有 key, 每个字段都记
                var allKeys = new HashSet<string>(StringComparer.Ordinal);
                if (currentLfSnapshot != null)
                {
                    allKeys.UnionWith(currentLfSnapshot.Keys);
                }
                if (oldMap != null)
                {
                    allKeys.UnionWith(oldMap.Keys);
                }
                foreach (string key in allKeys)
                {
                    object newVal = currentLfSnapshot != null && currentLfSnapshot.TryGetValue(key, out var nv) ? nv : null;
                    object oldVal = oldMap != null && oldMap.TryGetValue(key, out var ov) ? ov : null;
                    string label = inlineLabelMap.TryGetValue(key, out var lb) ? lb : null;
                    audits.Add(BuildAudit(formCode, processNumber, key, label, oldVal, newVal, createUser, createUserName, tenantId));
                }

                // 5) 外部多表单: 按 formdataId 维度逐个字段记
                var fdIds = new HashSet<string>(StringComparer.Ordinal);
                if (currentMultiSnapshot != null)
                {
                    fdIds.UnionWith(currentMultiSnapshot.Keys);
                }
                if (oldMultiMap != null)
                {
                    fdIds.UnionWith(oldMultiMap.Keys);
                }
                foreach (string fdId in fdIds)
                {
                    Dictionary<string, object> newFields = currentMultiSnapshot != null && currentMultiSnapshot.TryGetValue(fdId, out var nf) ? nf : null;
                    Dictionary<string, object> oldFields = oldMultiMap != null && oldMultiMap.TryGetValue(fdId, out var of) ? of : null;
                    var subKeys = new HashSet<string>(StringComparer.Ordinal);
                    if (newFields != null)
                    {
                        subKeys.UnionWith(newFields.Keys);
                    }
                    if (oldFields != null)
                    {
                        subKeys.UnionWith(oldFields.Keys);
                    }
                    externalLabelMap.TryGetValue(fdId, out var subLabels);
                    foreach (string key in subKeys)
                    {
                        object newVal = newFields != null && newFields.TryGetValue(key, out var nv) ? nv : null;
                        object oldVal = oldFields != null && oldFields.TryGetValue(key, out var ov) ? ov : null;
                        string label = subLabels != null && subLabels.TryGetValue(key, out var lb) ? lb : null;
                        audits.Add(BuildAudit(formCode, processNumber, key, label, oldVal, newVal, createUser, createUserName, tenantId));
                    }
                }

                // 6) 恢复 vo 的前端新值, 不影响后续 OnConsentData 写入
                vo.LfFields = currentLfSnapshot;
                if (udlf != null)
                {
                    udlf.LfFieldsMulti = currentMultiSnapshot;
                }
            }
            else
            {
                // DIY 流程: 反射遍历 vo 子类自己声明的属性 (排除 BusinessDataVo 父类引擎属性)
                List<PropertyInfo> props = CollectDeclaredProperties(vo.GetType());
                // 1) 快照当前(前端提交)业务属性值
                Dictionary<string, object> currentSnapshot = SnapshotProperties(vo, props);
                // 2) OnQueryData 把数据库旧值回写到 vo
                formAdaptor.OnQueryData(vo);
                // 3) 取旧值快照
                Dictionary<string, object> oldSnapshot = SnapshotProperties(vo, props);
                foreach (PropertyInfo p in props)
                {
                    object newVal = currentSnapshot.TryGetValue(p.Name, out var nv) ? nv : null;
                    object oldVal = oldSnapshot.TryGetValue(p.Name, out var ov) ? ov : null;
                    // DIY 无 label 概念, fieldLabel 留空, 前端 fallback 用 fieldName
                    audits.Add(BuildAudit(formCode, processNumber, p.Name, null, oldVal, newVal, createUser, createUserName, tenantId));
                }
                // 4) 恢复 vo 的业务属性为前端提交值
                RestoreProperties(vo, currentSnapshot);
            }

            if (audits.Count > 0)
            {
                foreach (BpmProcessAudit a in audits)
                {
                    a.TaskDefKey = task.TaskDefKey;
                    a.TaskName = task.Name;
                    a.CreateTime = DateTime.Now;
                }
                _repository.AddRange(audits);
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "SaveChanges failed, processNumber={ProcessNumber}", processNumber);
        }
    }

    public List<BpmProcessAudit> GetProcessAudits(string processNumber)
    {
        if (string.IsNullOrEmpty(processNumber))
        {
            return new List<BpmProcessAudit>();
        }
        return _repository.GetQueryable()
            .Where(a => a.ProcessNumber == processNumber)
            .OrderBy(a => a.TaskDefKey)
            .ThenBy(a => a.CreateTime)
            .ToList();
    }

    /// <summary>
    /// 内联表单模式: 一次性查 confId 下所有字段的 fieldId -> label.
    /// confId 拿不到时退化按流程实例定位唯一 bpmn_conf:
    /// processNumber -> bpm_business_process.VERSION(bpmn_code) -> bpmn_conf(form_code + bpmn_code).
    /// </summary>
    private Dictionary<string, string> LoadInlineLabelMap(BusinessDataVo vo)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        try
        {
            long? confId = vo.BpmnConfVo?.Id;
            if (confId == null || confId <= 0)
            {
                string formCode = vo.FormCode;
                string processNumber = vo.ProcessNumber;
                if (!string.IsNullOrEmpty(formCode) && !string.IsNullOrEmpty(processNumber)
                    && !StringConstants.LOWFLOW_FORM_CODE.Equals(formCode))
                {
                    string bpmnCode = null;
                    try
                    {
                        BpmBusinessProcess bpm = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
                        bpmnCode = bpm?.Version;
                    }
                    catch
                    {
                        // 查不到流程实例, 放弃 label 查询
                    }
                    if (!string.IsNullOrEmpty(bpmnCode))
                    {
                        BpmnConf? bpmnConf = _bpmnConfRepository.FirstOrDefault(a => a.FormCode == formCode && a.BpmnCode == bpmnCode);
                        if (bpmnConf != null)
                        {
                            confId = bpmnConf.Id;
                        }
                    }
                }
            }
            if (confId == null || confId <= 0)
            {
                return result;
            }
            Dictionary<string, BpmnConfLfFormdataField> fieldMap = _lfFormdataFieldService.QryFormDataFieldMap(confId.Value);
            foreach (var e in fieldMap)
            {
                result[e.Key] = e.Value?.FieldName;
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "LoadInlineLabelMap failed, processNumber={ProcessNumber}",
                vo?.ProcessNumber ?? string.Empty);
        }
        return result;
    }

    /// <summary>
    /// 取 vo 子类自己声明的属性(逐级向上, 排除 BusinessDataVo / Object).
    /// </summary>
    private static List<PropertyInfo> CollectDeclaredProperties(Type type)
    {
        var list = new List<PropertyInfo>();
        Type? t = type;
        while (t != null && t != typeof(object) && t != typeof(BusinessDataVo))
        {
            list.AddRange(t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            t = t.BaseType;
        }
        return list;
    }

    private static Dictionary<string, object> SnapshotProperties(object obj, List<PropertyInfo> props)
    {
        var snapshot = new Dictionary<string, object>(StringComparer.Ordinal);
        if (obj == null)
        {
            return snapshot;
        }
        foreach (PropertyInfo p in props)
        {
            try
            {
                snapshot[p.Name] = p.GetValue(obj);
            }
            catch
            {
                // 忽略单个属性取值失败
            }
        }
        return snapshot;
    }

    private static void RestoreProperties(object obj, Dictionary<string, object> values)
    {
        if (obj == null || values == null)
        {
            return;
        }
        foreach (var e in values)
        {
            try
            {
                PropertyInfo? p = obj.GetType().GetProperty(e.Key);
                if (p != null && p.CanWrite)
                {
                    p.SetValue(obj, e.Value);
                }
            }
            catch
            {
                // 忽略单个属性写回失败
            }
        }
    }

    private static BpmProcessAudit BuildAudit(string formCode, string processNumber,
        string fieldName, string fieldLabel, object oldVal, object newVal,
        string createUser, string createUserName, string tenantId)
    {
        return new BpmProcessAudit
        {
            FormCode = formCode,
            ProcessNumber = processNumber,
            FieldName = fieldName,
            FieldLabel = fieldLabel,
            OldValue = ValueToString(oldVal),
            NewValue = ValueToString(newVal),
            CreateUser = createUser,
            CreateUserName = createUserName,
            TenantId = tenantId,
        };
    }

    /// <summary>
    /// 值转字符串存储.
    /// 字符串/数字/布尔/日期等基本类型直接 ToString(避免出现引号包裹);
    /// 对象/集合/数组才 JSON 序列化, 保证结构信息不丢.
    /// </summary>
    private static string ValueToString(object val)
    {
        if (val == null)
        {
            return null;
        }
        if (val is string or char or byte or sbyte or short or ushort or int or uint or long or ulong
            or float or double or decimal or bool or DateTime or DateTimeOffset or DateOnly or TimeOnly
            or Guid or TimeSpan)
        {
            return val.ToString();
        }
        try
        {
            return System.Text.Json.JsonSerializer.Serialize(val);
        }
        catch
        {
            return val.ToString();
        }
    }
}
