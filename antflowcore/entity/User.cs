using System.Text.Json.Serialization;
using antflowcore.vo;

namespace antflowcore.entity
{
    /**
     * 用户表实际上是demo表,用户改为自己的数据库时不必映射为User,只需要将自己的用户表实体转成BaseIdTranStruVo(只需要用户id和用户名)
     */
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
        public string Name { get; set; }

        public string? Mobile { get; set; }

        public string? Email { get; set; }

        public long? LeaderId { get; set; }

        public long? HrbpId { get; set; }

        public bool? MobileIsShow { get; set; }

        public long? DepartmentId { get; set; }

        public string? Path { get; set; }

        public bool? IsDel { get; set; }

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
