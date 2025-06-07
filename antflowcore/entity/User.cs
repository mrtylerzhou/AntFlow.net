using System;
using System.Text.Json.Serialization;
using antflowcore.vo;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entities
{
    [Table(Name = "t_user")]
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [JsonPropertyName("userName")]
        [Column(Name = "user_name")]
        public string Name { get; set; }
        [Column(Name = "mobile")]
        public string? Mobile { get; set; }

        [Column(Name = "email")]
        public string? Email { get; set; }

        [Column(Name = "leader_id")]
        public long? LeaderId { get; set; }

        [Column(Name = "hrbp_id")]
        public long? HrbpId { get; set; }

        [Column(Name = "mobile_is_show")]
        public bool? MobileIsShow { get; set; }

        [Column(Name = "department_id")]
        public long? DepartmentId { get; set; }

        [Column(Name = "path")]
        public string? Path { get; set; }

        [Column(Name = "is_del")]
        public bool? IsDel { get; set; }

        [Column(Name = "head_img")]
        public string? HeadImg { get; set; }
    }

    public static class UserExtensions
    {
        public static BaseIdTranStruVo ToBaseIdTranStruVo(this User user)
        {
            BaseIdTranStruVo tmp = new BaseIdTranStruVo
            {
                Id = user.Id.ToString(),
                Name = user.Name
            };
            return tmp;
        }
    }
}
