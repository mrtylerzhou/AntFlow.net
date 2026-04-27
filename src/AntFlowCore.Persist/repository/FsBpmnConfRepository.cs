using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.extension;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql.Internal.Model;

namespace AntFlowCore.Persist.repository;

public class FsBpmnConfRepository : RepositoryBase<BpmnConf>, IBpmnConfRepository
{
    public FsBpmnConfRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public BpmnConf GetBpmnConfByFormCode(string formCode)
    {
        return _ormContext.FreeSql.Select<BpmnConf>()
            .Where(a => a.FormCode == formCode && a.EffectiveStatus == 1)
            .ToOne() ?? new BpmnConf();
    }

    public List<BpmnConf> GetBpmnConfByFormCodeBatch(List<string> formCodes)
    {
        return _ormContext.FreeSql.Select<BpmnConf>()
            .Where(a => formCodes.Contains(a.FormCode) && a.EffectiveStatus == 1)
            .ToList();
    }

    public string? GetMaxBpmnCode(string bpmnCodeParts)
    {
        return _ormContext.FreeSql.Select<BpmnConf>()
            .Where(a => a.BpmnName.EndsWith(bpmnCodeParts))
            .Max(a => a.BpmnCode);
    }

    public string ReCheckBpmnCode(string bpmnCodeParts, string bpmnCode)
    {
        long count = _ormContext.FreeSql.Select<BpmnConf>()
            .Where(a => a.BpmnCode.Equals(bpmnCode))
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
        var select = _ormContext.FreeSql
            .Select<BpmnConf, OutSideBpmBusinessParty, DictData>()
            .LeftJoin((a, b, c) => a.BusinessPartyId == b.Id)
            .LeftJoin((a, b, c) => a.FormCode == c.Value && a.IsLowCodeFlow == 1);
        var expression = AntFlowCore.Base.extension.LinqExtensions.True<BpmnConf, OutSideBpmBusinessParty, DictData>();
        expression = expression.And((a, b, c) => a.IsDel == 0);
        expression = expression.WhereIf(vo.EffectiveStatus > 0, (a, b, c) => a.EffectiveStatus == vo.EffectiveStatus);
        expression = expression.WhereIf(vo.IsOutSideProcess == 1, (a, b, c) => a.IsOutSideProcess == 1);
        expression = expression.WhereIf(!vo.IsOutSideProcess.HasValue || vo.IsOutSideProcess == 0, (a, b, c) => a.IsOutSideProcess == null || a.IsOutSideProcess == 0);
        expression = expression.WhereIf(vo.IsLowCodeFlow.HasValue, (a, b, c) => a.IsLowCodeFlow == vo.IsLowCodeFlow);
        expression = expression.WhereIf(!string.IsNullOrEmpty(vo.Search), (a, b, c)
            => a.BpmnName.Contains(vo.Search) || a.FormCode.Contains(vo.Search) || a.BpmnCode.Contains(vo.Search));
        expression = expression.WhereIf(!string.IsNullOrEmpty(vo.FormCode), (a, b, c) => a.FormCode.Trim() == vo.FormCode.Trim());
        expression = expression.WhereIf(!string.IsNullOrEmpty(vo.BusinessPartyMark), (a, b, c) => b.BusinessPartyMark.Trim() == vo.BusinessPartyMark.Trim());

       BasePagingInfo basePagingInfo = page.ToPagingInfo().ToBasePagingInfo();
        List<BpmnConfVo> bpmnConfVos = select.Where(expression)
            .OrderByDescending(a => a.t1.CreateTime)
            .Page(basePagingInfo)
            .ToList((a, b, c) => new BpmnConfVo()
            {
                Id = a.Id,
                BpmnCode = a.BpmnCode,
                FormCode = a.FormCode,
                FormCodeDisplayName = c.Label,
                DeduplicationType = a.DeduplicationType,
                EffectiveStatus = a.EffectiveStatus,
                BusinessPartyId = a.BusinessPartyId,
                UpdateTime = a.UpdateTime,
                IsOutSideProcess = a.IsOutSideProcess,
                IsLowCodeFlow = a.IsLowCodeFlow,
                Remark = a.Remark,
            });
        page.Total = (int)basePagingInfo.Count;
        return bpmnConfVos;
    }

    public void EffectiveBpmnConf(int id)
    {
        BpmnConf bpmnConf = _ormContext.FreeSql.Select<BpmnConf>()
            .Where(a => a.Id == id)
            .ToOne();
        if (bpmnConf == null)
        {
            throw new Exception($"Bpmn conf with id {id} not found");
        }

        BpmnConf alreadyEffectiveConf = _ormContext.FreeSql.Select<BpmnConf>()
            .Where(a => a.FormCode == bpmnConf.FormCode && a.EffectiveStatus == 1)
            .ToOne();
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

        _ormContext.FreeSql.Update<BpmnConf>()
            .Set(a => a.AppId, confToEffective.AppId)
            .Set(a => a.BpmnType, confToEffective.BpmnType)
            .Set(a => a.IsAll, confToEffective.IsAll)
            .Set(a => a.EffectiveStatus, 1)
            .Where(a => a.Id == id)
            .ExecuteAffrows();
    }
}
