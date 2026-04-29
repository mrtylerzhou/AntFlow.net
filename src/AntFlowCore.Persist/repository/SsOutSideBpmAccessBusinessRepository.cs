using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsOutSideBpmAccessBusinessRepository : RepositoryBase<OutSideBpmAccessBusiness>, IOutSideBpmAccessBusinessRepository
{
    public SsOutSideBpmAccessBusinessRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public bool CheckDuplicateData(string name, long? id)
    {
        int count = Db.Queryable<OutSideBpmAccessBusiness>()
            .LeftJoin<OutSideBpmBusinessParty>((a, b) => a.BusinessPartyId == b.Id)
            .Where((a, b) => a.IsDel == 0 && b.Name == name)
            .WhereIF(id != null && id > 0, (a, b) => a.Id != id)
            .Count();
        return count > 0;
    }

    public void DeleteByBusinessPartyId(long businessPartyId)
    {
        Db.Deleteable<OutSideBpmAccessBusiness>()
            .Where(a => a.BusinessPartyId == businessPartyId)
            .ExecuteCommand();
    }

    public List<OutSideBpmAccessBusiness> GetListByPageNumberAndPageSize(int pageNumber, int pageSize)
    {
        return Db.Queryable<OutSideBpmAccessBusiness>()
            .Where(a => a.IsDel == 0)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public List<BpmnConfVo> SelectOutSideFormCodePageList()
    {
        return Db.Queryable<BpmnConf>()
            .InnerJoin<BpmProcessAppApplication>((a, b) => a.FormCode == b.ProcessKey)
            .Where((a, b) => a.EffectiveStatus == 1 && a.IsOutSideProcess == 1)
            .OrderByDescending((a, b) => a.CreateTime)
            .Select<BpmnConfVo>((a, b) => new BpmnConfVo()
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
            })
            .ToList();
    }
}
