using Abp.Application.Services.Dto;
using Abp.Authorization.Roles;
using System.ComponentModel.DataAnnotations;
using antflow.abp.Authorization.Roles;

namespace antflow.abp.Roles.Dto;

public class RoleEditDto : EntityDto<int>
{
    [Required]
    [StringLength(AbpRoleBase.MaxNameLength)]
    public string Name { get; set; }

    [Required]
    [StringLength(AbpRoleBase.MaxDisplayNameLength)]
    public string DisplayName { get; set; }

    [StringLength(Role.MaxDescriptionLength)]
    public string Description { get; set; }

    public bool IsStatic { get; set; }
}