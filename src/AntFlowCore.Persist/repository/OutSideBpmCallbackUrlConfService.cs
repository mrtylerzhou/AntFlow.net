using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class OutSideBpmCallbackUrlConfService : IOutSideBpmCallbackUrlConfService
{
    private readonly IUserService _employeeService;
    private readonly IOutSideBpmBusinessPartyRepository _outSideBpmBusinessPartyRepository;
    private readonly IOutSideBpmAdminPersonnelRepository _outSideBpmAdminPersonnelRepository;

    public OutSideBpmCallbackUrlConfService(
        IOutSideBpmCallbackUrlConfRepository repository,
        IUserService employeeService,
        IOutSideBpmBusinessPartyRepository outSideBpmBusinessPartyRepository,
        IOutSideBpmAdminPersonnelRepository outSideBpmAdminPersonnelRepository)
    {
        _repository = repository;
        _employeeService = employeeService;
        _outSideBpmBusinessPartyRepository = outSideBpmBusinessPartyRepository;
        _outSideBpmAdminPersonnelRepository = outSideBpmAdminPersonnelRepository;
    }

    public IOutSideBpmCallbackUrlConfRepository _repository { get; }

    public OutSideBpmCallbackUrlConf GetOutSideBpmCallbackUrlConf(long businessPartyId)
    {
        OutSideBpmCallbackUrlConf outSideBpmCallbackUrlConf =
            _repository.FirstOrDefault(a => a.BusinessPartyId == businessPartyId && a.Status == 1);

        if (outSideBpmCallbackUrlConf == null)
        {
            throw new AFBizException("流程回调URL未配置，方法执行失败");
        }

        return outSideBpmCallbackUrlConf;
    }

    public List<OutSideBpmCallbackUrlConf> SelectListByFormCode(String formCode)
    {
        List<OutSideBpmCallbackUrlConf> confList = _repository
            .Find(a => a.FormCode == formCode && a.Status == 1);
        return confList;
    }

    public OutSideBpmCallbackUrlConfVo Detail(int id)
    {
        OutSideBpmCallbackUrlConf outSideBpmCallbackUrlConf = _repository
            .FirstOrDefault(a => a.Id == id);

        OutSideBpmCallbackUrlConfVo vo = outSideBpmCallbackUrlConf.MapToVo();

        OutSideBpmBusinessParty outSideBpmBusinessParty = _outSideBpmBusinessPartyRepository
            .FirstOrDefault(a => a.Id == vo.BusinessPartyId);

        return RebuildVo(vo, outSideBpmBusinessParty);
    }

    private OutSideBpmCallbackUrlConfVo RebuildVo(
        OutSideBpmCallbackUrlConfVo vo,
        OutSideBpmBusinessParty businessParty)
    {
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

        if (businessParty != null)
        {
            vo.BusinessPartyName = businessParty.Name;
            vo.AccessType = businessParty.Type;
            vo.AccessTypeName = BusinessPartyTypeEnum.GetDescByCode(businessParty.Type);

            List<OutSideBpmAdminPersonnel> outSideBpmAdminPersonnels = _outSideBpmAdminPersonnelRepository
                .Find(p => p.BusinessPartyId == businessParty.Id &&
                            p.Type == AdminPersonnelTypeEnum.ADMIN_PERSONNEL_TYPE_INTERFACE.Code);

            if (outSideBpmAdminPersonnels != null && outSideBpmAdminPersonnels.Any())
            {
                List<DetailedUser> employees = _employeeService.GetEmployeeDetailByIds(outSideBpmAdminPersonnels
                    .Select(a => a.EmployeeId)
                    .ToList());

                vo.InterfaceAdmins = employees
                    .Select(emp => new Employee
                    {
                        Id = emp.Id,
                        Username = emp.UserName
                    })
                    .ToList();
            }
        }

        return vo;
    }

    public void Edit(OutSideBpmCallbackUrlConfVo vo)
    {
        if (vo.Id is null or 0)
        {
            var count = _repository
                .Count(x => x.BusinessPartyId == vo.BusinessPartyId && x.ApplicationId == vo.ApplicationId);

            if (count > 0)
            {
               // throw new AFBizException("一个业务方只能配置一条接口回调数据");
            }
        }

        var now = DateTime.Now;
        var loginUser = SecurityUtils.GetLogInEmpName();

        OutSideBpmCallbackUrlConf? entity = vo.Id != null
            ? _repository.FirstOrDefault(a => a.Id == vo.Id)
            : null;

        if (entity != null)
        {
            vo.CopyToEntity(entity);
            entity.UpdateTime = now;
            entity.UpdateUser = loginUser;
            _repository.Update(entity);
        }
        else
        {
            entity = new OutSideBpmCallbackUrlConf();
            vo.CopyToEntity(entity);
            entity.Status = 1;
            entity.CreateTime = now;
            entity.CreateUser = loginUser;
            entity.UpdateTime = now;
            entity.UpdateUser = loginUser;

            _repository.Add(entity);
        }
    }
}
