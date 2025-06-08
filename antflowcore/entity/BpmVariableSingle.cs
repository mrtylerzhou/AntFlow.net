using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entities
{
    [Table(Name = "t_bpm_variable_single")]
    public class BpmVariableSingle
    {
        /// <summary>
        /// 自增ID
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }

        /// <summary>
        /// 变量ID
        /// </summary>
        [Column(Name = "variable_id")]
        public long VariableId { get; set; }

        /// <summary>
        /// 元素ID
        /// </summary>
        [Column(Name = "element_id")]
        public string ElementId { get; set; }

        /// <summary>
        /// 节点ID
        /// </summary>
        [Column(Name = "node_id")]
        public string NodeId { get; set; }

        /// <summary>
        /// 元素名称
        /// </summary>
        [Column(Name = "element_name")]
        public string ElementName { get; set; }

        /// <summary>
        /// 单一指派参数名称
        /// </summary>
        [Column(Name = "assignee_param_name")]
        public string AssigneeParamName { get; set; }

        /// <summary>
        /// 被指派人
        /// </summary>
        public string Assignee { get; set; }

        /// <summary>
        /// 被指派人姓名
        /// </summary>
        [Column(Name = "assignee_name")]
        public string AssigneeName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 是否删除，0表示否，1表示是
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新用户
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column(Name = "update_time")]
        public DateTime? UpdateTime { get; set; }

        // 无参构造函数
        public BpmVariableSingle() { }

        // 带参构造函数
        public BpmVariableSingle(long id, long variableId, string elementId, string nodeId, string elementName,
            string assigneeParamName, string assignee, string assigneeName, string remark, int isDel,
            string createUser, DateTime createTime, string updateUser, DateTime updateTime)
        {
            Id = id;
            VariableId = variableId;
            ElementId = elementId;
            NodeId = nodeId;
            ElementName = elementName;
            AssigneeParamName = assigneeParamName;
            Assignee = assignee;
            AssigneeName = assigneeName;
            Remark = remark;
            IsDel = isDel;
            CreateUser = createUser;
            CreateTime = createTime;
            UpdateUser = updateUser;
            UpdateTime = updateTime;
        }
    }
}
