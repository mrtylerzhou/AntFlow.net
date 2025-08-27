using AntFlow.Core.Entity;
using AntFlow.Core.Util;
using AntFlow.Core.Util.Extension;
using AntFlow.Core.Vo;
using FreeSql;
using FreeSql.Internal.Model;
using System.Linq.Expressions;

namespace AntFlow.Core.Service.Repository;

public class BpmnConfService
{
    private readonly IFreeSql _freeSql;

    public BpmnConfService(IFreeSql freeSql)
    {
        _freeSql = freeSql;
        baseRepo = freeSql.GetRepository<BpmnConf>();
    }

    public IBaseRepository<BpmnConf> baseRepo { get; }

    public string GetMaxBpmnCode(string bpmnCodeParts)
    {
        //todo 根据流程名称部分查找最大的流程编码，用于生成新编码
        string maxBpmnCode = baseRepo
            .Select
            .Where(a => a.BpmnName.EndsWith(bpmnCodeParts))
            .Max(a => a.BpmnCode);
        return maxBpmnCode;
    }

    public string ReCheckBpmnCode(string bpmnCodeParts, string bpmnCode)
    {
        long count = baseRepo.Select.Where(a => a.BpmnCode.Equals(bpmnCode)).Count();

        if (count == 0)
        {
            return bpmnCode;
        }

        string reJoinedBpmnCode = StrUtils.JoinBpmnCode(bpmnCodeParts, bpmnCode);

        return ReCheckBpmnCode(bpmnCodeParts, reJoinedBpmnCode);
    }

    public List<BpmnConfVo> SelectPageList(Page<BpmnConfVo> page, BpmnConfVo vo)
    {
        ISelect<BpmnConf, OutSideBpmBusinessParty, DictData> select = _freeSql
            .Select<BpmnConf, OutSideBpmBusinessParty, DictData>()
            .LeftJoin((a, b, c) => a.BusinessPartyId == b.Id)
            .LeftJoin((a, b, c) => a.FormCode == c.Value && a.IsLowCodeFlow == 1);
        Expression<Func<BpmnConf, OutSideBpmBusinessParty, DictData, bool>>? expression =
            LinqExtensions.True<BpmnConf, OutSideBpmBusinessParty, DictData>();
        expression = expression.And((a, b, c) => a.IsDel == 0);
        expression = expression.WhereIf(vo.EffectiveStatus > 0, (a, b, c) => a.EffectiveStatus == vo.EffectiveStatus);
        expression = expression.WhereIf(vo.IsOutSideProcess == 1, (a, b, c) => a.IsOutSideProcess == 1);
        expression = expression.WhereIf(!vo.IsOutSideProcess.HasValue || vo.IsOutSideProcess == 0,
            (a, b, c) => a.IsOutSideProcess == null || a.IsOutSideProcess == 0);
        expression = expression.WhereIf(vo.IsLowCodeFlow.HasValue, (a, b, c) => a.IsLowCodeFlow == vo.IsLowCodeFlow);
        expression = expression.WhereIf(!string.IsNullOrEmpty(vo.Search), (a, b, c)
            => a.BpmnName.Contains(vo.Search) || a.FormCode.Contains(vo.Search) || a.BpmnCode.Contains(vo.Search));
        expression = expression.WhereIf(!string.IsNullOrEmpty(vo.FormCode),
            (a, b, c) => a.FormCode.Trim() == vo.FormCode.Trim());
        expression = expression.WhereIf(!string.IsNullOrEmpty(vo.BusinessPartyMark),
            (a, b, c) => b.BusinessPartyMark.Trim() == vo.BusinessPartyMark.Trim());

        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        List<BpmnConfVo> bpmnConfVos = select.Where(expression)
            .OrderByDescending(a => a.t1.CreateTime)
            .Page(basePagingInfo)
            .ToList((a, b, c) => new BpmnConfVo
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
                Remark = a.Remark
            });
        page.Total = (int)basePagingInfo.Count;
        return bpmnConfVos;
    }

    public void EffectiveBpmnConf(int id)
    {
        BpmnConf bpmnConf = baseRepo.Where(a => a.Id == id)
            .ToOne();
        if (bpmnConf == null)
        {
            throw new System.Exception($"Bpmn conf with id {id} not found");
        }

        BpmnConf alreadyEffectiveConf =
            baseRepo.Where(a => a.FormCode == bpmnConf.FormCode && a.EffectiveStatus == 1).ToOne();
        if (alreadyEffectiveConf != null)
        {
            alreadyEffectiveConf.EffectiveStatus = 0;
            baseRepo.Update(alreadyEffectiveConf);
        }
        else
        {
            alreadyEffectiveConf = new BpmnConf();
        }

        BpmnConf confToEffective = new()
        {
            Id = id,
            AppId = alreadyEffectiveConf.AppId ?? bpmnConf.AppId,
            FormCode = alreadyEffectiveConf.FormCode ?? bpmnConf.FormCode,
            BpmnType = alreadyEffectiveConf.BpmnType ?? bpmnConf.BpmnType,
            IsAll = GetIsAll(bpmnConf, alreadyEffectiveConf)
        };
        _freeSql
            .Update<BpmnConf>()
            .Set(a => a.AppId, confToEffective.AppId)
            .Set(a => a.BpmnType, confToEffective.BpmnType)
            .Set(a => a.IsAll, confToEffective.IsAll)
            .Set(a => a.EffectiveStatus, 1)
            .Where(a => a.Id == id)
            .ExecuteAffrows();
        BpmProcessNameService bpmProcessNameService = ServiceProviderUtils.GetService<BpmProcessNameService>();
        bpmProcessNameService.EditProcessName(bpmnConf);
    }

    private int GetIsAll(BpmnConf bpmnConf, BpmnConf prevBpmnConf)
    {
        if (bpmnConf.IsOutSideProcess != null && bpmnConf.IsOutSideProcess == 1)
        {
            return 1;
        }

        return prevBpmnConf.IsAll;
    }
}