using System.Linq.Expressions;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class OutSideBpmBusinessPartyService : IOutSideBpmBusinessPartyService
{
    private readonly IOutSideBpmAdminPersonnelService _outSideBpmAdminPersonnelService;
    private readonly IOutSideBpmCallbackUrlConfService _outSideBpmCallbackUrlConfService;
    private readonly IUserService _employeeService;

    public OutSideBpmBusinessPartyService(
        IOutSideBpmBusinessPartyRepository repository,
        IOutSideBpmAdminPersonnelService outSideBpmAdminPersonnelService,
        IOutSideBpmCallbackUrlConfService outSideBpmCallbackUrlConfService,
        IUserService employeeService
    )
    {
        _repository = repository;
        _outSideBpmAdminPersonnelService = outSideBpmAdminPersonnelService;
        _outSideBpmCallbackUrlConfService = outSideBpmCallbackUrlConfService;
        _employeeService = employeeService;
    }

    public IOutSideBpmBusinessPartyRepository _repository { get; }

    public ResultAndPage<OutSideBpmBusinessPartyVo> ListPage(PageDto pageDto, OutSideBpmBusinessPartyVo vo)
    {
        Page<OutSideBpmBusinessPartyVo> page = PageUtils.GetPageByPageDto<OutSideBpmBusinessPartyVo>(pageDto);
        List<OutSideBpmBusinessPartyVo> records = this.SelectPageList(page);

        if (records == null || !records.Any())
        {
            return PageUtils.GetResultAndPage(page);
        }

        List<long?> businessPartyIds = records
            .Select(r => r.Id)
            .Distinct()
            .ToList();

        List<OutSideBpmAdminPersonnel> outSideBpmAdminPersonnels = _outSideBpmAdminPersonnelService._repository
            .Find(a => businessPartyIds.Contains(a.Id));

        if (outSideBpmAdminPersonnels == null || !outSideBpmAdminPersonnels.Any())
        {
            page.Records = records;
            return PageUtils.GetResultAndPage(page);
        }

        page.Records = (records
            .Select(record => ReBuildVo(record, outSideBpmAdminPersonnels, false))
            .ToList());

        return PageUtils.GetResultAndPage(page);
    }

    public List<OutSideBpmBusinessPartyVo> SelectPageList(Page<OutSideBpmBusinessPartyVo> page)
    {
        PagingInfo pagingInfo = page.ToPagingInfo();
        

        List<OutSideBpmBusinessPartyVo> result = _repository.ListPage(a => a.IsDel == 0, pagingInfo)
            .Select(a => new OutSideBpmBusinessPartyVo
            {
                Id = a.Id,
                BusinessPartyMark = a.BusinessPartyMark,
                Name = a.Name,
                Type = a.Type,
                TypeName = a.Type == 1 ? "嵌入式" : "调入式",
                IsDel = a.IsDel,
                Remark = a.Remark,
                CreateTime = a.CreateTime
            }).ToList();

        page.Total = (int)pagingInfo.Count;
        return result;
    }

    private OutSideBpmBusinessPartyVo ReBuildVo(
        OutSideBpmBusinessPartyVo outSideBpmBusinessPartyVo,
        List<OutSideBpmAdminPersonnel> outSideBpmAdminPersonnels,
        bool isDetail)
    {
        if (outSideBpmBusinessPartyVo.Type != null)
        {
            outSideBpmBusinessPartyVo.TypeName =
                BusinessPartyTypeEnum.GetDescByCode(outSideBpmBusinessPartyVo.Type.Value);
        }

        var employeeIds = outSideBpmAdminPersonnels
            .Select(p => p.EmployeeId)
            .Distinct()
            .ToList();

        var employeeMap = _employeeService.GetEmployeeDetailByIds(employeeIds)
            .ToDictionary(e => e.Id, e => e);

        List<OutSideBpmAdminPersonnel> adminPersonnels = outSideBpmAdminPersonnels
            .Where(p => p.BusinessPartyId == outSideBpmBusinessPartyVo.Id)
            .ToList();

        if (!adminPersonnels.Any())
            return outSideBpmBusinessPartyVo;

        foreach (AdminPersonnelTypeEnum typeEnum in Enum.GetValues(typeof(AdminPersonnelTypeEnum))
                     .Cast<AdminPersonnelTypeEnum>())
        {
            List<OutSideBpmAdminPersonnel> bpmAdminPersonnels = adminPersonnels
                .Where(a => a.Type == typeEnum.Code)
                .ToList();

            if (isDetail)
            {
                var list = bpmAdminPersonnels.Select(p => new BaseIdTranStruVo
                {
                    Id = p.EmployeeId,
                    Name = !string.IsNullOrEmpty(p.EmployeeName)
                        ? p.EmployeeName
                        : (employeeMap.TryGetValue(p.EmployeeId, out var emp) ? emp.UserName : string.Empty)
                }).ToList();

                SetProperty(outSideBpmBusinessPartyVo, typeEnum.ListField, list);

                var idList = bpmAdminPersonnels.Select(p => p.EmployeeId).Distinct().ToList();
                SetProperty(outSideBpmBusinessPartyVo, typeEnum.IdsField, idList);
            }
            else
            {
                var nameStr = string.Join(",", bpmAdminPersonnels
                    .Select(p => employeeMap.TryGetValue(p.EmployeeId, out var emp) ? emp.UserName : string.Empty));

                SetProperty(outSideBpmBusinessPartyVo, typeEnum.StrField, nameStr);
            }
        }

        return outSideBpmBusinessPartyVo;
    }

    public OutSideBpmBusinessPartyVo Detail(int id)
    {
        OutSideBpmBusinessParty outSideBpmBusinessParty = _repository.Find(a => a.Id == id).FirstOrDefault() ?? new OutSideBpmBusinessParty();

        OutSideBpmBusinessPartyVo vo = outSideBpmBusinessParty.MapToVo();

        List<OutSideBpmAdminPersonnel> outSideBpmAdminPersonnels = _outSideBpmAdminPersonnelService._repository
            .Find(a => a.BusinessPartyId == outSideBpmBusinessParty.Id);

        if (outSideBpmAdminPersonnels == null || !outSideBpmAdminPersonnels.Any())
        {
            return vo;
        }

        return ReBuildVo(vo, outSideBpmAdminPersonnels, true);
    }

    public void Edit(OutSideBpmBusinessPartyVo vo)
    {
        if (this.CheckData(vo) > 0)
        {
        }

        OutSideBpmBusinessParty? outSideBpmBusinessParty = _repository.Find(a => a.Id == vo.Id).FirstOrDefault();

        if (outSideBpmBusinessParty != null)
        {
            vo.CopyTo(outSideBpmBusinessParty);
            outSideBpmBusinessParty.UpdateTime = DateTime.Now;
            outSideBpmBusinessParty.UpdateUser = SecurityUtils.GetLogInEmpName();
            _repository.Update(outSideBpmBusinessParty);
        }
        else
        {
            outSideBpmBusinessParty = vo.MapToEntity();
            outSideBpmBusinessParty.IsDel = 0;
            outSideBpmBusinessParty.CreateTime = DateTime.Now;
            outSideBpmBusinessParty.CreateUser = SecurityUtils.GetLogInEmpName();
            outSideBpmBusinessParty.UpdateTime = DateTime.Now;
            outSideBpmBusinessParty.UpdateUser = SecurityUtils.GetLogInEmpName();
            _repository.Add(outSideBpmBusinessParty);
        }

        var id = outSideBpmBusinessParty.Id;

        if (id != null && id > 0)
        {
            _outSideBpmAdminPersonnelService._repository.RemoveRange(
                _outSideBpmAdminPersonnelService._repository.Find(a => a.BusinessPartyId == id));

            foreach (AdminPersonnelTypeEnum typeEnum in AdminPersonnelTypeEnum.Values())
            {
                var property = this.GetProperty(vo, typeEnum.IdsField);
                if (property is List<string> ids)
                {
                    var personnels = ids.Select(o => new OutSideBpmAdminPersonnel
                    {
                        BusinessPartyId = id,
                        EmployeeId = o,
                        Type = typeEnum.Code,
                        CreateTime = DateTime.Now,
                        CreateUser = SecurityUtils.GetLogInEmpName(),
                        UpdateTime = DateTime.Now,
                        UpdateUser = SecurityUtils.GetLogInEmpName()
                    }).ToList();

                    _outSideBpmAdminPersonnelService._repository.AddRange(personnels);
                }
            }

            var count = _outSideBpmCallbackUrlConfService
                ._repository
                .Count(a => a.BusinessPartyId == id);

            if (count == 0)
            {
                var conf = new OutSideBpmCallbackUrlConf
                {
                    BusinessPartyId = id
                };
                _outSideBpmCallbackUrlConfService._repository.Add(conf);
            }
        }
    }

    public long CheckData(OutSideBpmBusinessPartyVo vo)
    {
        Expression<Func<OutSideBpmBusinessParty, bool>> expression = a =>
            a.BusinessPartyMark == vo.BusinessPartyMark || a.Name == vo.Name;
        if (vo.Id != null && vo.Id > 0)
        {
            expression = expression.And(a => a.Id == vo.Id);
        }

        return _repository.Count(expression);
    }

    private void SetProperty(object obj, string propertyName, object value)
    {
        var prop = obj.GetType().GetProperty(propertyName);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(obj, value);
        }
    }

    private object GetProperty(object obj, string propertyName)
    {
        var prop = obj.GetType().GetProperty(propertyName);
        if (prop != null && prop.CanWrite)
        {
            return prop.GetValue(obj);
        }

        throw new AFBizException("获取属性方法错误!");
    }
}
