using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace antflow.abp.Users.Dto;

//custom PagedResultRequestDto
public class PagedUserResultRequestDto : PagedResultRequestDto, IShouldNormalize
{
    public string Keyword { get; set; }
    public bool? IsActive { get; set; }

    public string Sorting { get; set; }

    public void Normalize()
    {
        if (string.IsNullOrEmpty(Sorting))
        {
            Sorting = "UserName,EmailAddress";
        }

        Keyword = Keyword?.Trim();
    }
}
