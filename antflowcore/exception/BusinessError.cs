namespace antflowcore.exception;

 public sealed class BusinessError

    {

        public int Code { get; }

        public string Msg { get; }


        private BusinessError(int code, string msg)

        {

            Code = code;

            Msg = msg;

        }


        // ========== 参数错误 ==========

        public static readonly BusinessError PARAMS_IS_NULL = new BusinessError(1418300271, "参数为空错误");

        public static readonly BusinessError PARAMS_NOT_COMPLETE = new BusinessError(1418300272, "参数不全");

        public static readonly BusinessError PARAMS_TYPE_ERROR = new BusinessError(1418300273, "参数类型匹配错误");

        public static readonly BusinessError PARAMS_IS_INVALID = new BusinessError(1418300274, "参数无效");

        public static readonly BusinessError STATUS_ERROR = new BusinessError(1418300275, "状态异常");

        public static readonly BusinessError DATA_IS_OVER_MAXIMUM_LENGTH = new BusinessError(1418300276, "数据长度超过最大长度:{0}");

        public static readonly BusinessError FILE_FORMAT_ERROR = new BusinessError(1418300277, "文件格式不正确");

        public static readonly BusinessError PARAMS_MISMATCH = new BusinessError(1418300278, "参数不匹配");

        public static readonly BusinessError PARAMS_NULL_AFTER_CONVERT = new BusinessError(1418300279, "未定义的类型");

        public static readonly BusinessError CAN_NOT_GET_VALUE_FROM_DB = new BusinessError(1418300280, "未能从数据库中获取到值");


        // ========== 数据处理错误 ==========

        public static readonly BusinessError DATA_PROCESS_ERRROR = new BusinessError(1418300371, "数据处理错误");

        public static readonly BusinessError UPDATE_FAIL = new BusinessError(1418300371, "更新数据失败"); // 注意：与上一条 code 重复！

        public static readonly BusinessError INSERT_FAIL = new BusinessError(1418300372, "插入数据失败");

        public static readonly BusinessError SELECT_FAIL = new BusinessError(1418300373, "查询失败，未找到记录");

        public static readonly BusinessError DATA_NOT_FOUND = new BusinessError(1418300374, "数据未找到");

        public static readonly BusinessError DATA_IS_WRONG = new BusinessError(1418300375, "数据有误");

        public static readonly BusinessError DATA_ALREADY_EXISTED = new BusinessError(1418300376, "数据已存在");

        public static readonly BusinessError UPDATE_FAIL_EXISTS = new BusinessError(1418300377, "更新失败，活动状态已经被更改");

        public static readonly BusinessError DATA_TOO_LONG = new BusinessError(1418300378, "数据导出超出上限{0}，请缩小查询时间范围！");

        public static readonly BusinessError UNFINISHED_TASK = new BusinessError(1418300379, "已有未完成任务,请在{0}分钟之后再导出!");

        public static readonly BusinessError DATA_EXCEED_EXCEL_LIMIT = new BusinessError(1418300380, "数据超过100万,不能导出!");

        public static readonly BusinessError IMPORT_TASK_ERROR = new BusinessError(1418300381, "导入任务保存失败，请重新导入!");

        public static readonly BusinessError FILE_UPLOAD_ERROR = new BusinessError(1418300382, "文件上传失败");

        public static readonly BusinessError FILE_DOWNLOAD_ERROR = new BusinessError(1418300384, "文件下载失败");

        public static readonly BusinessError FILE_DELETE_ERROR = new BusinessError(1418300385, "文件删除失败");

        public static readonly BusinessError FILE_GET_ERROR = new BusinessError(1418300386, "获取文件失败");

        public static readonly BusinessError FILE_TYPE_ERROR = new BusinessError(1418300387, "文件类型错误");

        public static readonly BusinessError MODULE_TYPE_ERROR = new BusinessError(1418300388, "模块类型错误");


        // ========== 用户/组织关系错误 ==========

        public static readonly BusinessError USER_NOT_EXIST = new BusinessError(1418300471, "用户不存在");

        public static readonly BusinessError USER_NOT_LOGGED_IN = new BusinessError(1418300472, "用户未登陆");

        public static readonly BusinessError USER_ACCOUNT_ERROR = new BusinessError(1418300473, "用户名或密码错误");

        public static readonly BusinessError USER_ACCOUNT_FORBIDDEN = new BusinessError(1418300474, "用户账户已被禁用");

        public static readonly BusinessError USER_HAS_EXIST = new BusinessError(1418300475, "该用户已存在");

        public static readonly BusinessError USER_CODE_ERROR = new BusinessError(1418300476, "验证码错误");

        public static readonly BusinessError USER_NOT_BELONG_CURRENT_ORG = new BusinessError(1418300476, "用户不属于当前组织"); // ⚠️ code 重复！


        // ========== 系统/接口错误 ==========

        public static readonly BusinessError ACT_INNER_ERROER = new BusinessError(1418300500, "activiti引擎内部错误");

        public static readonly BusinessError ANTFLOW_INNER_ERROER = new BusinessError(1418300501, "antflow引擎内部错误");

        public static readonly BusinessError API_INNER_INVOKE_ERROR = new BusinessError(1418300571, "系统内部接口调用异常");

        public static readonly BusinessError API_OUTER_INVOKE_ERROR = new BusinessError(1418300572, "系统外部接口调用异常");

        public static readonly BusinessError API_ACCESS_DENIED = new BusinessError(1418300573, "接口禁止访问");

        public static readonly BusinessError API_ENDPOINT_INVALID = new BusinessError(1418300574, "接口地址无效");

        public static readonly BusinessError API_REQUEST_TIMEOUT = new BusinessError(1418300575, "接口请求超时");

        public static readonly BusinessError INTERFACE_EXCEED_LOAD = new BusinessError(999005050, "接口负载过高");

        public static readonly BusinessError SYSTEM_INNER_ERROR = new BusinessError(999005060, " 服务在开小差，请稍等");

        public static readonly BusinessError SERVICE_CALL_ERROR = new BusinessError(999005070, "跨服务调用错误");

        public static readonly BusinessError SERVICE_NOT_AVAILABLE = new BusinessError(999005080, "服务不可用");

        public static readonly BusinessError TOO_MANY_REQUEST = new BusinessError(999005081, "请求过于频繁");

        public static readonly BusinessError DB_ACCESS_ERROR = new BusinessError(999005082, "数据库请求错误");


        // ========== 权限错误 ==========

        public static readonly BusinessError PERMISSION_NO_ACCESS = new BusinessError(1418300671, "当前按钮没有访问权限");

        public static readonly BusinessError CONNECTION_RELATION_ERROR = new BusinessError(1418300672, "关联关系不匹配");

        public static readonly BusinessError RIGHT_VIOLATE = new BusinessError(1418300673, "超出访问权限");

        public static readonly BusinessError RIGHT_INVALID = new BusinessError(1418300674, "当前状态不允许操作");


        // ========== 配置错误 ==========

        public static readonly BusinessError CONFIG_FORMAT_ERROR = new BusinessError(1418300771, "配置格式错误");

        public static readonly BusinessError CONFIG_ITEM_NOT_EXIST = new BusinessError(1418300771, "配置条目不存在");      // ⚠️ code 重复！

        public static readonly BusinessError CONFIG_ITEM_NOT_INVALID = new BusinessError(1418300771, "配置内容非法");     // ⚠️ code 重复！


        // ========== 辅助方法 ==========

        public string FormatMsg(params object[] args)

        {

            if (args == null || args.Length == 0)

                return Msg;

            return string.Format(Msg, args);

        }


        public override string ToString()

        {

            return $"BusinessError [Code={Code}, Msg={Msg}]";

        }

    }
    