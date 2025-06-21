using antflowcore.constant.enus;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class OutSideBpmCallbackUrlConfService : AFBaseCurdRepositoryService<OutSideBpmCallbackUrlConf>
{
   
    private readonly OutSideBpmAdminPersonnelService _outSideBpmAdminPersonnelService;
    private readonly UserService _employeeService;

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
            throw new AFBizException("流程回调URL未配置，方法执行失败");
        }

        return outSideBpmCallbackUrlConf;
    }

    public List<OutSideBpmCallbackUrlConf> SelectListByFormCode(String formCode)
    {
        List<OutSideBpmCallbackUrlConf> confList = this.baseRepo
            .Where(a => a.FormCode == formCode && a.Status == 1)
            .ToList();
        return confList;
    }

    public OutSideBpmCallbackUrlConfVo Detail(int id)
    {
        OutSideBpmCallbackUrlConf outSideBpmCallbackUrlConf = this.baseRepo
            .Where(a => a.Id == id)
            .ToOne();

        OutSideBpmCallbackUrlConfVo vo = outSideBpmCallbackUrlConf.MapToVo();

        OutSideBpmBusinessPartyService outSideBpmBusinessPartyService = ServiceProviderUtils.GetService<OutSideBpmBusinessPartyService>();
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
        // 映射状态名称
        if (vo.Status != null)
        {
            if (vo.Status == 1)
            {
                vo.StatusName = "启用";
            }
            else if (vo.Status == 2)
            {
                vo.StatusName = "封存";
            }
        }

        // 映射业务方信息
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
                    .Select(emp => new Employee
                    {
                        Id = emp.Id,
                        Username = emp.Username
                    })
                    .ToList();
            }
        }

        return vo;
    }

    public void Edit(OutSideBpmCallbackUrlConfVo vo)
    {
        // 如果是新增，判断该业务方是否已存在记录（一个业务方只能有一条配置）
        if (vo.Id is null or 0)
        {
            var count = this.baseRepo
                .Where(x => x.BusinessPartyId == vo.BusinessPartyId&&x.ApplicationId == vo.ApplicationId)
                .Count();

            if (count > 0)
            {
                throw new AFBizException("一个业务方只能配置一条接口回调数据");
            }
        }

        var now = DateTime.Now;
        var loginUser = SecurityUtils.GetLogInEmpName();

        OutSideBpmCallbackUrlConf? entity = vo.Id != null
            ? this.baseRepo.Where(a => a.Id == vo.Id).ToOne()
            : null;

        if (entity != null)
        {
            // 更新
            vo.CopyToEntity(entity);
            entity.UpdateTime = now;
            entity.UpdateUser = loginUser;
            this.baseRepo.Update(entity);
        }
        else
        {
            // 新增
            entity = new OutSideBpmCallbackUrlConf();
            vo.CopyToEntity(entity);
            entity.Status = 1; // 默认启用
            entity.CreateTime = now;
            entity.CreateUser = loginUser;
            entity.UpdateTime = now;
            entity.UpdateUser = loginUser;

            this.baseRepo.Insert(entity);
        }
    }
}