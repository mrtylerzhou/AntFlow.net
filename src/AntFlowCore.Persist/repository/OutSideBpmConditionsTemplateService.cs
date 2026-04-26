using System.Linq.Expressions;
using System.Text.Json;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class OutSideBpmConditionsTemplateService : IOutSideBpmConditionsTemplateService
{
    private readonly IOutSideBpmBaseService _outSideBpmBaseService;
    private readonly IOutSideBpmBusinessPartyRepository _outSideBpmBusinessPartyRepository;
    private readonly IBpmProcessAppApplicationRepository _bpmProcessAppApplicationRepository;
    private readonly IUserService _employeeService;
    private readonly IBpmnConfRepository _bpmnConfRepository;
    private readonly IBpmnNodeService _bpmnNodeService;
    private readonly IBpmnNodeConditionsConfRepository _bpmnNodeConditionsConfRepository;
    private readonly IBpmnNodeConditionsParamConfRepository _bpmnNodeConditionsParamConfRepository;

    public OutSideBpmConditionsTemplateService(
        IOutSideBpmConditionsTemplateRepository repository,
        IOutSideBpmBaseService outSideBpmBaseService,
        IOutSideBpmBusinessPartyRepository outSideBpmBusinessPartyRepository,
        IBpmProcessAppApplicationRepository bpmProcessAppApplicationRepository,
        IUserService employeeService,
        IBpmnConfRepository bpmnConfRepository,
        IBpmnNodeService bpmnNodeService,
        IBpmnNodeConditionsConfRepository bpmnNodeConditionsConfRepository,
        IBpmnNodeConditionsParamConfRepository bpmnNodeConditionsParamConfRepository)
    {
        _repository = repository;
        _outSideBpmBaseService = outSideBpmBaseService;
        _outSideBpmBusinessPartyRepository = outSideBpmBusinessPartyRepository;
        _bpmProcessAppApplicationRepository = bpmProcessAppApplicationRepository;
        _employeeService = employeeService;
        _bpmnConfRepository = bpmnConfRepository;
        _bpmnNodeService = bpmnNodeService;
        _bpmnNodeConditionsConfRepository = bpmnNodeConditionsConfRepository;
        _bpmnNodeConditionsParamConfRepository = bpmnNodeConditionsParamConfRepository;
    }

    public IOutSideBpmConditionsTemplateRepository _repository { get; }

    public ResultAndPage<OutSideBpmConditionsTemplateVo> ListPage(PageDto pageDto, OutSideBpmConditionsTemplateVo vo)
    {
        Page<OutSideBpmConditionsTemplateVo> page = PageUtils.GetPageByPageDto<OutSideBpmConditionsTemplateVo>(pageDto);

        var emplBusinessPartys = _outSideBpmBaseService.GetEmplBusinessPartys(string.Empty,
            AdminPersonnelTypeEnum.ADMIN_PERSONNEL_TYPE_TEMPLATE.PermCode);
        vo.BusinessPartyIds = emplBusinessPartys.Select(x => x.Id).ToList();

        List<OutSideBpmConditionsTemplateVo> outSideBpmConditionsTemplateVos = this.SelectPageList(page, vo);

        if (outSideBpmConditionsTemplateVos == null || !outSideBpmConditionsTemplateVos.Any())
        {
            return PageUtils.GetResultAndPage(page);
        }

        var businessPartyIds = outSideBpmConditionsTemplateVos.Select(x => x.BusinessPartyId).Distinct().ToList();
        Dictionary<long, OutSideBpmBusinessParty> businessPartyMap = _outSideBpmBusinessPartyRepository
            .Find(a => businessPartyIds.Contains(a.Id))
            .ToDictionary(x => x.Id, x => x);

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
                item.CreateUserName = emp.UserName;
            }

            if (item.ApplicationId != null && item.ApplicationId != 0)
            {
                var app = _bpmProcessAppApplicationRepository
                    .FirstOrDefault(a => a.Id == item.ApplicationId);

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

        List<OutSideBpmConditionsTemplateVo> outSideBpmConditionsTemplateVos = _repository
            .Find(expression)
            .Select(a => new OutSideBpmConditionsTemplateVo()
            {
                Id = a.Id,
                BusinessPartyId = a.BusinessPartyId,
                TemplateName = a.TemplateName,
                TemplateMark = a.TemplateMark,
                ApplicationId = a.ApplicationId,
                CreateUserId = a.CreateUserId,
            })
            .ToList();
        return outSideBpmConditionsTemplateVos;
    }

    public List<OutSideBpmConditionsTemplateVo> SelectConditionListByAppId(int appId)
    {
        var list = _repository
            .Find(o => o.IsDel == 0 && o.ApplicationId == appId);

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

        var existsTemplateMark = _repository
            .Any(markExpression);

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

        var existsTemplateName = _repository
            .Any(nameExpression);

        if (existsTemplateName)
        {
            throw new AFBizException("条件模板名称重复，编辑失败");
        }

        var now = DateTime.Now;
        var currentUserId = SecurityUtils.GetLogInEmpIdSafe();
        var currentUserName = SecurityUtils.GetLogInEmpName();

        if (vo.Id != null)
        {
            OutSideBpmConditionsTemplate entity = _repository.FirstOrDefault(a => a.Id == vo.Id);
            if (entity != null)
            {
                entity.TemplateName = vo.TemplateName;
                entity.TemplateMark = vo.TemplateMark;
                entity.Remark = vo.Remark;
                entity.ApplicationId = vo.ApplicationId;
                entity.BusinessPartyId = vo.BusinessPartyId;
                entity.UpdateUser = currentUserName;
                entity.UpdateTime = now;
                _repository.Update(entity);
            }
        }
        else
        {
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

            _repository.Add(entity);
        }
    }

    public void Delete(int id)
    {
        OutSideBpmConditionsTemplate outSideBpmConditionsTemplate = _repository.FirstOrDefault(a => a.Id == id);

        OutSideBpmBusinessParty outSideBpmBusinessParty = _outSideBpmBusinessPartyRepository
            .FirstOrDefault(a => a.Id == outSideBpmConditionsTemplate.BusinessPartyId);

        if (TemplateIsUsed(id, outSideBpmBusinessParty))
        {
            throw new AFBizException("审批流程中正使用此条件模板，无法删除");
        }

        outSideBpmConditionsTemplate.IsDel = 1;
        _repository.Update(outSideBpmConditionsTemplate);
    }

    private bool TemplateIsUsed(int id, OutSideBpmBusinessParty outSideBpmBusinessParty)
    {
        var bpmnConfs = _bpmnConfRepository
            .Find(conf => conf.BusinessPartyId == outSideBpmBusinessParty.Id && conf.IsDel == 0 &&
                           conf.EffectiveStatus == 1);

        if (!bpmnConfs.Any())
        {
            var confIds = bpmnConfs.Select(c => c.Id).ToList();
            var nodeTypeList = new List<int>
            {
                (int)NodeTypeEnum.NODE_TYPE_CONDITIONS,
                (int)NodeTypeEnum.NODE_TYPE_OUT_SIDE_CONDITIONS
            };

            var bpmnNodes = _bpmnNodeService._repository
                .Find(node => confIds.Contains(node.ConfId) && nodeTypeList.Contains(node.NodeType));

            if (bpmnNodes.Any())
            {
                List<long> nodeIds = bpmnNodes.Select(n => n.Id).ToList();

                var bpmnNodeConditionsConfs = _bpmnNodeConditionsConfRepository
                    .Find(conf => nodeIds.Contains(conf.BpmnNodeId));

                if (bpmnNodeConditionsConfs.Any())
                {
                    int conditionTemplatemark = 9999;
                    var conditionIds = bpmnNodeConditionsConfs.Select(c => c.Id).ToList();

                    var paramConfs = _bpmnNodeConditionsParamConfRepository
                        .Find(param => conditionIds.Contains(param.BpmnNodeConditionsId) &&
                                        param.ConditionParamType == conditionTemplatemark);

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
