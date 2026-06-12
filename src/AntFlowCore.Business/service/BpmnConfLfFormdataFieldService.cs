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

    private Dictionary<string, BpmnConfLfFormdataField> QryFormDataFieldMapFromJson(long confId)
    {
        BpmnConf? bpmnConf = _bpmnConfRepository.FirstOrDefault(a => a.Id == confId);
        BpmnConfConfigJson? confConfig = JsonConfUtil.ParseConfConfig(bpmnConf?.ConfConfigJson);
        return BpmnConfConfigHolder.ToFieldMap(confId, 0, confConfig?.LowCodeFormConfig);
    }
}
