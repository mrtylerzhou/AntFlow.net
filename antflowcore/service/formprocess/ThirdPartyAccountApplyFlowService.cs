using antflowcore.adaptor;
using antflowcore.constant.enus;
using antflowcore.entity;
using antflowcore.factory;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;


namespace antflowcore.service.formprocess;
[DIYFormServiceAnno(SvcName = "DSFZH_WMA",Desc = "三方账号申请")]
public class ThirdPartyAccountApplyFlowService : AbstractLowFlowSpyFormOperationAdaptor<ThirdPartyAccountApplyVo>
{
    private readonly ThirdPartyAccountApplyService _thirdPartyAccountApplyService;

    public ThirdPartyAccountApplyFlowService(ThirdPartyAccountApplyService thirdPartyAccountApplyService,
        BpmnNodeConditionsConfService bpmnNodeConditionsConfService) : base(bpmnNodeConditionsConfService)
    {
        _thirdPartyAccountApplyService = thirdPartyAccountApplyService;
    }


    public override void PreviewSetCondition(BpmnStartConditionsVo conditionsVo, ThirdPartyAccountApplyVo businessDataVo)
    {
       
    }
    

    public override void LaunchParameters(BpmnStartConditionsVo conditionsVo, ThirdPartyAccountApplyVo businessDataVo)
    {
       
    }
    
    

    public override void OnInitData(ThirdPartyAccountApplyVo vo)
    {
        
    }

    public override void OnQueryData(ThirdPartyAccountApplyVo vo)
    {
        ThirdPartyAccountApply accountApply=_thirdPartyAccountApplyService.baseRepo.Where(a=>a.Id==Convert.ToInt32(vo.BusinessId)).First();
        vo.AccountType = accountApply.AccountType;
        vo.AccountOwnerName = accountApply.AccountOwnerName;
        vo.Remark = accountApply.Remark;
    }

    public override void OnSubmitData(ThirdPartyAccountApplyVo vo)
    {
        ThirdPartyAccountApply thirdPartyAccountApply = vo.MapToEntity();
        
        _thirdPartyAccountApplyService.baseRepo.Insert(thirdPartyAccountApply);
        vo.BusinessId=(thirdPartyAccountApply.Id.ToString());
        vo.ProcessTitle="第三方账号申请";
        vo.ProcessDigest=vo.Remark;
        vo.EntityName=(nameof(ThirdPartyAccountApply));
    }

    public override void OnConsentData(ThirdPartyAccountApplyVo vo)
    {
        if (vo.OperationType==(int)ButtonTypeEnum.BUTTON_TYPE_RESUBMIT)
        {
            ThirdPartyAccountApply thirdPartyAccountApply = vo.MapToEntity();
            int id = Convert.ToInt32(vo.BusinessId);
            thirdPartyAccountApply.Id=id;
            _thirdPartyAccountApplyService.baseRepo.Update(thirdPartyAccountApply);
        }
    }

    public override void OnBackToModifyData(ThirdPartyAccountApplyVo vo)
    {
       
    }

    public override void OnCancellationData(ThirdPartyAccountApplyVo vo)
    {
       
    }

    public override void OnFinishData(BusinessDataVo vo)
    {
       
    }
}