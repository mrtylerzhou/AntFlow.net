using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.extension;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmnConfRepository : RepositoryBase<BpmnConf>, IBpmnConfRepository
{
    public SsBpmnConfRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public BpmnConf GetBpmnConfByFormCode(string formCode)
    {
        return Db.Queryable<BpmnConf>()
            .Where(a => a.FormCode == formCode && a.EffectiveStatus == 1)
            .First() ?? new BpmnConf();
    }

    public List<BpmnConf> GetBpmnConfByFormCodeBatch(List<string> formCodes)
    {
        return Db.Queryable<BpmnConf>()
            .Where(a => formCodes.Contains(a.FormCode) && a.EffectiveStatus == 1)
            .ToList();
    }

    public string? GetMaxBpmnCode(string bpmnCodeParts)
    {
        return Db.Queryable<BpmnConf>()
            .Where(a => a.BpmnName.EndsWith(bpmnCodeParts))
            .Max(a => a.BpmnCode);
    }

    public string ReCheckBpmnCode(string bpmnCodeParts, string bpmnCode)
    {
        long count = Db.Queryable<BpmnConf>()
            .Where(a => a.BpmnCode == bpmnCode)
            .Count();
        if (count == 0)
        {
            return bpmnCode;
        }
        string reJoinedBpmnCode = AntFlowCore.Base.util.StrUtils.JoinBpmnCode(bpmnCodeParts, bpmnCode);
        return ReCheckBpmnCode(bpmnCodeParts, reJoinedBpmnCode);
    }

    public List<BpmnConfVo> SelectPageList(Page<BpmnConfVo> page, BpmnConfVo vo)
    {
        var select = Db.Queryable<BpmnConf>()
            .LeftJoin<OutSideBpmBusinessParty>((a, b) => a.BusinessPartyId == b.Id)
            .LeftJoin<DictData>((a, b, c) => a.FormCode == c.Value && a.IsLowCodeFlow == 1);

        var expression = LinqExtensions.True<BpmnConf, OutSideBpmBusinessParty, DictData>();
        expression = expression.And((a, b, c) => a.IsDel == 0);
        expression = expression.WhereIf(vo.EffectiveStatus > 0, (a, b, c) => a.EffectiveStatus == vo.EffectiveStatus);
        expression = expression.WhereIf(vo.IsOutSideProcess == 1, (a, b, c) => a.IsOutSideProcess == 1);
        expression = expression.WhereIf(!vo.IsOutSideProcess.HasValue || vo.IsOutSideProcess == 0, (a, b, c) => a.IsOutSideProcess == null || a.IsOutSideProcess == 0);
        expression = expression.WhereIf(vo.IsLowCodeFlow.HasValue, (a, b, c) => a.IsLowCodeFlow == vo.IsLowCodeFlow);
        expression = expression.WhereIf(!string.IsNullOrEmpty(vo.Search), (a, b, c)
            => a.BpmnName.Contains(vo.Search) || a.FormCode.Contains(vo.Search) || a.BpmnCode.Contains(vo.Search));
        expression = expression.WhereIf(!string.IsNullOrEmpty(vo.FormCode), (a, b, c) => a.FormCode.Trim() == vo.FormCode.Trim());

        int totalCount = 0;
        var query = select.Where(expression);

        if (!string.IsNullOrEmpty(vo.BusinessPartyMark))
        {
            query = query.Where((a, b, c) => b.BusinessPartyMark.Trim() == vo.BusinessPartyMark.Trim());
        }

        List<BpmnConfVo> bpmnConfVos = query
            .OrderByDescending(a => a.CreateTime)
            .ToPageList(page.Current, page.Size, ref totalCount)
            .Select(a => new BpmnConfVo()
            {
                Id = a.Id,
                BpmnCode = a.BpmnCode,
                FormCode = a.FormCode,
                DeduplicationType = a.DeduplicationType,
                EffectiveStatus = a.EffectiveStatus,
                BusinessPartyId = a.BusinessPartyId,
                UpdateTime = a.UpdateTime,
                IsOutSideProcess = a.IsOutSideProcess,
                IsLowCodeFlow = a.IsLowCodeFlow,
                Remark = a.Remark,
            }).ToList();
        page.Total = totalCount;
        return bpmnConfVos;
    }

    public void EffectiveBpmnConf(int id)
    {
        BpmnConf bpmnConf = Db.Queryable<BpmnConf>()
            .Where(a => a.Id == id)
            .First();
        if (bpmnConf == null)
        {
            throw new Exception($"Bpmn conf with id {id} not found");
        }

        BpmnConf alreadyEffectiveConf = Db.Queryable<BpmnConf>()
            .Where(a => a.FormCode == bpmnConf.FormCode && a.EffectiveStatus == 1)
            .First();
        if (alreadyEffectiveConf != null)
        {
            alreadyEffectiveConf.EffectiveStatus = 0;
            Update(alreadyEffectiveConf);
        }
        else
        {
            alreadyEffectiveConf = new BpmnConf();
        }

        BpmnConf confToEffective = new BpmnConf
        {
            Id = id,
            AppId = alreadyEffectiveConf.AppId ?? bpmnConf.AppId,
            FormCode = alreadyEffectiveConf.FormCode ?? bpmnConf.FormCode,
            BpmnType = alreadyEffectiveConf.BpmnType ?? bpmnConf.BpmnType,
        };
        int isAll = 0;
        if (bpmnConf.IsOutSideProcess != null && bpmnConf.IsOutSideProcess == 1)
        {
            isAll = 1;
        }
        else
        {
            isAll = alreadyEffectiveConf.IsAll;
        }
        confToEffective.IsAll = isAll;

        Db.Updateable<BpmnConf>()
            .SetColumns(a => a.AppId == confToEffective.AppId)
            .SetColumns(a => a.BpmnType == confToEffective.BpmnType)
            .SetColumns(a => a.IsAll == confToEffective.IsAll)
            .SetColumns(a => a.EffectiveStatus == 1)
            .Where(a => a.Id == id)
            .ExecuteCommand();
    }
}
