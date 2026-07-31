
namespace AntFlowCore.Base.util
{
    public interface StringConstants
    {
        public const string SCAN_BASE_PACKAGES = "org.openoa";
        public const string SPECIAL_CHARACTERS = "[ _`~!@#$%^&*()+=|{}':;',\\[\\].<>/?~！@#￥%……&*（）——+|{}【】‘；：”“’。，、？]|\n|\r|\t";
        public const string BPMN_CODE_SPLITMARK = "-";

        public const string FORM_CODE_LINKMARK = "_";
        public const string CREATEUSERNAME = "defaultUser";
        public const int CREATEUSERID = 1;
        public const string JOBNUM = "9527";
        public const string MOCK_LOGIN_USER_KEY = "mockedloginuser";
        public const string DB_NAME_1 = "activiti_main";
        public const string DB_NAME_2 = "jimu_biz";
        public const string DRUID_POOL_NAME_PREFIX = "druidDataSourcePool_";
        public const string DB_TRANSACTION_MANAGERNAME_SUFFIX = "transactionmanager";

        public const string ADAPTOR_FACTORY_BEANNAME = "jimuAdaptorFactory";

        public const string TASK_ASSIGNEE_NAME = "assigneeName";
        public const string VERIFY_COMMENT = "verifyComment";
        public const string PROJECT_NAME = "antFlow";
        public const string LOWFLOW_FORM_CODE = "LF";
        public const string LOWFLOW_FORM_CONTAINER_TYPE = "container";
        public const string LOWFLOW_CONDITION_CONTAINER_FIELD_NAME = "LfConditions";
        public const string LOWFLOW_FORM_DATA_MAIN_TABLE_NAME = "t_lf_main";
        public const string LOWFLOW_FORM_DATA_FIELD_TABLE_NAME = "t_lf_main_field";

        public const string FORMCODE_NO_CAMAL = "formCode";
        public const string FORM_CODE = "form_code";

        public const string TENANT_USER = "tenantUser";
        public const string LOWCODE_FLOW_DICT_TYPE = "lowcodeflow";
        

    
        public  const String outSideMarker = "outSide";
       
        public const  String outSideAccessmarker = "outSideAccess";
        
        public const String NUM_OPERATOR="NumberOperator";

        public const string START_USER_NODE_NAME = "发起人";
        
        public const string DEFAULT_TASK_DELETE_REASON ="completed";
        public const string TASK_FINISH_REASON="finished";
        public const string BACK_TO_MODIFY_DESC = "打回修改";
        public const string ADMIN_RIGHTS = "3060101";
        public const string DYNAMIC_APPROVER = "--";
        public const String TENANT_ID="tenantId";
        public const String DEFAULT_TENANT="default";
        public const string BIG_WHITE_BLANK = " ";
        
        public const String HIDDEN_FIELD_PERMISSION="H";
        public const string HIDDEN_FIELD_VALUE = "******";
        public const string READ_ONLY_FIELD_PERMISSION="R";

        // Node label value constants (aligned with Java StringConstants)
        public const string DYNAMIC_CONDITION_NODE = "af_syslabel_dynamiccondition";
        public const string COPY_NODE = "af_syslabel_copynode";
        public const string COPY_NODEV2 = "af_syslabel_copynodeV2";
        public const string AUTOMATIC_NODE = "auto_node";
        public const string SKIPPED_ASSIGNEE = "lbl_skipped_assignee";

        // 条件审批节点 / 条件抄送节点 (对等 Java 版 nodeType=12/13)
        public const string CONDITION_APPROVE_NODE = "condition_approve_node";
        public const string CONDITION_COPY_NODE = "condition_copy_node";

        // 条件审批/条件抄送运行时 verifyInfo 备注
        public const string AF_CONDITION_APPROVE_AUTO_COMMENT = "条件审批自动通过,条件评估结果:True";
        public const string AF_CONDITION_APPROVE_WAIT_COMMENT = "条件审批条件未满足,等待人工审批";
        public const string AF_CONDITION_COPY_EXECUTE_COMMENT = "条件抄送执行,条件评估结果:True";
        public const string AF_CONDITION_COPY_SKIP_COMMENT = "条件抄送跳过,条件评估结果:False";

        // 自动节点运行时 verifyInfo 备注
        public const string AF_AUTO_EVALUATE_SKIP_COMMENT = "自动节点自动跳过,条件评估结果:{0}";

        // Dynamic condition related constants
        public const string CONDITION_CHANGED = "condition_changed";
        public const string CURRENT_USER_ALREADY_PROCESSED = "currentUserAlreadyProcessed";

        // Adjacent deduplication auto-skip comment and suffix
        public const string AF_AUTO_SKIP_COMMENT = "相同审批人自动跳过";
        public const string AF_SKIP_ASSIGNEE_NODE_SUFFIX = "⬇️";

        // Element name suffixes for special node types (aligned with Java StringConstants)
        public const string AF_COPY_V2_NODE_SUFFIX = "\uD83D\uDCE2";        // 抄送节点v2后缀
        public const string AF_NODE_SIGN_SUFFIX = "\uD83D\uDD00";           // 会签后缀
        public const string AF_NODE_SIGN_IN_ORDER_SUFFIX = "\uD83D\uDD03";  // 顺序会签后缀
        public const string AF_NODE_OR_SIGN_SUFFIX = "\uD83D\uDD02";        // 或签后缀
        public const string AF_DEFAULT_NODE_NAME = "审核人";
        public const string LASTNODE_COPY = "af_syslabel_lastnode_copy";

        // Thread-local key for duplication process strategy (used by element adaptors)
        public const string DUPLICATION_PROCESS_STRATEGY = "duplicationProcessStrategy";

        // 选择条件节点标签 (对等 Java StringConstants.AF_SYSLABEL_PICK_CONDITION)
        public const string AF_SYSLABEL_PICK_CONDITION = "af_syslabel_pick_condition";

        // 上一节点指定审批人相关常量 (对等 Java StringConstants)
        public const string AF_SYSLABEL_PREV_NODE_APPOINTED = "af_syslabel_prev_node_appointed";
        public const string AF_SYSLABEL_APPOINT_NEXT_NODE_APPROVER = "af_syslabel_appoint_next_node_approver";
        /// <summary>不同意按钮配置了退回行为时贴此标签,运行时EndProcessService据此转发BackToModifyService</summary>
        public const string AF_SYSLABEL_DISAGREE_BACK = "af_syslabel_disagree_back";
        // Thread-local key for passing nextNodeApprovers from ButtonOperationService to AFTaskService
        public const string NEXT_NODE_APPROVER = "nextNodeApprover";
        // Thread-local key for runtime BusinessDataVo (set by BpmnSendMessageAspect, read by BpmnTaskListener)
        public const string AF_RUNTIME_BUISINESS_INFO = "af_runtime_business_info";
        // Thread-local key for runtime BpmnConf (set by BpmnSendMessageAspect)
        public const string AF_RUNTIME_BPMN_CONF = "af_runtime_bpmn_conf";
    }
}
