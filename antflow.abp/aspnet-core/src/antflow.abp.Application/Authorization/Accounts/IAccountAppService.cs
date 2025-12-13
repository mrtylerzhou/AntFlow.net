using Abp.Application.Services;
using System.Threading.Tasks;
using antflow.abp.Authorization.Accounts.Dto;

namespace antflow.abp.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
