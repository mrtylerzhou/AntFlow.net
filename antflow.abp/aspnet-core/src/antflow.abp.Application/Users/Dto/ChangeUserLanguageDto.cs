using System.ComponentModel.DataAnnotations;

namespace antflow.abp.Users.Dto;

public class ChangeUserLanguageDto
{
    [Required]
    public string LanguageName { get; set; }
}