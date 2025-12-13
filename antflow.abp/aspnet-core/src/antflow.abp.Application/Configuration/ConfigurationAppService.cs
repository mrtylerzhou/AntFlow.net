using Abp.Authorization;
using Abp.Runtime.Session;
using System.Threading.Tasks;
using antflow.abp.Configuration.Dto;

namespace antflow.abp.Configuration;

[AbpAuthorize]
public class ConfigurationAppService : flowerAppServiceBase, IConfigurationAppService
{
    public async Task ChangeUiTheme(ChangeUiThemeInput input)
    {
        await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
    }
}
