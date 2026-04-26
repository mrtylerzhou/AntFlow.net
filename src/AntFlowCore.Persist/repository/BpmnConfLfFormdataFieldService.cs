using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class BpmnConfLfFormdataFieldService : IBpmnConfLfFormdataFieldService
{
    public BpmnConfLfFormdataFieldService(IBpmnConfLfFormdataFieldRepository repository)
    {
        _repository = repository;
    }

    public IBpmnConfLfFormdataFieldRepository _repository { get; }

    public Dictionary<string, BpmnConfLfFormdataField> QryFormDataFieldMap(long confId)
    {
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
}
