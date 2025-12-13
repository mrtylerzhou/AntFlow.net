using Abp.Application.Services;
using Abp.Application.Services.Dto;

using System.Threading.Tasks;
using antflow.abp.Roles.Dto;
using antflow.abp.Users.Dto;

namespace antflow.abp.Users;

public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
{
    Task DeActivate(EntityDto<long> user);
    Task Activate(EntityDto<long> user);
    Task<ListResultDto<RoleDto>> GetRoles();
    Task ChangeLanguage(ChangeUserLanguageDto input);

    Task<bool> ChangePassword(ChangePasswordDto input);
}
