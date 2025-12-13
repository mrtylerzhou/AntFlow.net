
using System.Threading.Tasks;
using antflow.abp.Configuration.Dto;

namespace antflow.abp.Configuration;

public interface IConfigurationAppService
{
    Task ChangeUiTheme(ChangeUiThemeInput input);
}
