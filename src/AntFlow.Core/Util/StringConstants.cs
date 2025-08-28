namespace AntFlow.Core.Constant;

public interface StringConstants
{
    public const string SCAN_BASE_PACKAGES = "org.openoa";

    public const string SPECIAL_CHARACTERS =
        "[ _`~!@#$%^&*()+=|{}':;',\\[\\].<>/?~??@#??%????&*????????+|{}????????????????????????]|\n|\r|\t";

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


    public const string outSideMarker = "outSide";

    public const string outSideAccessmarker = "outSideAccess";

    public const string NUM_OPERATOR = "NumberOperator";

    public const string START_USER_NODE_NAME = "??????";

    public const string DEFAULT_TASK_DELETE_REASON = "completed";
    public const string TASK_FINISH_REASON = "finished";
    public const string BACK_TO_MODIFY_DESC = "??????";
    public const string ADMIN_RIGHTS = "3060101";
    public const string DYNAMIC_APPROVER = "--";
    public const string TENANT_ID = "tenantId";
    public const string DEFAULT_TENANT = "default";
}