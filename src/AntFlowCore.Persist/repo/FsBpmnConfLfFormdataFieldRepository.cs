using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnConfLfFormdataFieldRepository : RepositoryBase<BpmnConfLfFormdataField>, IBpmnConfLfFormdataFieldRepository
{
    public FsBpmnConfLfFormdataFieldRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public int UpdateIsConditionField(long confId, string fieldId, int isConditionField)
    {
        return _ormContext.FreeSql.Update<BpmnConfLfFormdataField>()
            .Set(a => a.IsConditionField, isConditionField)
            .Where(a => a.BpmnConfId == confId && a.FieldId == fieldId)
            .ExecuteAffrows();
    }
}
