using antflowcore.entity;
using AntFlowCore.Entity; 
using antflowcore.util;
using antflowcore.vo;
using AntOffice.Base.Util;
using FreeSql;
using FreeSql.Internal.Model;
using antflowcore.util.Extension; 

namespace antflowcore.service.repository;

public class BpmnConfService
{
    private readonly IFreeSql _freeSql;
    public IBaseRepository<BpmnConf> baseRepo{get;}

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
            .Where(a=>a.BpmnName.EndsWith(bpmnCodeParts))
            .Max(a=>a.BpmnCode);
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

        var expression = LinqExtensions.True<BpmnConf, OutSideBpmBusinessParty, DictData>(); 
        expression = expression.And((a, b, c) => a.IsDel == 0);
        expression = expression.WhereIf(vo.EffectiveStatus > 0, (a, b, c) => a.EffectiveStatus == vo.EffectiveStatus);
        expression = expression.WhereIf(vo.IsOutSideProcess.HasValue ,(a, b, c) => (!a.IsOutSideProcess.HasValue || a.IsOutSideProcess == vo.IsOutSideProcess));
        expression = expression.WhereIf(vo.IsLowCodeFlow.HasValue, (a, b, c) => a.IsLowCodeFlow == vo.IsLowCodeFlow);
        expression = expression.WhereIf(!string.IsNullOrEmpty(vo.Search), (a, b, c) 
            => a.BpmnName.Contains(vo.Search) || a.FormCode.Contains(vo.Search) || a.BpmnCode.Contains(vo.Search));
        expression = expression.WhereIf(!string.IsNullOrEmpty(vo.FormCode), (a, b, c) => a.FormCode.Trim() == vo.FormCode.Trim());
        expression = expression.WhereIf(!string.IsNullOrEmpty(vo.BusinessPartyMark), (a, b, c) => b.BusinessPartyMark.Trim() == vo.BusinessPartyMark.Trim());
   
        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        List<BpmnConfVo> bpmnConfVos = select.Where(expression)
            .Page(basePagingInfo)
            .ToList((a, b, c) => new BpmnConfVo()
        {
            Id = a.Id,
            BpmnCode = a.BpmnCode,
            FormCode = a.FormCode,
            FormCodeDisplayName =c.Label,
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
        BpmnConf bpmnConf = this.baseRepo.Where(a=>a.Id==id)
            .ToOne();
        if (bpmnConf == null)
        {
            throw new Exception($"Bpmn conf with id {id} not found");
        }

        BpmnConf alreadyEffectiveConf = this.baseRepo.Where(a=>a.FormCode==bpmnConf.FormCode&&a.EffectiveStatus==1).ToOne();
        if (alreadyEffectiveConf != null)
        {
            alreadyEffectiveConf.EffectiveStatus = 0;
            this.baseRepo.Update(alreadyEffectiveConf);
        }
        else
        {
            alreadyEffectiveConf=new BpmnConf();
        }

        BpmnConf confToEffective = new BpmnConf
        {
            Id = id,
            AppId = alreadyEffectiveConf.AppId??bpmnConf.AppId,
            FormCode = alreadyEffectiveConf.FormCode??bpmnConf.FormCode,
            BpmnType = alreadyEffectiveConf.BpmnType??bpmnConf.BpmnType,
            IsAll = GetIsAll(bpmnConf, alreadyEffectiveConf)
        };
        this._freeSql
            .Update<BpmnConf>()
            .Set(a => a.AppId, confToEffective.AppId)
            .Set(a => a.BpmnType, confToEffective.BpmnType)
            .Set(a => a.IsAll, confToEffective.IsAll)
            .Set(a => a.EffectiveStatus, 1)
            .Where(a=>a.Id==id)
            .ExecuteAffrows();
        BpmProcessNameService bpmProcessNameService = ServiceProviderUtils.GetService<BpmProcessNameService>();
        bpmProcessNameService.EditProcessName(bpmnConf);
    }
    private int GetIsAll(BpmnConf bpmnConf, BpmnConf prevBpmnConf) {
        if (bpmnConf.IsOutSideProcess!=null&&bpmnConf.IsOutSideProcess == 1) {
            return 1;
        } else {
            if (prevBpmnConf.IsAll!=null) {
                return prevBpmnConf.IsAll;
            }
        }
        return 0;
    }
}