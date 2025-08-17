using AntFlowCore.Enums;
using antflowcore.evt;
using AntFlowCore.Vo;

namespace antflowcore.service.processor.lowcodeflow;

  /// <summary>
    /// AntFlow 按钮操作后置处理器
    /// </summary>
    public class AntFlowButtonsOperationPostProcessor : IAntFlowOrderPostProcessor<BusinessDataVo>
    {
        private readonly IWorkflowButtonOperationHandler _workflowButtonHandler;

        public AntFlowButtonsOperationPostProcessor(IWorkflowButtonOperationHandler workflowButtonHandler)
        {
            _workflowButtonHandler = workflowButtonHandler;
        }

        /// <summary>
        /// 后置处理方法，根据操作类型调用相应的按钮事件
        /// </summary>
        public void PostProcess(BusinessDataVo vo)
        {
            var poEnum = ProcessOperationEnumExtensions.GetEnumByCode(vo.OperationType);
            if (poEnum == null)
            {
                throw new ArgumentException("无效的操作类型: " + vo.OperationType);
            }

            switch (poEnum)
            {
                case ProcessOperationEnum.BUTTON_TYPE_SUBMIT:
                    _workflowButtonHandler.onSubmit(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_RESUBMIT:
                    _workflowButtonHandler.onResubmit(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_AGREE:
                    _workflowButtonHandler.onAgree(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE:
                    _workflowButtonHandler.onDisAgree(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_VIEW_BUSINESS_PROCESS:
                    _workflowButtonHandler.onViewBusinessProcess(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_ABANDON:
                    _workflowButtonHandler.onAbandon(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_UNDERTAKE:
                    _workflowButtonHandler.onUndertake(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_CHANGE_ASSIGNEE:
                    _workflowButtonHandler.onChangeAssignee(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_STOP:
                    _workflowButtonHandler.onStop(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_FORWARD:
                    _workflowButtonHandler.onForward(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_BACK_TO_MODIFY:
                    _workflowButtonHandler.onBackToModify(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_JP:
                    _workflowButtonHandler.onJp(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_ZB:
                    _workflowButtonHandler.onZb(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_CHOOSE_ASSIGNEE:
                    _workflowButtonHandler.onChooseAssignee(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_BACK_TO_ANY_NODE:
                    _workflowButtonHandler.onBackToAnyNode(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_REMOVE_ASSIGNEE:
                    _workflowButtonHandler.onRemoveAssignee(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_ADD_ASSIGNEE:
                    _workflowButtonHandler.onAddAssignee(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_CHANGE_FUTURE_ASSIGNEE:
                    _workflowButtonHandler.onChangeFutureAssignee(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_REMOVE_FUTURE_ASSIGNEE:
                    _workflowButtonHandler.onRemoveFutureAssignee(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_ADD_FUTURE_ASSIGNEE:
                    _workflowButtonHandler.onAddFutureAssignee(vo);
                    break;
                case ProcessOperationEnum.BUTTON_TYPE_PROCESS_DRAW_BACK:
                    _workflowButtonHandler.onProcessDrawBack(vo);
                    break;
                default:
                    throw new NotSupportedException("不支持的操作类型: " + poEnum);
            }
        }

        /// <summary>
        /// 执行顺序
        /// </summary>
        public int Order()
        {
            return 0;
        }
    }