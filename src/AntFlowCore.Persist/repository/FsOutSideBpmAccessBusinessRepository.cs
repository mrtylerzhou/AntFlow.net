using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsOutSideBpmAccessBusinessRepository : RepositoryBase<OutSideBpmAccessBusiness>, IOutSideBpmAccessBusinessRepository
{
    public FsOutSideBpmAccessBusinessRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public List<BpmnConfVo> SelectOutSideFormCodePageList()
    {
        return _ormContext.FreeSql
            .Select<BpmnConf, BpmProcessAppApplication>()
            .InnerJoin((a, b) => a.FormCode == b.ProcessKey)
            .Where((a, b) => a.EffectiveStatus == 1 && a.IsOutSideProcess == 1)
            .OrderByDescending((a, b) => a.CreateTime)
            .ToList<BpmnConfVo>((a, b) => new BpmnConfVo()
            {
                BpmnCode = a.BpmnCode,
                FormCode = a.FormCode,
                BpmnName = a.BpmnName,
                DeduplicationType = a.DeduplicationType,
                EffectiveStatus = a.EffectiveStatus,
                BusinessPartyId = a.BusinessPartyId,
                ApplicationId = b.Id,
                UpdateTime = a.UpdateTime,
                Remark = a.Remark
            });
    }
}
