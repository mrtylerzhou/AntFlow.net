using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repo;

public class FsBpmnConfLfFormdataRepository : RepositoryBase<BpmnConfLfFormdata>, IBpmnConfLfFormdataRepository
{
    public FsBpmnConfLfFormdataRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public BpmnConfLfFormdata GetLFFormDataByFormCode(string formCode)
    {
        return _ormContext.FreeSql
            .Select<BpmnConfLfFormdata, BpmnConf>()
            .InnerJoin((a, b) => a.BpmnConfId == b.Id && b.EffectiveStatus == 1)
            .Where(m => m.t2.FormCode == formCode)
            .First();
    }
}
