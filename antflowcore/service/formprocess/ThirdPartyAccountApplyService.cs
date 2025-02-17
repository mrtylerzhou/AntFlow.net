using antflowcore.adaptor;
using antflowcore.constant.enus;
using antflowcore.entity;
using antflowcore.factory;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;
using AutoMapper;

namespace antflowcore.service.formprocess;
[AfFormServiceAnno(SvcName = "DSFZH_WMA",Desc = "三方账号申请")]
public class ThirdPartyAccountApplyService : AFBaseCurdRepositoryService<ThirdPartyAccountApply>,IFormOperationAdaptor<ThirdPartyAccountApplyVo>
{
    
    public ThirdPartyAccountApplyService(
        IFreeSql freeSql) : base(freeSql)
    {
       
    }

    public BpmnStartConditionsVo PreviewSetCondition(ThirdPartyAccountApplyVo vo)
    {
        String userId =  vo.StartUserId;
        return new BpmnStartConditionsVo
        {
            StartUserId = userId,
            StartUserName = vo.StartUserName,
            AccountType = vo.AccountType,
        };
    }

    public BpmnStartConditionsVo LaunchParameters(ThirdPartyAccountApplyVo vo)
    {
        String userId =  vo.StartUserId;
        return new BpmnStartConditionsVo
        {
            StartUserId = userId,
            AccountType = vo.AccountType,
        };
    }

    public void OnInitData(ThirdPartyAccountApplyVo vo)
    {
        
    }

    public void OnQueryData(ThirdPartyAccountApplyVo vo)
    {
        ThirdPartyAccountApply accountApply=baseRepo.Where(a=>a.Id==Convert.ToInt32(vo.BusinessId)).First();
        mapper.Map(accountApply,vo);
    }

    public void OnSubmitData(ThirdPartyAccountApplyVo vo)
    {
        ThirdPartyAccountApply thirdPartyAccountApply=new ThirdPartyAccountApply();
        mapper.Map(vo,thirdPartyAccountApply);
        baseRepo.Insert(thirdPartyAccountApply);
        vo.BusinessId=(thirdPartyAccountApply.Id.ToString());
        vo.ProcessTitle="第三方账号申请";
        vo.ProcessDigest=vo.Remark;
        vo.EntityName=(nameof(ThirdPartyAccountApply));
    }

    public void OnConsentData(ThirdPartyAccountApplyVo vo)
    {
        if (vo.OperationType==(int)ButtonTypeEnum.BUTTON_TYPE_RESUBMIT){
            ThirdPartyAccountApply thirdPartyAccountApply=new ThirdPartyAccountApply();
            mapper.Map(vo,thirdPartyAccountApply);
            int id = Convert.ToInt32(vo.BusinessId);
            thirdPartyAccountApply.Id=id;
            baseRepo.Update(thirdPartyAccountApply);
        }
    }

    public void OnBackToModifyData(ThirdPartyAccountApplyVo vo)
    {
       
    }

    public void OnCancellationData(ThirdPartyAccountApplyVo vo)
    {
       
    }

    public void OnFinishData(BusinessDataVo vo)
    {
       
    }
}