using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class BpmnConfLfFormdataFieldService : IBpmnConfLfFormdataFieldService
{
    private readonly IBpmnConfRepository _bpmnConfRepository;

    public BpmnConfLfFormdataFieldService(
        IBpmnConfLfFormdataFieldRepository repository,
        IBpmnConfRepository bpmnConfRepository)
    {
        _repository = repository;
        _bpmnConfRepository = bpmnConfRepository;
    }

    public IBpmnConfLfFormdataFieldRepository _repository { get; }

    public Dictionary<string, BpmnConfLfFormdataField> QryFormDataFieldMap(long confId)
    {
        Dictionary<string, BpmnConfLfFormdataField> jsonFieldMap = QryFormDataFieldMapFromJson(confId);
        if (jsonFieldMap.Any())
        {
            return jsonFieldMap;
        }

        List<BpmnConfLfFormdataField> allFields = _repository.Find(x => x.BpmnConfId == confId);
        if (allFields == null || !allFields.Any())
        {
            throw new AFBizException("lowcode form data has no fields");
        }

        var id2SelfMap = new Dictionary<string, BpmnConfLfFormdataField>();
        foreach (var field in allFields)
        {
            id2SelfMap[field.FieldId] = field;
        }

        return id2SelfMap;
    }

    /// <summary>
    /// 按表单版本ID(formdataId)查询字段配置Map
    /// 用于外部表单模式,每个表单版本有独立的字段配置
    /// </summary>
    public Dictionary<string, BpmnConfLfFormdataField> QryFieldMapByFormdataId(long formdataId)
    {
        List<BpmnConfLfFormdataField> allFields = _repository.Find(x => x.FormDataId == formdataId);
        if (allFields == null || !allFields.Any())
        {
            throw new AFBizException($"lowcode form data has no fields by formdataId:{formdataId}");
        }

        var id2SelfMap = new Dictionary<string, BpmnConfLfFormdataField>();
        foreach (var field in allFields)
        {
            id2SelfMap[field.FieldId] = field;
        }

        return id2SelfMap;
    }

    private Dictionary<string, BpmnConfLfFormdataField> QryFormDataFieldMapFromJson(long confId)
    {
        BpmnConf? bpmnConf = _bpmnConfRepository.FirstOrDefault(a => a.Id == confId);
        BpmnConfConfigJson? confConfig = JsonConfUtil.ParseConfConfig(bpmnConf?.ConfConfigJson);
        return BpmnConfConfigHolder.ToFieldMap(confId, 0, confConfig?.LowCodeFormConfig);
    }
}
