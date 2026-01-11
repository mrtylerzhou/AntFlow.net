using antflowcore.adaptor;
using antflowcore.adaptor.bpmnelementadp;
using antflowcore.adaptor.bpmnprocessnotice;
using antflowcore.adaptor.nodetypecondition;
using antflowcore.adaptor.nodetypecondition.judge;
using antflowcore.adaptor.personnel;
using antflowcore.adaptor.personnel.businesstableadp;
using antflowcore.adaptor.personnel.provider;
using antflowcore.adaptor.processoperation;
using antflowcore.adaptor.variable;
using antflowcore.aop;
using antflowcore.bpmn.listener;
using antflowcore.bpmn.service;
using antflowcore.dto;
using antflowcore.evt;
using antflowcore.factory;
using antflowcore.service;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.service.formprocess;
using antflowcore.service.interf;
using antflowcore.service.interf.repository;
using antflowcore.service.processor;
using antflowcore.service.processor.filter;
using antflowcore.service.processor.lowcodeflow;
using antflowcore.service.processor.personnel;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;

namespace antflowcore.conf.serviceregistration;

public static class ServiceRegistration
{
    public static void AntFlowServiceSetUp(this IServiceCollection services,IConfiguration configuration)
    {
       
        services.Configure<MailSettings>(
            configuration.GetSection("MailSettings"));
        
        services.AddSingleton<BpmnConfService>();
        services.AddSingleton<BpmnConfCommonService>();
        services.AddSingleton<BpmnConfBizService>();
        services.AddSingleton<BpmnConfNoticeTemplateService>();
        services.AddSingleton<BpmnNodeService>();
        services.AddSingleton<BpmnNodeToService>();
        services.AddSingleton<BpmnNodeSignUpConfService>();
        services.AddSingleton<BpmnTemplateService>();
        services.AddSingleton<BpmnApproveRemindService>();
        services.AddSingleton<BpmnNodePersonnelConfService>();
        services.AddSingleton<BpmnNodePersonnelEmplConfService>();
        services.AddSingleton<IBpmnEmployeeInfoProviderService, BpmnEmployeeInfoProviderService>();
        services.AddSingleton<IUserService,UserService>();
        services.AddSingleton<IRoleService,RoleService>();
        services.AddSingleton<DepartmentService>();
        services.AddSingleton<BpmnConfLfFormdataService>();
        services.AddSingleton<BpmnConfLfFormdataFieldService>();
        services.AddSingleton<IAntFlowOrderPreProcessor<BpmnConfVo>,LFFormDataPreProcessor>();
        services.AddSingleton<IAntFlowOrderPostProcessor<BpmnConfVo>, LFFieldControlPostProcessor>();
        services.AddSingleton<IAntFlowOrderPostProcessor<BusinessDataVo>, AntFlowButtonsOperationPostProcessor>();
        services.AddSingleton<BpmnNodeLfFormdataFieldControlService>();
        services.AddSingleton<LFFieldControlPostProcessor>();
        services.AddSingleton<ProcessApprovalService>();
        services.AddSingleton<IFormOperationAdaptor<ThirdPartyAccountApplyVo>, ThirdPartyAccountApplyService>();
        services.AddSingleton<IFormOperationAdaptor<UDLFApplyVo>,LowFlowApprovalService>();
        services.AddSingleton<LowFlowApprovalService>();
        services.AddSingleton<OutSideBpmAccessBusinessService>();
        services.AddSingleton<FormFactory>();
        services.AddSingleton<ThirdPartyAccountApplyService>();
        services.AddSingleton<ButtonOperationService>();
        services.AddSingleton<OutSideBpmBusinessPartyService>();
        services.AddSingleton<OutSideBpmCallbackUrlConfService>();
        services.AddSingleton<BpmVariableService>();
        services.AddSingleton<BpmVariableApproveRemindService>();
        services.AddSingleton<BpmVariableMessageService>();
        services.AddSingleton<BpmVariableViewPageButtonService>();
        services.AddSingleton<BpmVariableButtonService>();
        services.AddSingleton<BpmVariableSequenceFlowService>();
        services.AddSingleton<BpmVariableSignUpService>();
        services.AddSingleton<BpmBusinessProcessService>();
        services.AddSingleton<BpmProcessNameService>();
        services.AddSingleton<BpmProcessNameRelevancyService>();
        services.AddSingleton<BpmProcessAppApplicationService>();
        services.AddSingleton<BpmnNodeButtonConfService>();
        services.AddSingleton<InformationTemplateService>();
        services.AddSingleton<BpmnViewPageButtonService>();
        services.AddSingleton<IConditionService, ConditionService>();
        services.AddSingleton<ConditionFilterService>();
        services.AddSingleton<IBpmnStartFormat, BpmnStartFormatService>();
        services.AddSingleton<BpmnStartFormatFactory>();
        services.AddSingleton<IBpmnPersonnelFormat, BpmnPersonnelFormatService>();
        services.AddSingleton<AssigneeVoBuildUtils>();
        services.AddSingleton<OutSideBpmConditionsTemplateService>();
        services.AddSingleton<BpmVerifyInfoService>();
        services.AddSingleton<BpmProcessNodeSubmitService>();
        services.AddSingleton<BpmVariableSignUpPersonnelService>();
        services.AddSingleton<BpmFlowrunEntrustService>();
        services.AddSingleton<TaskMgmtService>();
        services.AddSingleton<ProcessBusinessContansService>();
        services.AddSingleton<BpmProcessForwardService>();
        services.AddSingleton<ThirdPartyCallBackService>();
        services.AddSingleton<BpmnNodeBusinessTableConfService>();
        services.AddSingleton<BpmnNodeHrbpConfService>();
        services.AddSingleton<BpmnNodeAssignLevelConfService>();
        services.AddSingleton<BpmnNodeLoopConfService>();
        services.AddSingleton<BpmnNodeCustomizeConfService>();
        services.AddSingleton<BpmnNodeOutSideAccessConfService>();
        services.AddSingleton<BpmnNodeRoleConfService>();
        services.AddSingleton<BpmnNodeRoleOutsideEmpConfService>();
        services.AddSingleton<BpmnNodeConditionsConfService>();
        services.AddSingleton<BpmnNodeConditionsParamConfService>();
        services.AddSingleton<BpmnInsertVariablesService>();
        services.AddSingleton<ITaskListener,BpmnTaskListener>();
        services.AddSingleton<IExecutionListener,BpmnExecutionListener>();
        services.AddSingleton<ProcessNodeJumpService>();
        services.AddSingleton<BpmnConfLFFormDataBizService>();
        services.AddSingleton<DicMainService>();
        services.AddSingleton<DicDataSerivce>();
        services.AddSingleton<DictService>();
        services.AddSingleton<TaskService>();
        services.AddSingleton<LFMainService>();
        services.AddSingleton<LFMainFieldService>();
        services.AddSingleton<BpmVerifyInfoBizService>();
        services.AddSingleton<ProcessConstantsService>();
        services.AddSingleton<ActivitiAdditionalInfoService>();
        services.AddSingleton<UserMessageService>();
        services.AddSingleton<ConfigFlowButtonContantService>();
        services.AddSingleton<UserEntrustService>();
        services.AddSingleton<BpmnNodeAssignLevelConfService>();
        
        services.AddSingleton<AbstractOrderedSignNodeAdp, BpmnLoopSignNodeAdp>();
        services.AddSingleton<AbstractOrderedSignNodeAdp, OutSideOrderedSignNodeAdp>();
        services.AddSingleton<AbstractOrderedSignNodeAdp, TestOrderedSignNodeAdp>();
        
        services.AddSingleton<CustomizePersonnelProvider>();
        services.AddSingleton<DefaultTemplateService>();
        services.AddSingleton<OutSideBpmApproveTemplateService>();
        services.AddSingleton<OutSideBpmAdminPersonnelService>();
        services.AddSingleton<OutSideBpmBaseService>();
        services.AddSingleton<BpmnNodeAdditionalSignConfService>();
        #region IBpmnPersonnelProviderService with different register ways
        
        services.AddSingleton<DirectLeaderPersonnelProvider>();
        services.AddSingleton<CustomizePersonnelProvider>();
        services.AddSingleton<HrbpPersonnelProvider>();
        services.AddSingleton<LevelPersonnelProvider>();
        services.AddSingleton<LoopPersonnelProvider>();
        services.AddSingleton<OutSidePersonnelProvider>();
        services.AddSingleton<RolePersonnelProvider>();
        services.AddSingleton<StartUserPersonnelProvider>();
        services.AddSingleton<UserPointedPersonnelProvider>();
        services.AddSingleton<BusinessTablePersonnelProvider>();
        
        
        services.AddSingleton<IBpmnPersonnelProviderService, DirectLeaderPersonnelProvider>();
        services.AddSingleton<IBpmnPersonnelProviderService, CustomizePersonnelProvider>();
        services.AddSingleton<IBpmnPersonnelProviderService, HrbpPersonnelProvider>();
        services.AddSingleton<IBpmnPersonnelProviderService, LevelPersonnelProvider>();
        services.AddSingleton<IBpmnPersonnelProviderService, LoopPersonnelProvider>();
        services.AddSingleton<IBpmnPersonnelProviderService, OutSidePersonnelProvider>();
        services.AddSingleton<IBpmnPersonnelProviderService, RolePersonnelProvider>();
        services.AddSingleton<IBpmnPersonnelProviderService, StartUserPersonnelProvider>();
        services.AddSingleton<IBpmnPersonnelProviderService, UserPointedPersonnelProvider>();
        services.AddSingleton<IBpmnPersonnelProviderService, BusinessTablePersonnelProvider>();
        services.AddSingleton<IBpmnProcessAdminProvider, ProcessAddminProvider>();
        #endregion
        
        
        
        services.AddSingleton<AbstractBpmnPersonnelAdaptor, CustomizablePersonnelAdaptor>();
        services.AddSingleton<AbstractBpmnPersonnelAdaptor, DirectLeaderPersonnelAdaptor>();
        services.AddSingleton<AbstractBpmnPersonnelAdaptor, HrbpPersonnelAdaptor>();
        services.AddSingleton<AbstractBpmnPersonnelAdaptor, LevelPersonnelAdaptor>();
        services.AddSingleton<AbstractBpmnPersonnelAdaptor, LoopPersonnelAdaptor>();
        services.AddSingleton<AbstractBpmnPersonnelAdaptor, OutSidePersonnelAdaptor>();
        services.AddSingleton<AbstractBpmnPersonnelAdaptor, RolePersonnelAdaptor>();
        services.AddSingleton<AbstractBpmnPersonnelAdaptor, StartUserPersonnelAdaptor>();
        services.AddSingleton<AbstractBpmnPersonnelAdaptor, UserPointedPersonnelAdaptor>();
        services.AddSingleton<AbstractBpmnPersonnelAdaptor, BusinessTablePersonnelAdaptor>();


        services.AddSingleton<IBpmnNodeConditionsAdaptor, BpmnNodeConditionsAccountTypeAdaptor>();
        services.AddSingleton<IBpmnNodeConditionsAdaptor, BpmnNodeConditionsEmptyAdaptor>();
        services.AddSingleton<IBpmnNodeConditionsAdaptor, BpmnNodeConditionsPurchaseTypeAdaptor>();
        services.AddSingleton<IBpmnNodeConditionsAdaptor, BpmnTemplateMarkAdaptor>();
        

        services.AddSingleton<IConditionJudge, ThirdAccountJudgeService>();
        services.AddSingleton<IConditionJudge, AskLeaveJudge>();
        services.AddSingleton<IConditionJudge, PurchaseTotalMoneyJudge>();
        services.AddSingleton<IConditionJudge, NumberOperatorJudgeService>();
        services.AddSingleton<IConditionJudge, BpmnTemplateMarkJudge>();
        services.AddSingleton<IConditionJudge, LFStringConditionJudge>();
        services.AddSingleton<IConditionJudge, LFNumberFormatJudge>();
        services.AddSingleton<IConditionJudge, LFDateConditionJudge>();
        services.AddSingleton<IConditionJudge, LFDateTimeConditionJudge>();
        services.AddSingleton<IConditionJudge, LFCollectionConditionJudge>();

        services.AddSingleton<IBpmnDeduplicationFormat, BpmnDeduplicationFormatService>();
        services.AddSingleton<IBpmnOptionalDuplicatesAdaptor, BpmnOptionalDuplicateService>();
        services.AddSingleton<IBpmnRemoveFormat, BpmnRemoveFormatService>();
        //services.AddSingleton<IBpmnRemoveFormat, BpmnRemoveCopyFormatService>();


        services.AddSingleton<BpmnRemoveConfFormatFactory>();


        services.AddSingleton<IAdaptorService,NodePropertyPersonnelAdaptor>();
        services.AddSingleton<IAdaptorService, NodePropertyBusinessTableAdaptor>();
        services.AddSingleton<IAdaptorService, NodePropertyDirectLeaderAdaptor>();
        services.AddSingleton<IAdaptorService, NodePropertyHrbpAdaptor>();
        services.AddSingleton<IAdaptorService, NodePropertyLevelAdaptor>();
        services.AddSingleton<IAdaptorService, NodePropertyLoopAdaptor>();
        services.AddSingleton<IAdaptorService, NodePropertyOutSideAccessAdaptor>();
        services.AddSingleton<IAdaptorService, NodePropertyRoleAdaptor>();
        services.AddSingleton<IAdaptorService, NodePropertyStartUserAdaptor>();
        services.AddSingleton<IAdaptorService, NodeTypeConditionsAdaptor>();
        services.AddSingleton<IAdaptorService, NodePropertyCustomizeAdaptor>();
        
        
        
        services.AddSingleton<IAdaptorService, BpmnElementBusinessTableAdaptor>();
        services.AddSingleton<IAdaptorService, BpmnElementCustomizeAdaptor>();
        services.AddSingleton<IAdaptorService, BpmnElementDirectLeaderAdaptor>();
        services.AddSingleton<IAdaptorService, BpmnElementHrbpAdaptor>();
        services.AddSingleton<IAdaptorService, BpmnElementLevelAdaptor>();
        services.AddSingleton<IAdaptorService, BpmnElementLoopAdaptor>();
        services.AddSingleton<IAdaptorService, BpmnElementOutSideAccessAdaptor>();
        services.AddSingleton<IAdaptorService, BpmnElementPersonnelAdaptor>();
        services.AddSingleton<IAdaptorService, BpmnElementRoleAdaptor>();
        services.AddSingleton<IAdaptorService, BpmnElementStartUserAdaptor>();

        services.AddSingleton<IBpmnAddFlowElementAdaptor, BpmnAddFlowElementSingleAdaptor>();
        services.AddSingleton<IBpmnAddFlowElementAdaptor, BpmnAddFlowElementLoopAdaptor>();
        services.AddSingleton<IBpmnAddFlowElementAdaptor, BpmnAddFlowElementMultOrSignAaptor>();
        services.AddSingleton<IBpmnAddFlowElementAdaptor, BpmnAddFlowElementSignUpSerialAdaptor>();
        services.AddSingleton<IBpmnAddFlowElementAdaptor, BpmnAddFlowElementMultSignAdaptor>();

        services.AddSingleton<BpmnNodeFormatService>();
        
        services.AddSingleton<BpmVariableMultiplayerPersonnelService>();
        services.AddSingleton<BpmVariableMultiplayerService>();
        services.AddSingleton<BpmVariableSingleService>();


        services.AddSingleton<BpmnInsertVariableSubsMultiplayerOrSignAdaptor>();
        services.AddSingleton<BpmnInsertVariableSubsMultiplayerSignAdaptor>();
        services.AddSingleton<BpmnInsertVariableSubsSingleAdaptor>();
        
        services.AddSingleton<IBpmnInsertVariableSubs, BpmnInsertVariableSubsMultiplayerOrSignAdaptor>();
        services.AddSingleton<IBpmnInsertVariableSubs, BpmnInsertVariableSubsMultiplayerSignAdaptor>();
        services.AddSingleton<IBpmnInsertVariableSubs, BpmnInsertVariableSubsSingleAdaptor>();

        
        
        services.AddSingleton<IProcessOperationAdaptor,EndProcessService>();
        services.AddSingleton<IProcessOperationAdaptor,SubmitProcessService>();
        services.AddSingleton<IProcessOperationAdaptor, ResubmitProcessService>();
        services.AddSingleton<IProcessOperationAdaptor, OutSideAccessSubmitProcessService>();
        services.AddSingleton<IProcessOperationAdaptor, ChangeAssigneeProcessService>();
        services.AddSingleton<IProcessOperationAdaptor,TransferAssigneeProcessService>();
        services.AddSingleton<IProcessOperationAdaptor, UndertakeProcessService>();
        services.AddSingleton<IProcessOperationAdaptor, BackToModifyService>();
        services.AddSingleton<IProcessOperationAdaptor, ProcessForwardService>();
        services.AddSingleton<IProcessOperationAdaptor, RemoveAssigneeProcessService>();
        services.AddSingleton<IProcessOperationAdaptor, AddAssigneeProcessService>();
        services.AddSingleton<IProcessOperationAdaptor, RemoveFutureAssigneeProcessService>();
        services.AddSingleton<IProcessOperationAdaptor, AddFutureAssigneeProcessService>();
        services.AddSingleton<IProcessOperationAdaptor, ChangeFutureAssigneeProcessService>();
        services.AddSingleton<IProcessOperationAdaptor, TaskRecoverProcessSerivce>();
        services.AddSingleton<IProcessOperationAdaptor, FastForwardProcessService>();
        

        services.AddSingleton<AFDeploymentService>();
        services.AddSingleton<AfTaskInstService>();
        services.AddSingleton<AFTaskService>();
        services.AddSingleton<AFExecutionService>();
        services.AddSingleton<RepositoryService>();
        services.AddSingleton<RuntimeService>();
        
        services.AddSingleton<BpmnCreateAndStartService>();
       
        services.AddSingleton<ICallbackAdaptor<CallbackReqVo,CallbackRespVo>,ProcBaseCallBackAdp>();
        services.AddSingleton<ICallbackAdaptor<CallbackReqVo,CallbackRespVo>,ProcSubmitCallbackAdp>();
        services.AddSingleton<ThirdPartyCallbackFactory>();
        services.AddSingleton<BpmVariableMessageListenerService>();
        services.AddSingleton<BpmProcessNoticeService>();
        services.AddSingleton<UserMessageStatusService>();
        services.AddSingleton<MessageService>();
        services.AddSingleton<MailUtils>();
        services.AddSingleton<BpmnViewPageButtonBizService>();
        services.AddSingleton<ProcessDeptBizService>();
        services.AddSingleton<ITenantIdHolder, MultiTenantIdHolder>();
        services.AddSingleton<IWorkflowButtonOperationHandler, AntFlowOperationListener>();
        services.AddSingleton<ActivitiBpmMsgTemplateService>();
        services.AddSingleton<BpmnConfNoticeTemplateDetailService>();

        services.AddSingleton<IProcessNoticeAdaptor, EmailSendAdaptor>();
        services.AddSingleton<IProcessNoticeAdaptor, AppPushAdaptor>();
        services.AddSingleton<IProcessNoticeAdaptor, SMSSendAdaptor>();
        services.AddSingleton<AfStaticLogUtil>();
        services.AddSingleton<BpmvariableBizService>();
        //=================================不可越过的三八线==============================
        IAdaptorFactory adaptorFactory = AdaptorFactoryProxy.GetProxyInstance();
        services.AddSingleton(adaptorFactory);
        services.AddHttpClient();
    }
}