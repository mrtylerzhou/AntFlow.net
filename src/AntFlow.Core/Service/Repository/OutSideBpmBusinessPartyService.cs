using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using FreeSql.Internal.Model;
using System.Linq.Expressions;
using System.Reflection;

namespace AntFlow.Core.Service.Repository;

public class OutSideBpmBusinessPartyService : AFBaseCurdRepositoryService<OutSideBpmBusinessParty>,
    IOutSideBpmBusinessPartyService
{
    private readonly UserService _employeeService;
    private readonly OutSideBpmAdminPersonnelService _outSideBpmAdminPersonnelService;
    private readonly OutSideBpmCallbackUrlConfService _outSideBpmCallbackUrlConfService;

    public OutSideBpmBusinessPartyService(
        OutSideBpmAdminPersonnelService outSideBpmAdminPersonnelService,
        OutSideBpmCallbackUrlConfService outSideBpmCallbackUrlConfService,
        UserService employeeService,
        IFreeSql freeSql) : base(freeSql)
    {
        _outSideBpmAdminPersonnelService = outSideBpmAdminPersonnelService;
        _outSideBpmCallbackUrlConfService = outSideBpmCallbackUrlConfService;
        _employeeService = employeeService;
    }

    public ResultAndPage<OutSideBpmBusinessPartyVo> ListPage(PageDto pageDto, OutSideBpmBusinessPartyVo vo)
    {
        // ??????????
        Page<OutSideBpmBusinessPartyVo> page = PageUtils.GetPageByPageDto<OutSideBpmBusinessPartyVo>(pageDto);

        // ??????
        List<OutSideBpmBusinessPartyVo> records = SelectPageList(page);

        // ???????????????????งา?
        if (records == null || !records.Any())
        {
            return PageUtils.GetResultAndPage(page);
        }

        // ???????????????????
        List<long?> businessPartyIds = records
            .Select(r => r.Id)
            .Distinct()
            .ToList();

        List<OutSideBpmAdminPersonnel> outSideBpmAdminPersonnels = _outSideBpmAdminPersonnelService.baseRepo
            .Where(a => businessPartyIds.Contains(a.Id))
            .ToList();


        // ???????????????????????????
        if (outSideBpmAdminPersonnels == null || !outSideBpmAdminPersonnels.Any())
        {
            page.Records = records;
            return PageUtils.GetResultAndPage(page);
        }

        // ???y??
        page.Records = records
            .Select(record => ReBuildVo(record, outSideBpmAdminPersonnels, false))
            .ToList();

        return PageUtils.GetResultAndPage(page);
    }

    public List<OutSideBpmBusinessPartyVo> SelectPageList(Page<OutSideBpmBusinessPartyVo> page)
    {
        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        List<OutSideBpmBusinessPartyVo> result = baseRepo
            .Where(a => a.IsDel == 0)
            .OrderByDescending(a => a.CreateTime)
            .Page(basePagingInfo)
            .ToList<OutSideBpmBusinessPartyVo>(a => new OutSideBpmBusinessPartyVo
            {
                Id = a.Id,
                BusinessPartyMark = a.BusinessPartyMark,
                Name = a.Name,
                Type = a.Type,
                TypeName = a.Type == 1 ? "????" : "?????",
                IsDel = a.IsDel,
                Remark = a.Remark,
                CreateTime = a.CreateTime
            });
        page.Total = (int)basePagingInfo.Count;
        return result;
    }

    private OutSideBpmBusinessPartyVo ReBuildVo(
        OutSideBpmBusinessPartyVo outSideBpmBusinessPartyVo,
        List<OutSideBpmAdminPersonnel> outSideBpmAdminPersonnels,
        bool isDetail)
    {
        // ???? typeName
        if (outSideBpmBusinessPartyVo.Type != null)
        {
            outSideBpmBusinessPartyVo.TypeName =
                BusinessPartyTypeEnum.GetDescByCode(outSideBpmBusinessPartyVo.Type.Value);
        }

        // ??????????
        List<string>? employeeIds = outSideBpmAdminPersonnels
            .Select(p => p.EmployeeId)
            .Distinct()
            .ToList();

        Dictionary<string, Employee>? employeeMap = _employeeService.GetEmployeeDetailByIds(employeeIds)
            .ToDictionary(e => e.Id, e => e);

        // ???????????
        List<OutSideBpmAdminPersonnel> adminPersonnels = outSideBpmAdminPersonnels
            .Where(p => p.BusinessPartyId == outSideBpmBusinessPartyVo.Id)
            .ToList();

        if (!adminPersonnels.Any())
        {
            return outSideBpmBusinessPartyVo;
        }

        foreach (AdminPersonnelTypeEnum typeEnum in Enum.GetValues(typeof(AdminPersonnelTypeEnum))
                     .Cast<AdminPersonnelTypeEnum>())
        {
            List<OutSideBpmAdminPersonnel> bpmAdminPersonnels = adminPersonnels
                .Where(a => a.Type == typeEnum.Code)
                .ToList();

            if (isDetail)
            {
                // ?งา?????
                List<BaseIdTranStruVo> list = bpmAdminPersonnels.Select(p => new BaseIdTranStruVo
                {
                    Id = p.EmployeeId,
                    Name = !string.IsNullOrEmpty(p.EmployeeName)
                        ? p.EmployeeName
                        : employeeMap.TryGetValue(p.EmployeeId, out Employee? emp)
                            ? emp.Username
                            : string.Empty
                }).ToList();

                // ????????????????? AdminList??AuditorList ???
                SetProperty(outSideBpmBusinessPartyVo, typeEnum.ListField, list);

                // ????ID?งา?
                List<string> idList = bpmAdminPersonnels.Select(p => p.EmployeeId).Distinct().ToList();
                SetProperty(outSideBpmBusinessPartyVo, typeEnum.IdsField, idList);
            }
            else
            {
                // ????????
                string nameStr = string.Join(",", bpmAdminPersonnels
                    .Select(p =>
                        employeeMap.TryGetValue(p.EmployeeId, out Employee? emp) ? emp.Username : string.Empty));

                SetProperty(outSideBpmBusinessPartyVo, typeEnum.StrField, nameStr);
            }
        }

        return outSideBpmBusinessPartyVo;
    }

    public OutSideBpmBusinessPartyVo Detail(int id)
    {
        OutSideBpmBusinessParty outSideBpmBusinessParty = baseRepo.Where(a => a.Id == id).ToOne();

        OutSideBpmBusinessPartyVo vo = outSideBpmBusinessParty.MapToVo();


        //querying all associated business party admin
        List<OutSideBpmAdminPersonnel> outSideBpmAdminPersonnels = _outSideBpmAdminPersonnelService
            .baseRepo
            .Where(a => a.BusinessPartyId == outSideBpmBusinessParty.Id)
            .ToList();


        //if the result is empty then return
        if (outSideBpmAdminPersonnels == null || !outSideBpmAdminPersonnels.Any())
        {
            return vo;
        }

        return ReBuildVo(vo, outSideBpmAdminPersonnels, true);
    }

    public void Edit(OutSideBpmBusinessPartyVo vo)
    {
        // ?????????????
        if (CheckData(vo) > 0)
        {
            throw new AFBizException("??????????????????");
        }

        OutSideBpmBusinessParty outSideBpmBusinessParty = baseRepo
            .Where(a => a.Id == vo.Id).ToOne();

        if (outSideBpmBusinessParty != null)
        {
            vo.CopyTo(outSideBpmBusinessParty);
            outSideBpmBusinessParty.UpdateTime = DateTime.Now;
            outSideBpmBusinessParty.UpdateUser = SecurityUtils.GetLogInEmpName();
            baseRepo.Update(outSideBpmBusinessParty);
        }
        else
        {
            outSideBpmBusinessParty = vo.MapToEntity();
            outSideBpmBusinessParty.IsDel = 0;
            outSideBpmBusinessParty.CreateTime = DateTime.Now;
            outSideBpmBusinessParty.CreateUser = SecurityUtils.GetLogInEmpName();
            outSideBpmBusinessParty.UpdateTime = DateTime.Now;
            outSideBpmBusinessParty.UpdateUser = SecurityUtils.GetLogInEmpName();
            baseRepo.Insert(outSideBpmBusinessParty);
        }

        long id = outSideBpmBusinessParty.Id;

        if (id != null && id > 0)
        {
            // ?????????????
            _outSideBpmAdminPersonnelService.baseRepo.Delete(a => a.BusinessPartyId == id);

            // ???????
            foreach (AdminPersonnelTypeEnum typeEnum in AdminPersonnelTypeEnum.Values())
            {
                object? property = GetProperty(vo, typeEnum.IdsField);
                if (property is List<string> ids)
                {
                    List<OutSideBpmAdminPersonnel>? personnels = ids.Select(o => new OutSideBpmAdminPersonnel
                    {
                        BusinessPartyId = id,
                        EmployeeId = o,
                        Type = typeEnum.Code,
                        CreateTime = DateTime.Now,
                        CreateUser = SecurityUtils.GetLogInEmpName(),
                        UpdateTime = DateTime.Now,
                        UpdateUser = SecurityUtils.GetLogInEmpName()
                    }).ToList();

                    _outSideBpmAdminPersonnelService.baseRepo.Insert(personnels);
                }
            }

            // ???????????????งต?
            long count = _outSideBpmCallbackUrlConfService
                .baseRepo
                .Where(a => a.BusinessPartyId == id)
                .Count();

            if (count == 0)
            {
                OutSideBpmCallbackUrlConf? conf = new() { BusinessPartyId = id };
                _outSideBpmCallbackUrlConfService.baseRepo.Insert(conf);
            }
        }
    }

    private long CheckData(OutSideBpmBusinessPartyVo vo)
    {
        Expression<Func<OutSideBpmBusinessParty, bool>> expression = a =>
            a.BusinessPartyMark == vo.BusinessPartyMark || a.Name == vo.Name;
        if (vo.Id != null && vo.Id > 0)
        {
            expression = expression.And(a => a.Id == vo.Id);
        }

        long count = baseRepo.Where(expression).Count();
        return count;
    }

    // ????????????????????
    private void SetProperty(object obj, string propertyName, object value)
    {
        PropertyInfo? prop = obj.GetType().GetProperty(propertyName);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(obj, value);
        }
    }

    private object GetProperty(object obj, string propertyName)
    {
        PropertyInfo? prop = obj.GetType().GetProperty(propertyName);
        if (prop != null && prop.CanWrite)
        {
            return prop.GetValue(obj);
        }

        throw new AFBizException("??????????????!");
    }
}