using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Repository;

public class OutSideBpmCallbackUrlConfService : AFBaseCurdRepositoryService<OutSideBpmCallbackUrlConf>,
    IOutSideBpmCallbackUrlConfService
{
    private readonly UserService _employeeService;

    private readonly OutSideBpmAdminPersonnelService _outSideBpmAdminPersonnelService;

    public OutSideBpmCallbackUrlConfService(
        OutSideBpmAdminPersonnelService outSideBpmAdminPersonnelService,
        UserService employeeService,
        IFreeSql freeSql) : base(freeSql)
    {
        _outSideBpmAdminPersonnelService = outSideBpmAdminPersonnelService;
        _employeeService = employeeService;
    }

    public OutSideBpmCallbackUrlConf GetOutSideBpmCallbackUrlConf(long businessPartyId)
    {
        OutSideBpmCallbackUrlConf outSideBpmCallbackUrlConf =
            baseRepo.Where(a => a.BusinessPartyId == businessPartyId && a.Status == 1).First();

        if (outSideBpmCallbackUrlConf == null)
        {
            throw new AFBizException("回调URL配置不存在或已禁用");
        }

        return outSideBpmCallbackUrlConf;
    }

    public List<OutSideBpmCallbackUrlConf> SelectListByFormCode(string formCode)
    {
        List<OutSideBpmCallbackUrlConf> confList = baseRepo
            .Where(a => a.FormCode == formCode && a.Status == 1)
            .ToList();
        return confList;
    }

    public OutSideBpmCallbackUrlConfVo Detail(int id)
    {
        OutSideBpmCallbackUrlConf outSideBpmCallbackUrlConf = baseRepo
            .Where(a => a.Id == id)
            .ToOne();

        OutSideBpmCallbackUrlConfVo vo = outSideBpmCallbackUrlConf.MapToVo();

        OutSideBpmBusinessPartyService outSideBpmBusinessPartyService =
            ServiceProviderUtils.GetService<OutSideBpmBusinessPartyService>();
        //query business party's info,for assemble the result
        OutSideBpmBusinessParty outSideBpmBusinessParty = outSideBpmBusinessPartyService
            .baseRepo
            .Where(a => a.Id == vo.BusinessPartyId)
            .ToOne();

        //rebuild the vo to give it detailed information for representing
        return RebuildVo(vo, outSideBpmBusinessParty);
    }

    private OutSideBpmCallbackUrlConfVo RebuildVo(
        OutSideBpmCallbackUrlConfVo vo,
        OutSideBpmBusinessParty businessParty)
    {
        // 状态名称转换
        if (vo.Status != null)
        {
            if (vo.Status == 1)
            {
                vo.StatusName = "启用";
            }
            else if (vo.Status == 2)
            {
                vo.StatusName = "禁用";
            }
        }

        // 业务方信息填充
        if (businessParty != null)
        {
            vo.BusinessPartyName = businessParty.Name;
            vo.AccessType = businessParty.Type;
            vo.AccessTypeName = BusinessPartyTypeEnum.GetDescByCode(businessParty.Type);

            List<OutSideBpmAdminPersonnel> outSideBpmAdminPersonnels = _outSideBpmAdminPersonnelService
                .baseRepo
                .Where(p => p.BusinessPartyId == businessParty.Id &&
                            p.Type == AdminPersonnelTypeEnum.ADMIN_PERSONNEL_TYPE_INTERFACE.Code)
                .ToList();

            if (outSideBpmAdminPersonnels != null && outSideBpmAdminPersonnels.Any())
            {
                List<Employee> employees = _employeeService.GetEmployeeDetailByIds(outSideBpmAdminPersonnels
                    .Select(a => a.EmployeeId)
                    .ToList());

                vo.InterfaceAdmins = employees
                    .Select(emp => new Employee { Id = emp.Id, Username = emp.Username })
                    .ToList();
            }
        }

        return vo;
    }

    public void Edit(OutSideBpmCallbackUrlConfVo vo)
    {
        // 新增时检查是否已存在相同业务方和应用的配置
        if (vo.Id is null or 0)
        {
            long count = baseRepo
                .Where(x => x.BusinessPartyId == vo.BusinessPartyId && x.ApplicationId == vo.ApplicationId)
                .Count();

            if (count > 0)
            {
                throw new AFBizException("该业务方已存在回调配置");
            }
        }

        DateTime now = DateTime.Now;
        string? loginUser = SecurityUtils.GetLogInEmpName();

        OutSideBpmCallbackUrlConf? entity = vo.Id != null
            ? baseRepo.Where(a => a.Id == vo.Id).ToOne()
            : null;

        if (entity != null)
        {
            // 更新操作
            vo.CopyToEntity(entity);
            entity.UpdateTime = now;
            entity.UpdateUser = loginUser;
            baseRepo.Update(entity);
        }
        else
        {
            // 新增操作
            entity = new OutSideBpmCallbackUrlConf();
            vo.CopyToEntity(entity);
            entity.Status = 1; // 默认启用状态
            entity.CreateTime = now;
            entity.CreateUser = loginUser;
            entity.UpdateTime = now;
            entity.UpdateUser = loginUser;

            baseRepo.Insert(entity);
        }
    }
}