using System.Text.Json.Serialization;

namespace antflowcore.constant.enus;

 public static class ProcessEnum
    {
        /**
         * process category
         */
        public static readonly ProcessItem AgencyType = new(1, "委托流程");
        public static readonly ProcessItem CirculateType = new(2, "传阅流程");
        public static readonly ProcessItem LaunchType = new(3, "发起流程");
        public static readonly ProcessItem ParticipType = new(4, "参与流程");
        public static readonly ProcessItem EntrustType = new(5, "代办流程");
        public static readonly ProcessItem SuperType = new(6, "流程代办流程");

        /**
         * process type
         */
        public static readonly ProcessItem HostType = new(7, "承办类型");
        public static readonly ProcessItem RevokeType = new(8, "撤销类型");
        public static readonly ProcessItem WithdrawType = new(9, "撤回类型");
        public static readonly ProcessItem NoticeType = new(10, "通知类型");
        public static readonly ProcessItem CirculateStartType = new(11, "传阅类型");

      

        /**
         * process node
         */
        public static readonly ProcessItem StartTaskKey = new(71, "task1418018332271");
        public static readonly ProcessItem TowTaskKey = new(72, "task1418018332272");
        public static readonly ProcessItem ThreeTaskKey = new(73, "task1418018332273");
        public static readonly ProcessItem FourTaskKey = new(74, "task1418018332274");
        public static readonly ProcessItem FiveTaskKey = new(75, "task1418018332275");
        public static readonly ProcessItem SixTaskKey = new(76, "task1418018332276");
        public static readonly ProcessItem SevenTaskKey = new(77, "task1418018332277");
        public static readonly ProcessItem EightTaskKey = new(78, "task1418018332278");
        public static readonly ProcessItem NineTaskKey = new(79, "task1418018332279");
        public static readonly ProcessItem HtnTaskKey = new(80, "task1418018332280");
        public static readonly ProcessItem ThirteenTaskKey = new(81, "task1418018332281");
        public static readonly ProcessItem FourteenTaskKey = new(82, "task1418018332282");
        public static readonly ProcessItem FifteenTaskKey = new(83, "task1418018332283");
        public static readonly ProcessItem ElevenTaskKey = new(90, "task14180183322");
        public static readonly ProcessItem TwelveTaskKey = new(93, "task14180183322");
        
        private static readonly Dictionary<int, string> CodeToDescMap =
            GetAll().ToDictionary(e => e.Code, e => e.Desc);

        public static string? GetDescByCode(int code)
        {
            return CodeToDescMap.TryGetValue(code, out var desc) ? desc : null;
        }

        public static IEnumerable<ProcessItem> GetAll()
        {
            return typeof(ProcessEnum)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.FieldType == typeof(ProcessItem))
                .Select(f => (ProcessItem)f.GetValue(null)!);
        }
    }

    public class ProcessItem
    {
        [JsonPropertyName("code")]
        public int Code { get; }

        [JsonPropertyName("desc")]
        public string Desc { get; }

        public ProcessItem(int code, string desc)
        {
            Code = code;
            Desc = desc;
        }
    }