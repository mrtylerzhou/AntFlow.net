using AntFlowCore.Vo;

namespace antflowcore.evt;

 /// <summary>
    /// 工作流按钮操作处理接口
    /// </summary>
    public interface IWorkflowButtonOperationHandler
    {
        /// <summary>
        /// 流程提交
        /// </summary>
        void onSubmit(BusinessDataVo businessData);

        /// <summary>
        /// 重新提交
        /// </summary>
        void onResubmit(BusinessDataVo businessData);

        /// <summary>
        /// 同意
        /// </summary>
        void onAgree(BusinessDataVo businessData);

        /// <summary>
        /// 不同意
        /// </summary>
        void onDisAgree(BusinessDataVo businessData);

        /// <summary>
        /// 查看流程详情
        /// </summary>
        void onViewBusinessProcess(BusinessDataVo businessData);

        /// <summary>
        /// 作废
        /// </summary>
        void onAbandon(BusinessDataVo businessData);

        /// <summary>
        /// 承办
        /// </summary>
        void onUndertake(BusinessDataVo businessData);

        /// <summary>
        /// 变更处理人
        /// </summary>
        void onChangeAssignee(BusinessDataVo businessData);

        /// <summary>
        /// 终止
        /// </summary>
        void onStop(BusinessDataVo businessData);

        /// <summary>
        /// 转发
        /// </summary>
        void onForward(BusinessDataVo businessData);

        /// <summary>
        /// 退回修改
        /// </summary>
        void onBackToModify(BusinessDataVo businessData);

        /// <summary>
        /// 流程撤回
        /// </summary>
        void onProcessDrawBack(BusinessDataVo businessData);

        /// <summary>
        /// 加批
        /// </summary>
        void onJp(BusinessDataVo businessData);

        /// <summary>
        /// 转办
        /// </summary>
        void onZb(BusinessDataVo businessData);

        /// <summary>
        /// 自选审批人
        /// </summary>
        void onChooseAssignee(BusinessDataVo businessData);

        /// <summary>
        /// 退回任意节点
        /// </summary>
        void onBackToAnyNode(BusinessDataVo businessData);

        /// <summary>
        /// 减签
        /// </summary>
        void onRemoveAssignee(BusinessDataVo businessData);

        /// <summary>
        /// 加签
        /// </summary>
        void onAddAssignee(BusinessDataVo businessData);

        /// <summary>
        /// 变更未来节点审批人
        /// </summary>
        void onChangeFutureAssignee(BusinessDataVo businessData);

        /// <summary>
        /// 未来节点减签
        /// </summary>
        void onRemoveFutureAssignee(BusinessDataVo businessData);

        /// <summary>
        /// 未来节点加签
        /// </summary>
        void onAddFutureAssignee(BusinessDataVo businessData);

        /// <summary>
        /// 完成数据
        /// </summary>
        void onFinishData(BusinessDataVo vo);
    }