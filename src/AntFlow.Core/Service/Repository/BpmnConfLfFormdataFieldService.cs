using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Processor.LowCodeFlow;

namespace AntFlow.Core.Service.Repository;

public class BpmnConfLfFormdataFieldService : AFBaseCurdRepositoryService<BpmnConfLfFormdataField>,
    IBpmnConfLfFormdataFieldService
{
    public BpmnConfLfFormdataFieldService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public Dictionary<string, BpmnConfLfFormdataField> QryFormDataFieldMap(long confId)
    {
        List<BpmnConfLfFormdataField> allFields = baseRepo.Where(x => x.BpmnConfId == confId).ToList();
        if (allFields == null || !allFields.Any())
        {
            throw new AFBizException("lowcode form data has no fields");
        }

        Dictionary<string, BpmnConfLfFormdataField>? id2SelfMap = new();
        foreach (BpmnConfLfFormdataField? field in allFields)
        {
            id2SelfMap[field.FieldId] = field;
        }

        return id2SelfMap;
    }
}