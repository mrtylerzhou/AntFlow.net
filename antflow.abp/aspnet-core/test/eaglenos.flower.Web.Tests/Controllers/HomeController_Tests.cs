using eaglenos.flower.Models.TokenAuth;
using eaglenos.flower.Web.Controllers;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace eaglenos.flower.Web.Tests.Controllers;

public class HomeController_Tests : flowerWebTestBase
{
    [Fact]
    public async Task Index_Test()
    {
        await AuthenticateAsync(null, new AuthenticateModel
        {
            UserNameOrEmailAddress = "admin",
            Password = "123qwe"
        });

        //Act
        var response = await GetResponseAsStringAsync(
            GetUrl<HomeController>(nameof(HomeController.Index))
        );

        //Assert
        response.ShouldNotBeNullOrEmpty();
    }
}