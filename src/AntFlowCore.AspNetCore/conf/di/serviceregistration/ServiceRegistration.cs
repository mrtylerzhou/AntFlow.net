using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.factory;
using AntFlowCore.Abstraction.formatter;
using AntFlowCore.Abstraction.formatter.filter;
using AntFlowCore.Abstraction.formatter.personnel;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.processor;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Bpmn.adaptor;
using AntFlowCore.Bpmn.adaptor.bpmnelementadp;
using AntFlowCore.Bpmn.adaptor.bpmnnodeadp;
using AntFlowCore.Bpmn.adaptor.nodetypecondition;
using AntFlowCore.Bpmn.adaptor.nodetypecondition.judge;
using AntFlowCore.Bpmn.adaptor.personnel.loopsign;
using AntFlowCore.Bpmn.adaptor.personnel.provider;
using AntFlowCore.Bpmn.adaptor.personnel.provideradp;
using AntFlowCore.Bpmn.adaptor.personnel.provideradp.businesstableadp;
using AntFlowCore.Bpmn.adaptor.processoperation;
using AntFlowCore.Bpmn.adaptor.variable;
using AntFlowCore.Bpmn.aop;
using AntFlowCore.Bpmn.evt;
using AntFlowCore.Bpmn.listener;
using AntFlowCore.Bpmn.service;
using AntFlowCore.Bpmn.util;
using AntFlowCore.Common.util;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.adaptor.formoperation;
using AntFlowCore.Core.adaptor.processoperation;
using AntFlowCore.Core.dto;
using AntFlowCore.Core.factory;
using AntFlowCore.Core.interf;
using AntFlowCore.Core.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Engine.Engine.service.biz;
using AntFlowCore.Engine.factory;
using AntFlowCore.Engine.service;
using AntFlowCore.Engine.service.biz;
using AntFlowCore.Engine.service.formprocess;
using AntFlowCore.Engine.service.org_dept;
using AntFlowCore.Engine.service.processor;
using AntFlowCore.Engine.service.processor.lowcodeflow;
using AntFlowCore.Extensions.Extensions.adaptor.bpmnelementadp;
using AntFlowCore.Extensions.Extensions.adaptor.bpmnprocessnotice;
using AntFlowCore.Extensions.Extensions.adaptor.variable;
using AntFlowCore.Extensions.Extensions.service.processor.filter;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Persist.repository;
using AntFlowCore.Vo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AntFlowCore.AspNetCore.conf.di.serviceregistration;

