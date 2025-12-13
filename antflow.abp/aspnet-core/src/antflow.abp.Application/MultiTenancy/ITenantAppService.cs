using Abp.Application.Services;
using antflow.abp.MultiTenancy.Dto;

namespace antflow.abp.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

