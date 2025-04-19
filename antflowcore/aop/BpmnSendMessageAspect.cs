using System.Reflection;
using antflowcore.adaptor.processoperation;
using antflowcore.entity;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.aop;

/// <summary>
/// 这里非通用的创建代理的方法,仅用于实现特定业务
/// </summary>
/// <typeparam name="T"></typeparam>
public class BpmnSendMessageAspect<T> : DispatchProxy
{
    private T _decorated;

    protected override object Invoke(MethodInfo targetMethod, object[] args)
    {
        BusinessDataVo businessDataVo = (BusinessDataVo)args[0];
        if (businessDataVo == null) {
            throw new AFBizException("入参为空，请检查你的参数！");
        }

        BpmnConfCommonService bpmnConfCommonService = ServiceProviderUtils.GetService<BpmnConfCommonService>();
        BpmnConf bpmnConf = bpmnConfCommonService.GetBpmnConfByFormCode(businessDataVo.FormCode);
        if (bpmnConf == null || bpmnConf.Id == null|| bpmnConf.Id<=0)
        {
            throw new AFBizException($"表单编号[{businessDataVo.FormCode}]未匹配到工作流配置，请检查入参或工作流相关配置");
        }
        if (bpmnConf.IsOutSideProcess == 1) {
            businessDataVo.IsOutSideAccessProc=true;
        }
        
        
        
        //query business party info
        long? businessPartyId = bpmnConf.BusinessPartyId;
        OutSideBpmBusinessParty outSideBpmBusinessParty=null;
        if(businessPartyId!=null&&businessPartyId>0){
            OutSideBpmBusinessPartyService outSideBpmBusinessPartyService = ServiceProviderUtils.GetService<OutSideBpmBusinessPartyService>();
            outSideBpmBusinessParty=outSideBpmBusinessPartyService.baseRepo.Where(a=>a.Id==businessPartyId).First();
            //set outside type
            businessDataVo.OutSideType=(outSideBpmBusinessParty.Type);
        }
        if(outSideBpmBusinessParty==null){
            outSideBpmBusinessParty=new OutSideBpmBusinessParty();
        }


        BpmnConfVo bpmnConfVo = bpmnConf.MapToVo();
        businessDataVo.BpmnConfVo=bpmnConfVo;
        
        //is form data is not null then set form data
        if (!string.IsNullOrEmpty(businessDataVo.FormData)) {
            bpmnConfVo.FormData=businessDataVo.FormData;
        }

        //set bpmn conf
        businessDataVo.BpmnCode=bpmnConf.BpmnCode;

        //set bpmn name
        businessDataVo.BpmnName=bpmnConf.BpmnName;
        //declare variable message vo
        BpmVariableMessageVo vo;

        //to check if it is a submit operation then execute aspect method first then assemble variable message vo
        if (businessDataVo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_SUBMIT)
        {
            //do execute aspect method
            DoMethod( bpmnConf,businessDataVo,outSideBpmBusinessParty,ProcessOperationEnum.BUTTON_TYPE_SUBMIT, targetMethod,args);
            return null;
        }

        BpmVariableMessageService bpmVariableMessageService = ServiceProviderUtils.GetService<BpmVariableMessageService>();
        //get bpmn variable message vo
        vo =bpmVariableMessageService.GetBpmVariableMessageVo(businessDataVo);
            
        //get process operation enum by operation type
        ProcessOperationEnum? processOperationEunm = ProcessOperationEnumExtensions.GetEnumByCode(businessDataVo.OperationType);
        if (processOperationEunm == null)
        {
            throw new AFBizException(
                $"can not get processOperationEunm by by providing operationtype:{businessDataVo.OperationType}");
        }
        //do execute aspect method
        DoMethod( bpmnConf,businessDataVo,outSideBpmBusinessParty, processOperationEunm.Value,targetMethod,args);
        return null;
    }
    
    /**
        *do aspect method
        *
        * @param joinPoint
        * @throws Throwable
        */
    private void DoMethod( BpmnConf bpmnConf, BusinessDataVo businessDataVo, OutSideBpmBusinessParty outSideBpmBusinessParty,
        ProcessOperationEnum processOperationEunm, MethodInfo targetMethod, object[] args)  {
        //if it is an outside process,then set call back related information
        if (bpmnConf.IsOutSideProcess == 1 && !businessDataVo.IsOutSideChecked) {
            businessDataVo.OutSideType=outSideBpmBusinessParty.Type;
            IAdaptorFactory adaptorFactory = ServiceProviderUtils.GetService<IAdaptorFactory>();
            IProcessOperationAdaptor processOperationAdaptor = adaptorFactory.GetProcessOperation(businessDataVo);

            if (processOperationAdaptor==null) {
                throw new AFBizException($"{processOperationEunm.GetDesc()}功能实现未匹配，方法执行失败！");
            }

            OutSideBpmCallbackUrlConfService outSideBpmCallbackUrlConfService = ServiceProviderUtils.GetService<OutSideBpmCallbackUrlConfService>();
            //query and set process flow callback url
            OutSideBpmCallbackUrlConf outSideBpmCallbackUrlConf = outSideBpmCallbackUrlConfService.GetOutSideBpmCallbackUrlConf(bpmnConf.BusinessPartyId??0);


            //set bpm flow callback url
            businessDataVo.BpmFlowCallbackUrl=(outSideBpmCallbackUrlConf.BpmFlowCallbackUrl);


            businessDataVo.IsOutSideChecked=true;
            processOperationAdaptor.DoProcessButton(businessDataVo);
            return ;
        }
        else {
            // do proceed
            var result = targetMethod.Invoke(_decorated, args);
        
            return;
        } 
    }
    public static T Create(T decorated)
    {
        object proxy = DispatchProxy.Create<T, BpmnSendMessageAspect<T>>();
        ((BpmnSendMessageAspect<T>)proxy)._decorated = decorated;
        return (T)proxy;
    }
}