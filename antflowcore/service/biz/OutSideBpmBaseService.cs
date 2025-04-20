using AntFlowCore.Constants;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.biz;

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
        
        GenericEmployee loginEmployee = new GenericEmployee
        {
            UserId = SecurityUtils.GetLogInEmpId(),
            Username = SecurityUtils.GetLogInEmpName()
        };

        List<OutSideBpmBusinessParty> outSideBpmBusinessPartys = new List<OutSideBpmBusinessParty>();

        // TODO: 权限判断逻辑可根据实际权限系统调整
        if (loginEmployee.Permissions.Contains(StringConstants.ADMIN_RIGHTS))
        {
           
            // 有全部权限，查询全部业务方
            outSideBpmBusinessPartys = _outSideBpmBusinessPartyService.baseRepo.Where(a => 1 == 1).ToList();
        }
        else
        {
            // 普通管理员，根据配置查询可管理的业务方
            var businessPartyIds = _outSideBpmAdminPersonnelService.GetBusinessPartyIdByEmployeeId(loginEmployee.UserId, permCodes);
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

        var result = outSideBpmBusinessPartys
            .Select(p => new OutSideBpmBusinessPartyVo
            {
                Id = p.Id,
                Name = p.Name,
                BusinessPartyMark = p.BusinessPartyMark,
                Type = p.Type
            })
            .ToList();

        if (!string.IsNullOrWhiteSpace(name))
        {
            result = result.Where(p => p.Name != null && p.Name.Contains(name)).ToList();
        }

        return result;
    }

}