using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnConfLfFormdataFieldRepository : RepositoryBase<BpmnConfLfFormdataField>, IBpmnConfLfFormdataFieldRepository
{
    public SsBpmnConfLfFormdataFieldRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public int UpdateIsConditionField(long confId, string fieldId, int isConditionField)
    {
        return Db.Updateable<BpmnConfLfFormdataField>()
            .SetColumns(a => a.IsConditionField == isConditionField)
            .Where(a => a.BpmnConfId == confId && a.FieldId == fieldId)
            .ExecuteCommand();
    }
}