public static class ServiceRegistration
{
    public static void AntFlowServiceSetUp(this IServiceCollection services,IConfiguration configuration)
    {
       
        services.Configure<MailSettings>(
            configuration.GetSection("MailSettings"));
        
        services.AddSingleton<BpmnConfService>();
        services.AddSingleton<IBpmnConfService, BpmnConfService>();
        services.AddSingleton<BpmnConfCommonService>();
        services.AddSingleton<IBpmnConfCommonService, BpmnConfCommonService>();
        services.AddSingleton<IBpmnConfBizService, BpmnConfBizService>();
        services.AddSingleton<BpmnConfBizService>();
        services.AddSingleton<BpmnConfNoticeTemplateService>();
        services.AddSingleton<IBpmnConfNoticeTemplateService, BpmnConfNoticeTemplateService>();
        services.AddSingleton<BpmnNodeService>();
        services.AddSingleton<IBpmnNodeService, BpmnNodeService>();
        services.AddSingleton<BpmnNodeToService>();
        services.AddSingleton<IBpmnNodeToService, BpmnNodeToService>();
        services.AddSingleton<BpmnNodeSignUpConfService>();
        services.AddSingleton<IBpmnNodeSignUpConfService, BpmnNodeSignUpConfService>();
        services.AddSingleton<BpmnTemplateService>();
        services.AddSingleton<IBpmnTemplateService, BpmnTemplateService>();
        services.AddSingleton<BpmnApproveRemindService>();
        services.AddSingleton<IBpmnApproveRemindService, BpmnApproveRemindService>();
        services.AddSingleton<BpmnNodePersonnelConfService>();
        services.AddSingleton<IBpmnNodePersonnelConfService, BpmnNodePersonnelConfService>();
        services.AddSingleton<BpmnNodePersonnelEmplConfService>();
        services.AddSingleton<IBpmnNodePersonnelEmplConfService, BpmnNodePersonnelEmplConfService>();
        services.AddSingleton<IBpmnEmployeeInfoProviderService, BpmnEmployeeInfoProviderService>();
        services.AddSingleton<IUserService,UserService>();
        services.AddSingleton<IRoleService,RoleService>();
        services.AddSingleton<DepartmentService>();
        services.AddSingleton<IDepartmentService, DepartmentService>();
        services.AddSingleton<BpmnConfLfFormdataService>();
        services.AddSingleton<IBpmnConfLfFormdataService, BpmnConfLfFormdataService>();
        services.AddSingleton<BpmnConfLfFormdataFieldService>();
        services.AddSingleton<IBpmnConfLfFormdataFieldService, BpmnConfLfFormdataFieldService>();
        services.AddSingleton<IAntFlowOrderPreProcessor<BpmnConfVo>,LFFormDataPreProcessor>();
        services.AddSingleton<IAntFlowOrderPostProcessor<BpmnConfVo>, LFFieldControlPostProcessor>();
        services.AddSingleton<IAntFlowOrderPostProcessor<BusinessDataVo>, AntFlowButtonsOperationPostProcessor>();
        services.AddSingleton<BpmnNodeLfFormdataFieldControlService>();
        services.AddSingleton<IBpmnNodeLfFormdataFieldControlService, BpmnNodeLfFormdataFieldControlService>();
        services.AddSingleton<LFFieldControlPostProcessor>();
        services.AddSingleton<ProcessApprovalService>();
        services.AddSingleton<IProcessApprovalService, ProcessApprovalService>();
        services.AddSingleton<IFormOperationAdaptor<ThirdPartyAccountApplyVo>, ThirdPartyAccountApplyFlowService>();
        services.AddSingleton<IFormOperationAdaptor<UDLFApplyVo>,LowFlowApprovalService>();
        services.AddSingleton<LowFlowApprovalService>();
        services.AddSingleton<OutSideBpmAccessBusinessService>();
        services.AddSingleton<IOutSideBpmAccessBusinessService, OutSideBpmAccessBusinessService>();
        services.AddSingleton<FormFactory>();
        services.AddSingleton<IFormFactory, FormFactory>();
        services.AddSingleton<ThirdPartyAccountApplyFlowService>();
        services.AddSingleton<ButtonOperationService>();
        services.AddSingleton<IButtonOperationService, ButtonOperationService>();
        services.AddSingleton<OutSideBpmBusinessPartyService>();
        services.AddSingleton<IOutSideBpmBusinessPartyService, OutSideBpmBusinessPartyService>();
        services.AddSingleton<OutSideBpmCallbackUrlConfService>();
        services.AddSingleton<IOutSideBpmCallbackUrlConfService, OutSideBpmCallbackUrlConfService>();
        services.AddSingleton<BpmVariableService>();
        services.AddSingleton<IBpmVariableService, BpmVariableService>();
        services.AddSingleton<BpmVariableApproveRemindService>();
        services.AddSingleton<IBpmVariableApproveRemindService, BpmVariableApproveRemindService>();
        services.AddSingleton<BpmVariableMessageService>();
        services.AddSingleton<IBpmVariableMessageService, BpmVariableMessageService>();
        services.AddSingleton<BpmVariableViewPageButtonService>();
        services.AddSingleton<IBpmVariableViewPageButtonService, BpmVariableViewPageButtonService>();
        services.AddSingleton<BpmVariableButtonService>();
        services.AddSingleton<IBpmVariableButtonService, BpmVariableButtonService>();
        services.AddSingleton<BpmVariableSequenceFlowService>();
        services.AddSingleton<IBpmVariableSequenceFlowService, BpmVariableSequenceFlowService>();
        services.AddSingleton<BpmVariableSignUpService>();
        services.AddSingleton<IBpmVariableSignUpService, BpmVariableSignUpService>();
        services.AddSingleton<BpmBusinessProcessService>();
        services.AddSingleton<IBpmBusinessProcessService, BpmBusinessProcessService>();
        services.AddSingleton<BpmProcessNameService>();
        services.AddSingleton<IBpmProcessNameService, BpmProcessNameService>();
        services.AddSingleton<BpmProcessNameRelevancyService>();
        services.AddSingleton<IBpmProcessNameRelevancyService, BpmProcessNameRelevancyService>();
        services.AddSingleton<BpmProcessAppApplicationService>();
        services.AddSingleton<IBpmProcessAppApplicationService, BpmProcessAppApplicationService>();
        services.AddSingleton<BpmnNodeButtonConfService>();
        services.AddSingleton<IBpmnNodeButtonConfService, BpmnNodeButtonConfService>();
        services.AddSingleton<InformationTemplateService>();
        services.AddSingleton<IInformationTemplateService, InformationTemplateService>();
        services.AddSingleton<BpmnViewPageButtonService>();
        services.AddSingleton<IBpmnViewPageButtonService, BpmnViewPageButtonService>();
        services.AddSingleton<IConditionService, ConditionService>();
        services.AddSingleton<ConditionFilterService>();
        services.AddSingleton<IBpmnStartFormat, BpmnStartFormatService>();
        services.AddSingleton<BpmnStartFormatFactory>();
        services.AddSingleton<IBpmnStartFormatFactory, BpmnStartFormatFactory>();
        services.AddSingleton<IBpmnPersonnelFormat, BpmnPersonnelFormatService>();
        services.AddSingleton<AssigneeVoBuildUtils>();
        services.AddSingleton<OutSideBpmConditionsTemplateService>();
        services.AddSingleton<IOutSideBpmConditionsTemplateService, OutSideBpmConditionsTemplateService>();
        services.AddSingleton<BpmVerifyInfoService>();
        services.AddSingleton<IBpmVerifyInfoService, BpmVerifyInfoService>();
        services.AddSingleton<BpmProcessNodeSubmitService>();
        services.AddSingleton<IBpmProcessNodeSubmitService, BpmProcessNodeSubmitService>();
        services.AddSingleton<BpmVariableSignUpPersonnelService>();
        services.AddSingleton<IBpmVariableSignUpPersonnelService, BpmVariableSignUpPersonnelService>();
        services.AddSingleton<BpmFlowrunEntrustService>();
        services.AddSingleton<IBpmFlowrunEntrustService, BpmFlowrunEntrustService>();
        services.AddSingleton<TaskMgmtService>();
        services.AddSingleton<ITaskMgmtService, TaskMgmtService>();
        services.AddSingleton<ProcessBusinessContansService>();
        services.AddSingleton<IProcessBusinessContansService, ProcessBusinessContansService>();
        services.AddSingleton<BpmProcessForwardService>();
        services.AddSingleton<IBpmProcessForwardService, BpmProcessForwardService>();
        services.AddSingleton<ThirdPartyCallBackService>();
        services.AddSingleton<IThirdPartyCallBackService, ThirdPartyCallBackService>();
        services.AddSingleton<BpmnNodeBusinessTableConfService>();
        services.AddSingleton<IBpmnNodeBusinessTableConfService, BpmnNodeBusinessTableConfService>();
        services.AddSingleton<BpmnNodeHrbpConfService>();
        services.AddSingleton<IBpmnNodeHrbpConfService, BpmnNodeHrbpConfService>();
        services.AddSingleton<BpmnNodeAssignLevelConfService>();
        services.AddSingleton<IBpmnNodeAssignLevelConfService, BpmnNodeAssignLevelConfService>();
        services.AddSingleton<BpmnNodeLoopConfService>();
        services.AddSingleton<IBpmnNodeLoopConfService, BpmnNodeLoopConfService>();
        services.AddSingleton<BpmnNodeCustomizeConfService>();
        services.AddSingleton<IBpmnNodeCustomizeConfService, BpmnNodeCustomizeConfService>();
        services.AddSingleton<BpmnNodeOutSideAccessConfService>();
        services.AddSingleton<IBpmnNodeOutSideAccessConfService, BpmnNodeOutSideAccessConfService>();
        services.AddSingleton<BpmnNodeRoleConfService>();
        services.AddSingleton<IBpmnNodeRoleConfService, BpmnNodeRoleConfService>();
        services.AddSingleton<BpmnNodeRoleOutsideEmpConfService>();
        services.AddSingleton<IBpmnNodeRoleOutsideEmpConfService, BpmnNodeRoleOutsideEmpConfService>();
        services.AddSingleton<BpmnNodeConditionsConfService>();
        services.AddSingleton<IBpmnNodeConditionsConfService, BpmnNodeConditionsConfService>();
        services.AddSingleton<BpmnNodeConditionsParamConfService>();
        services.AddSingleton<IBpmnNodeConditionsParamConfService, BpmnNodeConditionsParamConfService>();
        services.AddSingleton<BpmnInsertVariablesService>();
        services.AddSingleton<IBpmnInsertVariablesService, BpmnInsertVariablesService>();
        services.AddSingleton<ITaskListener,BpmnTaskListener>();
        services.AddSingleton<IExecutionListener,BpmnExecutionListener>();
        services.AddSingleton<ProcessNodeJumpService>();
        services.AddSingleton<IProcessNodeJumpService, ProcessNodeJumpService>();
        services.AddSingleton<BpmnConfLFFormDataBizService>();
        services.AddSingleton<IBpmnConfLFFormDataBizService, BpmnConfLFFormDataBizService>();
        services.AddSingleton<DicMainService>();
        services.AddSingleton<IDicMainService, DicMainService>();
        services.AddSingleton<DicDataSerivce>();
        services.AddSingleton<IDicDataSerivce, DicDataSerivce>();
        services.AddSingleton<DictService>();
        services.AddSingleton<IDictService, DictService>();
        services.AddSingleton<TaskService>();
        services.AddSingleton<ITaskService, TaskService>();
        services.AddSingleton<LFMainService>();
        services.AddSingleton<ILFMainService, LFMainService>();
        services.AddSingleton<LFMainFieldService>();
        services.AddSingleton<ILFMainFieldService, LFMainFieldService>();
        services.AddSingleton<BpmVerifyInfoBizService>();
        services.AddSingleton<IBpmVerifyInfoBizService, BpmVerifyInfoBizService>();
        services.AddSingleton<ProcessConstantsService>();
        services.AddSingleton<IProcessConstantsService, ProcessConstantsService>();
        services.AddSingleton<ActivitiAdditionalInfoService>();
        services.AddSingleton<IActivitiAdditionalInfoService, ActivitiAdditionalInfoService>();
        services.AddSingleton<UserMessageService>();
        services.AddSingleton<IUserMessageService, UserMessageService>();
        services.AddSingleton<ConfigFlowButtonContantService>();
        services.AddSingleton<IConfigFlowButtonContantService, ConfigFlowButtonContantService>();
        services.AddSingleton<UserEntrustService>();
        services.AddSingleton<IUserEntrustService, UserEntrustService>();
        services.AddSingleton<BpmnNodeAssignLevelConfService>();
        
        services.AddSingleton<AbstractOrderedSignNodeAdp, BpmnLoopSignNodeAdp>();
        services.AddSingleton<AbstractOrderedSignNodeAdp, OutSideOrderedSignNodeAdp>();
        services.AddSingleton<AbstractOrderedSignNodeAdp, TestOrderedSignNodeAdp>();
        
        services.AddSingleton<CustomizePersonnelProvider>();
        services.AddSingleton<DefaultTemplateService>();
        services.AddSingleton<IDefaultTemplateService, DefaultTemplateService>();
        services.AddSingleton<OutSideBpmApproveTemplateService>();
        services.AddSingleton<OutSideBpmAdminPersonnelService>();
        services.AddSingleton<IOutSideBpmAdminPersonnelService, OutSideBpmAdminPersonnelService>();
        services.AddSingleton<OutSideBpmBaseService>();
        services.AddSingleton<IOutSideBpmBaseService, OutSideBpmBaseService>();
        services.AddSingleton<BpmnNodeAdditionalSignConfService>();
        services.AddSingleton<IBpmnNodeAdditionalSignConfService, BpmnNodeAdditionalSignConfService>();
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
        services.AddSingleton<IBpmnRemoveConfFormatFactory, BpmnRemoveConfFormatFactory>();


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
        services.AddSingleton<IBpmnNodeFormatService, BpmnNodeFormatService>();
        
        services.AddSingleton<BpmVariableMultiplayerPersonnelService>();
        services.AddSingleton<IBpmVariableMultiplayerPersonnelService, BpmVariableMultiplayerPersonnelService>();
        services.AddSingleton<BpmVariableMultiplayerService>();
        services.AddSingleton<IBpmVariableMultiplayerService, BpmVariableMultiplayerService>();
        services.AddSingleton<BpmVariableSingleService>();
        services.AddSingleton<IBpmVariableSingleService, BpmVariableSingleService>();


        services.AddSingleton<BpmnInsertVariableSubsMultiplayerOrSignAdaptor>();
        services.AddSingleton<BpmnInsertVariableSubsMultiplayerSignAdaptor>();
        services.AddSingleton<BpmnInsertVariableSubsSingleAdaptor>();
        
        services.AddSingleton<IBpmnInsertVariableSubs, BpmnInsertVariableSubsMultiplayerOrSignAdaptor>();
        services.AddSingleton<IBpmnInsertVariableSubs, BpmnInsertVariableSubsMultiplayerSignAdaptor>();
        services.AddSingleton<IBpmnInsertVariableSubs, BpmnInsertVariableSubsSingleAdaptor>();

        
        
        services.AddSingleton<IProcessOperationAdaptor,EndProcessService>();
        services.AddSingleton<IProcessOperationAdaptorProxyFactory, ProcessOperationAdaptorProxyFactory>();
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
        services.AddSingleton<IProcessOperationAdaptor, RemoveCurrentNodeProcessService>();
        services.AddSingleton<IProcessOperationAdaptor, RemoveFutureNodeProcessService>();
        services.AddSingleton<IProcessOperationAdaptor, InsertNodeAfterCurrentOrFutureService>();
        services.AddSingleton<AddAssigneeProcessService>();
        services.AddSingleton<RemoveAssigneeProcessService>();

        services.AddSingleton<AFDeploymentService>();
        services.AddSingleton<IAFDeploymentService, AFDeploymentService>();
        services.AddSingleton<AfTaskInstService>();
        services.AddSingleton<IAfTaskInstService, AfTaskInstService>();
        services.AddSingleton<AFTaskService>();
        services.AddSingleton<IAFTaskService, AFTaskService>();
        services.AddSingleton<AFExecutionService>();
        services.AddSingleton<IAFExecutionService, AFExecutionService>();
        services.AddSingleton<RepositoryService>();
        services.AddSingleton<RuntimeService>();
        
        services.AddSingleton<BpmnCreateAndStartService>();
        services.AddSingleton<IBpmnCreateAndStartService, BpmnCreateAndStartService>();
        services.AddSingleton<IBpmProcessNodeSubmitBizService, BpmProcessNodeSubmitBizService>();
        services.AddSingleton<IBpmVariableMessageBizService, BpmVariableMessageBizService>();
       
        services.AddSingleton<ICallbackAdaptor<CallbackReqVo,CallbackRespVo>,ProcBaseCallBackAdp>();
        services.AddSingleton<ICallbackAdaptor<CallbackReqVo,CallbackRespVo>,ProcSubmitCallbackAdp>();
        services.AddSingleton<ThirdPartyCallbackFactory>();
        services.AddSingleton<BpmVariableMessageListenerService>();
        services.AddSingleton<IBpmVariableMessageListenerService, BpmVariableMessageListenerService>();
        services.AddSingleton<BpmProcessNoticeService>();
        services.AddSingleton<IBpmProcessNoticeService, BpmProcessNoticeService>();
        services.AddSingleton<UserMessageStatusService>();
        services.AddSingleton<IUserMessageStatusService, UserMessageStatusService>();
        services.AddSingleton<MessageService>();
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<MailUtils>();
        services.AddSingleton<BpmnViewPageButtonBizService>();
        services.AddSingleton<IBpmnViewPageButtonBizService, BpmnViewPageButtonBizService>();
        services.AddSingleton<ProcessDeptBizService>();
        services.AddSingleton<IProcessDeptBizService, ProcessDeptBizService>();
        services.AddSingleton<ITenantIdHolder, MultiTenantIdHolder>();
        services.AddSingleton<IWorkflowButtonOperationHandler, AntFlowOperationListener>();
        services.AddSingleton<ActivitiBpmMsgTemplateService>();
        services.AddSingleton<IActivitiBpmMsgTemplateService, ActivitiBpmMsgTemplateService>();
        services.AddSingleton<BpmnConfNoticeTemplateDetailService>();

        services.AddSingleton<IProcessNoticeAdaptor, EmailSendAdaptor>();
        services.AddSingleton<IProcessNoticeAdaptor, AppPushAdaptor>();
        services.AddSingleton<IProcessNoticeAdaptor, SMSSendAdaptor>();
        services.AddSingleton<AfStaticLogUtil>();
        services.AddSingleton<BpmvariableBizService>();
        services.AddSingleton<IBpmvariableBizService, BpmvariableBizService>();
        services.AddSingleton<ThirdPartyAccountApplyService>();
        services.AddSingleton<IThirdPartyAccountApplyService, ThirdPartyAccountApplyService>();
        //=================================不可越过的三八线==============================
        IAdaptorFactory adaptorFactory = AdaptorFactoryProxy.GetProxyInstance();
        services.AddSingleton(adaptorFactory);
        services.AddHttpClient();
    }
}