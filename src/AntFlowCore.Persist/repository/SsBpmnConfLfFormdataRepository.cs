using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnConfLfFormdataRepository : RepositoryBase<BpmnConfLfFormdata>, IBpmnConfLfFormdataRepository
{
    public SsBpmnConfLfFormdataRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public BpmnConfLfFormdata GetLFFormDataByFormCode(string formCode)
    {
        return Db.Queryable<BpmnConfLfFormdata>()
            .InnerJoin<BpmnConf>((a, b) => a.BpmnConfId == b.Id && b.EffectiveStatus == 1)
            .Where((a, b) => b.FormCode == formCode)
            .Select((a, b) => a)
            .First();
    }
}
