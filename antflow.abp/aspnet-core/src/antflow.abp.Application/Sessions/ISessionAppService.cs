using Abp.Application.Services;
using System.Threading.Tasks;
using antflow.abp.Sessions.Dto;

namespace antflow.abp.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
