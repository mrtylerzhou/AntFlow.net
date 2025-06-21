using System.Linq.Expressions;
using antflowcore.constant.enus;
using antflowcore.dto;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using FreeSql.Internal.Model;

namespace antflowcore.service.repository;

public class OutSideBpmBusinessPartyService : AFBaseCurdRepositoryService<OutSideBpmBusinessParty>
{
    private readonly OutSideBpmAdminPersonnelService _outSideBpmAdminPersonnelService;
    private readonly OutSideBpmCallbackUrlConfService _outSideBpmCallbackUrlConfService;
    private readonly UserService _employeeService;

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
        // 获取分页对象
        Page<OutSideBpmBusinessPartyVo> page = PageUtils.GetPageByPageDto<OutSideBpmBusinessPartyVo>(pageDto);

        // 查询结果
        List<OutSideBpmBusinessPartyVo> records = this.SelectPageList(page);

        // 如果结果为空，返回空的分页列表
        if (records == null || !records.Any())
        {
            return PageUtils.GetResultAndPage(page);
        }

        // 查询所有相关的业务方管理员
        List<long?> businessPartyIds = records
            .Select(r => r.Id)
            .Distinct()
            .ToList();

        List<OutSideBpmAdminPersonnel> outSideBpmAdminPersonnels = _outSideBpmAdminPersonnelService.baseRepo
            .Where(a => businessPartyIds.Contains(a.Id))
            .ToList();


        // 如果没有找到管理员人员，直接返回结果
        if (outSideBpmAdminPersonnels == null || !outSideBpmAdminPersonnels.Any())
        {
            page.Records = records;
            return PageUtils.GetResultAndPage(page);
        }

        // 设置结果
        page.Records = (records
            .Select(record => ReBuildVo(record, outSideBpmAdminPersonnels, false))
            .ToList());

        return PageUtils.GetResultAndPage(page);
    }

    public List<OutSideBpmBusinessPartyVo> SelectPageList(Page<OutSideBpmBusinessPartyVo> page)
    {
        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        List<OutSideBpmBusinessPartyVo> result = this.baseRepo
            .Where(a => a.IsDel == 0)
            .OrderByDescending(a => a.CreateTime)
            .Page(basePagingInfo)
            .ToList<OutSideBpmBusinessPartyVo>(a => new OutSideBpmBusinessPartyVo
            {
                Id = a.Id,
                BusinessPartyMark = a.BusinessPartyMark,
                Name = a.Name,
                Type = a.Type,
                TypeName = a.Type == 1 ? "嵌入式" : "调入式",
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
        // 设置 typeName
        if (outSideBpmBusinessPartyVo.Type != null)
        {
            outSideBpmBusinessPartyVo.TypeName =
                BusinessPartyTypeEnum.GetDescByCode(outSideBpmBusinessPartyVo.Type.Value);
        }

        // 获取员工详情
        var employeeIds = outSideBpmAdminPersonnels
            .Select(p => p.EmployeeId)
            .Distinct()
            .ToList();

        var employeeMap = _employeeService.GetEmployeeDetailByIds(employeeIds)
            .ToDictionary(e => e.Id, e => e);

        // 当前业务方的人员
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
                // 列表数据
                var list = bpmAdminPersonnels.Select(p => new BaseIdTranStruVo
                {
                    Id = p.EmployeeId,
                    Name = !string.IsNullOrEmpty(p.EmployeeName)
                        ? p.EmployeeName
                        : (employeeMap.TryGetValue(p.EmployeeId, out var emp) ? emp.Username : string.Empty)
                }).ToList();

                // 设置到对应属性上（如 AdminList、AuditorList 等）
                SetProperty(outSideBpmBusinessPartyVo, typeEnum.ListField, list);

                // 设置ID列表
                var idList = bpmAdminPersonnels.Select(p => p.EmployeeId).Distinct().ToList();
                SetProperty(outSideBpmBusinessPartyVo, typeEnum.IdsField, idList);
            }
            else
            {
                // 拼接用户名
                var nameStr = string.Join(",", bpmAdminPersonnels
                    .Select(p => employeeMap.TryGetValue(p.EmployeeId, out var emp) ? emp.Username : string.Empty));

                SetProperty(outSideBpmBusinessPartyVo, typeEnum.StrField, nameStr);
            }
        }

        return outSideBpmBusinessPartyVo;
    }

    public OutSideBpmBusinessPartyVo Detail(int id)
    {
        OutSideBpmBusinessParty outSideBpmBusinessParty = this.baseRepo.Where(a => a.Id == id).ToOne();

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
        // 检查数据是否重复
        if (this.CheckData(vo) > 0)
        {
            throw new AFBizException("业务方标识或业务方名称重复");
        }

        OutSideBpmBusinessParty outSideBpmBusinessParty = this.baseRepo
            .Where(a => a.Id == vo.Id).ToOne();

        if (outSideBpmBusinessParty != null)
        {
            vo.CopyTo(outSideBpmBusinessParty);
            outSideBpmBusinessParty.UpdateTime = DateTime.Now;
            outSideBpmBusinessParty.UpdateUser = SecurityUtils.GetLogInEmpName();
            this.baseRepo.Update(outSideBpmBusinessParty);
        }
        else
        {
            outSideBpmBusinessParty = vo.MapToEntity();
            outSideBpmBusinessParty.IsDel = 0;
            outSideBpmBusinessParty.CreateTime = DateTime.Now;
            outSideBpmBusinessParty.CreateUser = SecurityUtils.GetLogInEmpName();
            outSideBpmBusinessParty.UpdateTime = DateTime.Now;
            outSideBpmBusinessParty.UpdateUser = SecurityUtils.GetLogInEmpName();
            this.baseRepo.Insert(outSideBpmBusinessParty);
        }

        var id = outSideBpmBusinessParty.Id;

        if (id != null && id > 0)
        {
            // 删除旧的管理员信息
            _outSideBpmAdminPersonnelService.baseRepo.Delete(a => a.BusinessPartyId == id);

            // 添加管理员
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

                    _outSideBpmAdminPersonnelService.baseRepo.Insert(personnels);
                }
            }

            // 添加回调配置（如果没有）
            var count = _outSideBpmCallbackUrlConfService
                .baseRepo
                .Where(a => a.BusinessPartyId == id)
                .Count();

            if (count == 0)
            {
                var conf = new OutSideBpmCallbackUrlConf
                {
                    BusinessPartyId = id
                };
                _outSideBpmCallbackUrlConfService.baseRepo.Insert(conf);
            }
        }
    }

    long CheckData(OutSideBpmBusinessPartyVo vo)
    {
        Expression<Func<OutSideBpmBusinessParty, bool>> expression = a =>
            a.BusinessPartyMark == vo.BusinessPartyMark || a.Name == vo.Name;
        if (vo.Id != null && vo.Id > 0)
        {
            expression = expression.And(a => a.Id == vo.Id);
        }

        long count = this.baseRepo.Where(expression).Count();
        return count;
    }

    // 工具方法：动态设置属性
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