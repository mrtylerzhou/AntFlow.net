using AntFlow.Core.Constant;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Business;

public class OutSideBpmBaseService
{
    private readonly OutSideBpmAdminPersonnelService _outSideBpmAdminPersonnelService;
    private readonly OutSideBpmBusinessPartyService _outSideBpmBusinessPartyService;

    public OutSideBpmBaseService(
        OutSideBpmAdminPersonnelService outSideBpmAdminPersonnelService,
        OutSideBpmBusinessPartyService outSideBpmBusinessPartyService)
    {
        _outSideBpmAdminPersonnelService = outSideBpmAdminPersonnelService;
        _outSideBpmBusinessPartyService = outSideBpmBusinessPartyService;
    }

    public List<OutSideBpmBusinessPartyVo> GetEmplBusinessPartys(string name, params string[] permCodes)
    {
        GenericEmployee loginEmployee = new()
        {
            UserId = SecurityUtils.GetLogInEmpId(), Username = SecurityUtils.GetLogInEmpName()
        };

        List<OutSideBpmBusinessParty> outSideBpmBusinessPartys = new();

        // TODO: 根据用户权限获取业务方列表
        if (loginEmployee.Permissions.Contains(StringConstants.ADMIN_RIGHTS))
        {
            // 管理员获取所有业务方
            outSideBpmBusinessPartys = _outSideBpmBusinessPartyService.baseRepo.Where(a => 1 == 1).ToList();
        }
        else
        {
            // 普通用户根据权限获取业务方
            List<long>? businessPartyIds =
                _outSideBpmAdminPersonnelService.GetBusinessPartyIdByEmployeeId(loginEmployee.UserId, permCodes);
            if (businessPartyIds != null && businessPartyIds.Any())
            {
                outSideBpmBusinessPartys = _outSideBpmBusinessPartyService.baseRepo
                    .Where(p => businessPartyIds.Contains(p.Id)).ToList();
            }
            else
            {
                outSideBpmBusinessPartys.Add(new OutSideBpmBusinessParty { Id = -1 });
            }
        }

        if (outSideBpmBusinessPartys == null || !outSideBpmBusinessPartys.Any())
        {
            return new List<OutSideBpmBusinessPartyVo>();
        }

        List<OutSideBpmBusinessPartyVo>? result = outSideBpmBusinessPartys
            .Select(p => new OutSideBpmBusinessPartyVo
            {
                Id = p.Id, Name = p.Name, BusinessPartyMark = p.BusinessPartyMark, Type = p.Type
            })
            .ToList();

        if (!string.IsNullOrWhiteSpace(name))
        {
            result = result.Where(p => p.Name != null && p.Name.Contains(name)).ToList();
        }

        return result;
    }
}