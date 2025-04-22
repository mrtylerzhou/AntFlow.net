using antflowcore.entity;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Entity;
using FreeSql;
using System.Linq.Expressions;

namespace antflowcore.service.repository;

public class BpmnConfService
{
    private readonly IFreeSql _freeSql;
    public IBaseRepository<BpmnConf> baseRepo { get; }

    public BpmnConfService(IFreeSql freeSql)
    {
        _freeSql = freeSql;
        baseRepo = freeSql.GetRepository<BpmnConf>();
    }
    public string GetMaxBpmnCode(String bpmnCodeParts)
    {
        //todo 注意验证生成的语句是什么样子的,是否有性能问题
        string maxBpmnCode = baseRepo
            .Select
            .Where(a => a.BpmnName.EndsWith(bpmnCodeParts))
            .Max(a => a.BpmnCode);
        return maxBpmnCode;
    }

    public String ReCheckBpmnCode(String bpmnCodeParts, String bpmnCode)
    {
        long count = baseRepo.Select.Where(a => a.BpmnCode.Equals(bpmnCode)).Count();

        if (count == 0)
        {
            return bpmnCode;
        }

        String reJoinedBpmnCode = StrUtils.JoinBpmnCode(bpmnCodeParts, bpmnCode);

        return ReCheckBpmnCode(bpmnCodeParts, reJoinedBpmnCode);
    }

    public List<BpmnConfVo> SelectPageList(Page<BpmnConfVo> page, BpmnConfVo vo)
    {
        ISelect<BpmnConf, OutSideBpmBusinessParty, DictData> select = _freeSql
            .Select<BpmnConf, OutSideBpmBusinessParty, DictData>()
            .LeftJoin((a, b, c) => a.BusinessPartyId == b.Id)
            .LeftJoin((a, b, c) => a.FormCode == c.Value && a.IsLowCodeFlow == 1);
        Expression<Func<BpmnConf, OutSideBpmBusinessParty, DictData, bool>> expression = (a, b, c) => a.IsDel == 0;
        if (vo.IsOutSideProcess != null)
        {
            expression.And((a, b, c) => a.IsOutSideProcess == vo.IsOutSideProcess);
        }

        if (vo.IsLowCodeFlow != null)
        {
            expression.And((a, b, c) => a.IsLowCodeFlow == vo.IsLowCodeFlow);
        }

        if (!string.IsNullOrEmpty(vo.Search))
        {
            expression.And((a, b, c) => a.BpmnName.Contains(vo.Search));
        }

        if (!string.IsNullOrEmpty(vo.FormCode))
        {
            expression.And((a, b, c) => a.FormCode.Contains(vo.FormCode));
        }

        if (!string.IsNullOrEmpty(vo.BpmnCode))
        {
            expression.And((a, b, c) => a.BpmnCode.Contains(vo.BpmnCode));
        }

        if (!string.IsNullOrEmpty(vo.BusinessPartyMark))
        {
            expression.And((a, b, c) => b.BusinessPartyMark.Equals(vo.BusinessPartyMark));
        }
        expression.And((a, b, c) => a.EffectiveStatus == vo.EffectiveStatus);
        if (!string.IsNullOrEmpty(vo.FormCodeDisplayName))
        {
            expression.And((a, b, c) => c.Label.Contains(vo.FormCodeDisplayName));
        }

        List<BpmnConfVo> bpmnConfVos = select.Where(expression)
            .Page(page.Current, page.Size)
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
        return bpmnConfVos;
    }

    public void EffectiveBpmnConf(int id)
    {
        BpmnConf bpmnConf = this.baseRepo.Where(a => a.Id == id)
            .ToOne();
        if (bpmnConf == null)
        {
            throw new Exception($"Bpmn conf with id {id} not found");
        }

        BpmnConf alreadyEffectiveConf = this.baseRepo.Where(a => a.FormCode == bpmnConf.FormCode && a.EffectiveStatus == 1).ToOne();
        if (alreadyEffectiveConf != null)
        {
            alreadyEffectiveConf.EffectiveStatus = 0;
            this.baseRepo.Update(alreadyEffectiveConf);
        }
        else
        {
            alreadyEffectiveConf = new BpmnConf();
        }

        BpmnConf confToEffective = new BpmnConf
        {
            Id = id,
            AppId = alreadyEffectiveConf.AppId,
            FormCode = alreadyEffectiveConf.FormCode,
            BpmnType = alreadyEffectiveConf.BpmnType,
            IsAll = GetIsAll(bpmnConf, alreadyEffectiveConf)
        };
        this.baseRepo.Update(confToEffective);
        BpmProcessNameService bpmProcessNameService = ServiceProviderUtils.GetService<BpmProcessNameService>();
        bpmProcessNameService.EditProcessName(bpmnConf);
    }
    private int GetIsAll(BpmnConf bpmnConf, BpmnConf prevBpmnConf)
    {
        if (bpmnConf.IsOutSideProcess != null && bpmnConf.IsOutSideProcess == 1)
        {
            return 1;
        }
        else
        {
            if (prevBpmnConf.IsAll != null)
            {
                return prevBpmnConf.IsAll;
            }
        }
        return 0;
    }
}