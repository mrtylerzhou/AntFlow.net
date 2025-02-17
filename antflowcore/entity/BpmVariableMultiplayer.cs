using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entities
{
    [Table(Name = "t_bpm_variable_multiplayer")]
    public class BpmVariableMultiplayer
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
        /// 集合名称
        /// </summary>
        [Column(Name = "collection_name")]
        public string CollectionName { get; set; }

        /// <summary>
        /// 签名类型 1表示全签，2表示或签
        /// </summary>
        [Column(Name = "sign_type")]
        public int SignType { get; set; }

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
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新用户
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column(Name = "update_time",IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }
        
        
    }
}
