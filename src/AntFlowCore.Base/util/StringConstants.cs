
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

        // Dynamic condition related constants
        public const string CONDITION_CHANGED = "condition_changed";
        public const string CURRENT_USER_ALREADY_PROCESSED = "currentUserAlreadyProcessed";

        // Adjacent deduplication auto-skip comment and suffix
        public const string AF_AUTO_SKIP_COMMENT = "相同审批人自动跳过";
        public const string AF_AUTO_EVALUATE_SKIP_COMMENT = "自动节点自动跳过,条件评估结果:{0}";
        public const string AF_SKIP_ASSIGNEE_NODE_SUFFIX = "⬇️";

        // Thread-local key for duplication process strategy (used by element adaptors)
        public const string DUPLICATION_PROCESS_STRATEGY = "duplicationProcessStrategy";
    }
}
