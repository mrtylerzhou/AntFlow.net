using antflowcore.bpmn.listener;
using antflowcore.constant.enus;
using AntFlowCore.Enums;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.evt;

/// <summary>
    /// 此类是 antflow 动作事件监听器, 
    /// 如果想要统一处理流程流转正常流转事件, 
    /// 请使用 <see cref="BpmnTaskListener"/>
    /// </summary>
    public class AntFlowOperationListener : IWorkflowButtonOperationHandler
    {
        private readonly ThirdPartyCallBackService _thirdPartyCallBackService;
        private readonly BpmVariableMessageService _variableMessageBizService;
        private readonly BpmVariableMessageListenerService _bpmVariableMessageListenerService;

        public AntFlowOperationListener(
            ThirdPartyCallBackService thirdPartyCallBackService,
            BpmVariableMessageService variableMessageBizService,
            BpmVariableMessageListenerService bpmVariableMessageListenerService)
        {
            _thirdPartyCallBackService = thirdPartyCallBackService;
            _variableMessageBizService = variableMessageBizService;
            _bpmVariableMessageListenerService = bpmVariableMessageListenerService;
        }

        /// <summary>
        /// 流程提交
        /// </summary>
        public void onSubmit(BusinessDataVo businessData)
        {
            if (businessData.IsOutSideAccessProc == true)
            {
                _thirdPartyCallBackService.DoCallback(
                    CallbackTypeEnum.PROC_STARTED_CALL_BACK,
                    businessData.BpmnConfVo,
                    businessData.ProcessNumber,
                    businessData.BusinessId,
                    SecurityUtils.GetLogInEmpNameSafe()
                );
            }
        }

        /// <summary>
        /// 重新提交
        /// </summary>
        public void onResubmit(BusinessDataVo businessData) { }

        /// <summary>
        /// 同意
        /// </summary>
        public void onAgree(BusinessDataVo businessData)
        {
            if (businessData.IsOutSideAccessProc == true)
            {
                _thirdPartyCallBackService.DoCallback(
                    CallbackTypeEnum.PROC_COMMIT_CALL_BACK,
                    businessData.BpmnConfVo,
                    businessData.ProcessNumber,
                    businessData.BusinessId,
                    SecurityUtils.GetLogInEmpNameSafe()
                );
            }
        }

        /// <summary>
        /// 不同意
        /// </summary>
        public void onDisAgree(BusinessDataVo businessData) { }

        /// <summary>
        /// 查看流程详情
        /// </summary>
        public void onViewBusinessProcess(BusinessDataVo businessData) { }

        /// <summary>
        /// 作废
        /// </summary>
        public void onAbandon(BusinessDataVo businessData)
        {
            if (businessData.IsOutSideAccessProc == true)
            {
                _thirdPartyCallBackService.DoCallback(
                    CallbackTypeEnum.PROC_END_CALL_BACK,
                    businessData.BpmnConfVo,
                    businessData.ProcessNumber,
                    businessData.BusinessId,
                    SecurityUtils.GetLogInEmpNameSafe()
                );
            }
        }

        /// <summary>
        /// 承办
        /// </summary>
        public void onUndertake(BusinessDataVo businessData) { }

        /// <summary>
        /// 变更处理人
        /// </summary>
        public void onChangeAssignee(BusinessDataVo businessData) { }

        /// <summary>
        /// 终止
        /// </summary>
        public void onStop(BusinessDataVo businessData)
        {
            /*_bpmVariableMessageListenerService.SendProcessMessages(EventTypeEnum.PROCESS_CANCELLATION, businessData);*/
        }

        /// <summary>
        /// 转发
        /// </summary>
        public void onForward(BusinessDataVo businessData)
        {
            /*_bpmVariableMessageListenerService.SendProcessMessages(EventTypeEnum.PROCESS_FORWARD, businessData);*/
        }

        /// <summary>
        /// 退回修改
        /// </summary>
        public void onBackToModify(BusinessDataVo businessData)
        {
            /*_variableMessageBizService.SendTemplateMessages(businessData);*/
        }

        /// <summary>
        /// 加批
        /// </summary>
        public void onJp(BusinessDataVo businessData)
        {
            /*_variableMessageBizService.SendTemplateMessages(businessData);*/
        }

        /// <summary>
        /// 转办
        /// </summary>
        public void onZb(BusinessDataVo businessData) { }

        /// <summary>
        /// 自选审批人
        /// </summary>
        public void onChooseAssignee(BusinessDataVo businessData) { }

        /// <summary>
        /// 退回任意节点
        /// </summary>
        public void onBackToAnyNode(BusinessDataVo businessData) { }

        /// <summary>
        /// 减签
        /// </summary>
        public void onRemoveAssignee(BusinessDataVo businessData) { }

        /// <summary>
        /// 加签
        /// </summary>
        public void onAddAssignee(BusinessDataVo businessData) { }

        /// <summary>
        /// 变更未来节点审批人
        /// </summary>
        public void onChangeFutureAssignee(BusinessDataVo businessData) { }

        /// <summary>
        /// 未来节点减签
        /// </summary>
        public void onRemoveFutureAssignee(BusinessDataVo businessData) { }

        /// <summary>
        /// 未来节点加签
        /// </summary>
        public void onAddFutureAssignee(BusinessDataVo businessData) { }

        /// <summary>
        /// 流程撤回
        /// </summary>
        public void onProcessDrawBack(BusinessDataVo businessData) { }

        /// <summary>
        /// 注意: 外部 SaaS 工作流完成并不走这里
        /// </summary>
        public void onFinishData(BusinessDataVo vo)
        {
            // 注意: 流程完成通知消息并不在这里,实际上已经有了,请勿要在这里写
        }
    }