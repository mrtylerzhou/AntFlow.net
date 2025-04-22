using antflowcore.vo;
using FreeSql.DataAnnotations;
using System.Text.Json.Serialization;

namespace antflowcore.entity
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