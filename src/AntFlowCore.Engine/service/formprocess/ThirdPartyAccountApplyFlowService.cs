using AntFlowCore.Bpmn.adaptor.formoperation;
using AntFlowCore.Common.util;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.factory;
using AntFlowCore.Core.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Extensions.Extensions.adaptor;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;

namespace AntFlowCore.Engine.service.formprocess;
[DIYFormServiceAnno(SvcName = "DSFZH_WMA",Desc = "三方账号申请")]
public class ThirdPartyAccountApplyFlowService : AbstractLowFlowSpyFormOperationAdaptor<ThirdPartyAccountApplyVo>
{
    private readonly IThirdPartyAccountApplyService _thirdPartyAccountApplyService;

    public ThirdPartyAccountApplyFlowService(IThirdPartyAccountApplyService thirdPartyAccountApplyService,
        IBpmnNodeConditionsConfService bpmnNodeConditionsConfService) : base(bpmnNodeConditionsConfService)
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