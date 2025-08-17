using System.Linq.Expressions;
using System.Text.Json;
using antflowcore.constant.enus;
using antflowcore.dto;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.interf.repository;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class OutSideBpmConditionsTemplateService : AFBaseCurdRepositoryService<OutSideBpmConditionsTemplate>,IOutSideBpmConditionsTemplateService
{
    private readonly OutSideBpmBaseService _outSideBpmBaseService;
    private readonly OutSideBpmBusinessPartyService _outSideBpmBusinessPartyService;
    private readonly BpmProcessAppApplicationService _bpmProcessAppApplicationService;
    private readonly UserService _employeeService;
    private readonly BpmnConfService _bpmnConfService;
    private readonly BpmnNodeService _bpmnNodeService;
    private readonly BpmnNodeConditionsConfService _bpmnNodeConditionsConfService;
    private readonly BpmnNodeConditionsParamConfService _bpmnNodeConditionsParamConfService;

    public OutSideBpmConditionsTemplateService(
        OutSideBpmBaseService outSideBpmBaseService,
        OutSideBpmBusinessPartyService outSideBpmBusinessPartyService,
        BpmProcessAppApplicationService bpmProcessAppApplicationService,
        UserService employeeService,
        BpmnConfService bpmnConfService,
        BpmnNodeService bpmnNodeService,
        BpmnNodeConditionsConfService bpmnNodeConditionsConfService,
        BpmnNodeConditionsParamConfService bpmnNodeConditionsParamConfService,
        IFreeSql freeSql) : base(freeSql)
    {
        _outSideBpmBaseService = outSideBpmBaseService;
        _outSideBpmBusinessPartyService = outSideBpmBusinessPartyService;
        _bpmProcessAppApplicationService = bpmProcessAppApplicationService;
        _employeeService = employeeService;
        _bpmnConfService = bpmnConfService;
        _bpmnNodeService = bpmnNodeService;
        _bpmnNodeConditionsConfService = bpmnNodeConditionsConfService;
        _bpmnNodeConditionsParamConfService = bpmnNodeConditionsParamConfService;
    }

    public ResultAndPage<OutSideBpmConditionsTemplateVo> ListPage(PageDto pageDto, OutSideBpmConditionsTemplateVo vo)
    {
        Page<OutSideBpmConditionsTemplateVo> page = PageUtils.GetPageByPageDto<OutSideBpmConditionsTemplateVo>(pageDto);

        // 查询当前登录员工的业务方列表，控制查询权限
        var emplBusinessPartys = _outSideBpmBaseService.GetEmplBusinessPartys(string.Empty,
            AdminPersonnelTypeEnum.ADMIN_PERSONNEL_TYPE_TEMPLATE.PermCode);
        vo.BusinessPartyIds = emplBusinessPartys.Select(x => x.Id).ToList();

        // 分页查询条件模板
        List<OutSideBpmConditionsTemplateVo> outSideBpmConditionsTemplateVos = this.SelectPageList(page, vo);

        if (outSideBpmConditionsTemplateVos == null || !outSideBpmConditionsTemplateVos.Any())
        {
            return PageUtils.GetResultAndPage(page);
        }

        // 批量查询业务方信息
        var businessPartyIds = outSideBpmConditionsTemplateVos.Select(x => x.BusinessPartyId).Distinct().ToList();
        Dictionary<long, OutSideBpmBusinessParty> businessPartyMap = _outSideBpmBusinessPartyService.baseRepo
            .Where(a => businessPartyIds.Contains(a.Id))
            .ToList()
            .ToDictionary(x => x.Id, x => x);

        // 批量查询员工信息
        var userIds = outSideBpmConditionsTemplateVos.Select(x => x.CreateUserId).Where(id => !string.IsNullOrEmpty(id))
            .Distinct().ToList();
        var employeeMap =
            _employeeService.GetEmployeeDetailByIds(userIds)
                .ToDictionary(x => x.Id, x => x);

        foreach (OutSideBpmConditionsTemplateVo item in outSideBpmConditionsTemplateVos)
        {
            if (businessPartyMap.TryGetValue(item.BusinessPartyId, out var party))
            {
                item.BusinessPartyMark = party.BusinessPartyMark;
                item.BusinessPartyName = party.Name;
            }

            if (!string.IsNullOrEmpty(item.CreateUserId) && employeeMap.TryGetValue(item.CreateUserId, out var emp))
            {
                item.CreateUserName = emp.Username;
            }

            if (item.ApplicationId != null && item.ApplicationId != 0)
            {
                var app = _bpmProcessAppApplicationService
                    .baseRepo
                    .Where(a => a.Id == item.ApplicationId)
                    .ToOne();

                if (app != null)
                {
                    item.ApplicationFormCode = app.ProcessKey;
                    item.ApplicationName = app.Title;
                }
            }
        }

        page.Records = outSideBpmConditionsTemplateVos;
        return PageUtils.GetResultAndPage(page);
    }

    List<OutSideBpmConditionsTemplateVo> SelectPageList(Page<OutSideBpmConditionsTemplateVo> page,
        OutSideBpmConditionsTemplateVo vo)
    {
        Expression<Func<OutSideBpmConditionsTemplate, bool>> expression = x => x.IsDel == 0;
        if (vo.BusinessPartyIds != null && vo.BusinessPartyIds.Any())
        {
            expression = expression.And(x => vo.BusinessPartyIds.Contains(x.BusinessPartyId));
        }

        if (!string.IsNullOrEmpty(vo.TemplateMark))
        {
            expression = expression.And(x => x.TemplateMark.Contains(vo.TemplateMark));
        }

        if (!string.IsNullOrEmpty(vo.TemplateName))
        {
            expression = expression.And(x => x.TemplateName.Contains(vo.TemplateName));
        }

        List<OutSideBpmConditionsTemplateVo> outSideBpmConditionsTemplateVos = this.baseRepo
            .Where(expression)
            .ToList<OutSideBpmConditionsTemplateVo>(a => new OutSideBpmConditionsTemplateVo()
            {
                Id = a.Id,
                BusinessPartyId = a.BusinessPartyId,
                TemplateName = a.TemplateName,
                TemplateMark = a.TemplateMark,
                ApplicationId = a.ApplicationId,
                CreateUserId = a.CreateUserId,
            });
        return outSideBpmConditionsTemplateVos;
    }

    public List<OutSideBpmConditionsTemplateVo> SelectConditionListByAppId(int appId)
    {
        var list = this.baseRepo
            .Where(o => o.IsDel == 0 && o.ApplicationId == appId)
            .ToList();

        if (list != null && list.Count > 0)
        {
            return list
                .Select(o => new OutSideBpmConditionsTemplateVo
                {
                    Id = o.Id,
                    ApplicationId = o.ApplicationId,
                    BusinessPartyId = o.BusinessPartyId,
                    TemplateMark = o.TemplateMark,
                    TemplateName = o.TemplateName,
                    Remark = o.Remark,
                    CreateTime = o.CreateTime
                })
                .ToList();
        }

        return new List<OutSideBpmConditionsTemplateVo>();
    }

    public void Edit(OutSideBpmConditionsTemplateVo vo)
    {
        Expression<Func<OutSideBpmConditionsTemplate, bool>> markExpression = t => t.IsDel == 0
            && t.BusinessPartyId == vo.BusinessPartyId
            && t.ApplicationId == vo.ApplicationId
            && t.TemplateMark == vo.TemplateMark;
        if (vo.Id != null && vo.Id != 0)
        {
            markExpression = markExpression.And(x => x.Id != vo.Id);
        }

        // 判断模板标识是否重复
        var existsTemplateMark = this.baseRepo
            .Where(markExpression)
            .Any();

        if (existsTemplateMark)
        {
            throw new AFBizException("条件模板标识重复，编辑失败");
        }

        Expression<Func<OutSideBpmConditionsTemplate, bool>> nameExpression = t => t.IsDel == 0
            && t.BusinessPartyId == vo.BusinessPartyId
            && t.ApplicationId == vo.ApplicationId
            && t.TemplateName == vo.TemplateName;

        if (vo.Id != null && vo.Id != 0)
        {
            nameExpression = nameExpression.And(x => x.Id != vo.Id);
        }

        // 判断模板名称是否重复
        var existsTemplateName = this.baseRepo
            .Where(nameExpression)
            .Any();

        if (existsTemplateName)
        {
            throw new AFBizException("条件模板名称重复，编辑失败");
        }

        var now = DateTime.Now;
        var currentUserId = SecurityUtils.GetLogInEmpIdSafe();
        var currentUserName = SecurityUtils.GetLogInEmpName();

        if (vo.Id != null)
        {
            OutSideBpmConditionsTemplate entity = this.baseRepo.Where(a => a.Id == vo.Id).ToOne();
            if (entity != null)
            {
                // 更新
                entity.TemplateName = vo.TemplateName;
                entity.TemplateMark = vo.TemplateMark;
                entity.Remark = vo.Remark;
                entity.ApplicationId = vo.ApplicationId;
                entity.BusinessPartyId = vo.BusinessPartyId;
                entity.UpdateUser = currentUserName;
                entity.UpdateTime = now;
                this.baseRepo.Update(entity);
            }
        }
        else
        {
            // 新建
            var entity = new OutSideBpmConditionsTemplate
            {
                TemplateName = vo.TemplateName,
                TemplateMark = vo.TemplateMark,
                Remark = vo.Remark,
                ApplicationId = vo.ApplicationId,
                BusinessPartyId = vo.BusinessPartyId,
                IsDel = 0,
                CreateUserId = currentUserId,
                CreateUser = currentUserName,
                CreateTime = now,
                UpdateUser = currentUserName,
                UpdateTime = now
            };

            this.baseRepo.Insert(entity);
        }
    }

    public void Delete(int id)
    {
        OutSideBpmConditionsTemplate outSideBpmConditionsTemplate = this.baseRepo.Where(a => a.Id == id).ToOne();


        OutSideBpmBusinessParty outSideBpmBusinessParty = _outSideBpmBusinessPartyService
            .baseRepo.Where(a => a.Id == outSideBpmConditionsTemplate.BusinessPartyId).ToOne();


        if (TemplateIsUsed(id, outSideBpmBusinessParty))
        {
            throw new AFBizException("审批流程中正使用此条件模板，无法删除");
        }

        this.Frsql
            .Update<OutSideBpmConditionsTemplate>()
            .Set(a => a.IsDel, 1)
            .Where(a => a.Id == id)
            .ExecuteAffrows();
    }

    private bool TemplateIsUsed(int id, OutSideBpmBusinessParty outSideBpmBusinessParty)
    {
        var bpmnConfs = _bpmnConfService.baseRepo
            .Where(conf => conf.BusinessPartyId == outSideBpmBusinessParty.Id && conf.IsDel == 0 &&
                           conf.EffectiveStatus == 1)
            .ToList();

        if (!bpmnConfs.Any())
        {
            var confIds = bpmnConfs.Select(c => c.Id).ToList();
            var nodeTypeList = new List<int>
            {
                (int)NodeTypeEnum.NODE_TYPE_CONDITIONS,
                (int)NodeTypeEnum.NODE_TYPE_OUT_SIDE_CONDITIONS
            };

            var bpmnNodes = _bpmnNodeService.baseRepo
                .Where(node => confIds.Contains(node.ConfId) && nodeTypeList.Contains(node.NodeType))
                .ToList();

            if (bpmnNodes.Any())
            {
                List<long> nodeIds = bpmnNodes.Select(n => n.Id).ToList();

                var bpmnNodeConditionsConfs = _bpmnNodeConditionsConfService
                    .baseRepo
                    .Where(conf => nodeIds.Contains(conf.BpmnNodeId))
                    .ToList();

                if (bpmnNodeConditionsConfs.Any())
                {
                    int conditionTemplatemark = (int)ConditionTypeEnum.CONDITION_TEMPLATEMARK;
                    var conditionIds = bpmnNodeConditionsConfs.Select(c => c.Id).ToList();

                    var paramConfs = _bpmnNodeConditionsParamConfService.baseRepo
                        .Where(param => conditionIds.Contains(param.BpmnNodeConditionsId) &&
                                        param.ConditionParamType == conditionTemplatemark)
                        .ToList();

                    if (paramConfs.Any())
                    {
                        var usedTempList = new List<int>();

                        foreach (var param in paramConfs)
                        {
                            var json = param.ConditionParamJsom;
                            var list = JsonSerializer.Deserialize<List<int>>(json);
                            if (list != null)
                            {
                                usedTempList.AddRange(list);
                            }
                        }

                        if (usedTempList.Contains(id))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
}