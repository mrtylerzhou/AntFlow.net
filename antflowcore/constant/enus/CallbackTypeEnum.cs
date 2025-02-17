namespace antflowcore.constant.enus;

  /// <summary>
    /// 回调类型枚举
    /// </summary>
    public class CallbackTypeEnum
    {
        // 属性
        public string Mark { get; }
        public string Desc { get; }
        public string BeanId { get; }

        // 私有构造函数，确保只能通过定义的实例访问
        private CallbackTypeEnum(string mark, string desc, string beanId)
        {
            Mark = mark;
            Desc = desc;
            BeanId = beanId;
        }

        // 静态实例
        public static readonly CallbackTypeEnum CONF_CONDITION_CALL_BACK = 
            new CallbackTypeEnum("CONF_CONDITION_CALL_BACK", "条件分支回调", "CONF_CONDITION_CALL_BACK");

        public static readonly CallbackTypeEnum PROC_CONDITION_CALL_BACK = 
            new CallbackTypeEnum("PROC_CONDITION_CALL_BACK", "条件判断回调", "PROC_CONDITION_CALL_BACK");

        public static readonly CallbackTypeEnum PROC_SUBMIT_CALL_BACK = 
            new CallbackTypeEnum("PROC_SUBMIT_CALL_BACK", "提交操作回调", "PROC_SUBMIT_CALL_BACK");

        public static readonly CallbackTypeEnum PROC_STARTED_CALL_BACK = 
            new CallbackTypeEnum("PROC_STARTED_CALL_BACK", "流程发起完成回调", "PROC_BASE_CALL_BACK");

        public static readonly CallbackTypeEnum PROC_COMMIT_CALL_BACK = 
            new CallbackTypeEnum("PROC_COMMIT_CALL_BACK", "流转操作回调", "PROC_BASE_CALL_BACK");

        public static readonly CallbackTypeEnum PROC_END_CALL_BACK = 
            new CallbackTypeEnum("PROC_END_CALL_BACK", "结束操作回调", "PROC_BASE_CALL_BACK");

        public static readonly CallbackTypeEnum PROC_FINISH_CALL_BACK = 
            new CallbackTypeEnum("PROC_FINISH_CALL_BACK", "完成操作回调", "PROC_BASE_CALL_BACK");

        // 获取所有实例列表
        public static IEnumerable<CallbackTypeEnum> Values
        {
            get
            {
                yield return CONF_CONDITION_CALL_BACK;
                yield return PROC_CONDITION_CALL_BACK;
                yield return PROC_SUBMIT_CALL_BACK;
                yield return PROC_STARTED_CALL_BACK;
                yield return PROC_COMMIT_CALL_BACK;
                yield return PROC_END_CALL_BACK;
                yield return PROC_FINISH_CALL_BACK;
            }
        }

        // 重写 ToString 方法
        public override string ToString()
        {
            return $"{Mark} - {Desc}";
        }
    }