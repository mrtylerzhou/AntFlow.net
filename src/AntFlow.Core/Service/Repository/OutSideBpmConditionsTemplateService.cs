using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Linq.Expressions;
using System.Text.Json;

namespace AntFlow.Core.Service.Repository;

public class OutSideBpmConditionsTemplateService : AFBaseCurdRepositoryService<OutSideBpmConditionsTemplate>,
    IOutSideBpmConditionsTemplateService
{
    private readonly BpmnConfService _bpmnConfService;
    private readonly BpmnNodeConditionsConfService _bpmnNodeConditionsConfService;
    private readonly BpmnNodeConditionsParamConfService _bpmnNodeConditionsParamConfService;
    private readonly BpmnNodeService _bpmnNodeService;
    private readonly BpmProcessAppApplicationService _bpmProcessAppApplicationService;
    private readonly UserService _employeeService;
    private readonly OutSideBpmBaseService _outSideBpmBaseService;
    private readonly OutSideBpmBusinessPartyService _outSideBpmBusinessPartyService;

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

        // 查询当前登录员工的业务方列表，进行权限查询
        List<OutSideBpmBusinessPartyVo>? emplBusinessPartys = _outSideBpmBaseService.GetEmplBusinessPartys(string.Empty,
            AdminPersonnelTypeEnum.ADMIN_PERSONNEL_TYPE_TEMPLATE.PermCode);
        vo.BusinessPartyIds = emplBusinessPartys.Select(x => x.Id).ToList();

        // ��ҳ��ѯ����ģ��
        List<OutSideBpmConditionsTemplateVo> outSideBpmConditionsTemplateVos = SelectPageList(page, vo);

        if (outSideBpmConditionsTemplateVos == null || !outSideBpmConditionsTemplateVos.Any())
        {
            return PageUtils.GetResultAndPage(page);
        }

        // ������ѯҵ����Ϣ
        List<long>? businessPartyIds =
            outSideBpmConditionsTemplateVos.Select(x => x.BusinessPartyId).Distinct().ToList();
        Dictionary<long, OutSideBpmBusinessParty> businessPartyMap = _outSideBpmBusinessPartyService.baseRepo
            .Where(a => businessPartyIds.Contains(a.Id))
            .ToList()
            .ToDictionary(x => x.Id, x => x);

        // ������ѯԱ����Ϣ
        List<string>? userIds = outSideBpmConditionsTemplateVos.Select(x => x.CreateUserId)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct().ToList();
        Dictionary<string, Employee>? employeeMap =
            _employeeService.GetEmployeeDetailByIds(userIds)
                .ToDictionary(x => x.Id, x => x);

        foreach (OutSideBpmConditionsTemplateVo item in outSideBpmConditionsTemplateVos)
        {
            if (businessPartyMap.TryGetValue(item.BusinessPartyId, out OutSideBpmBusinessParty? party))
            {
                item.BusinessPartyMark = party.BusinessPartyMark;
                item.BusinessPartyName = party.Name;
            }

            if (!string.IsNullOrEmpty(item.CreateUserId) &&
                employeeMap.TryGetValue(item.CreateUserId, out Employee? emp))
            {
                item.CreateUserName = emp.Username;
            }

            if (item.ApplicationId != null && item.ApplicationId != 0)
            {
                BpmProcessAppApplication? app = _bpmProcessAppApplicationService
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

    private List<OutSideBpmConditionsTemplateVo> SelectPageList(Page<OutSideBpmConditionsTemplateVo> page,
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

        List<OutSideBpmConditionsTemplateVo> outSideBpmConditionsTemplateVos = baseRepo
            .Where(expression)
            .ToList<OutSideBpmConditionsTemplateVo>(a => new OutSideBpmConditionsTemplateVo
            {
                Id = a.Id,
                BusinessPartyId = a.BusinessPartyId,
                TemplateName = a.TemplateName,
                TemplateMark = a.TemplateMark,
                ApplicationId = a.ApplicationId,
                CreateUserId = a.CreateUserId
            });
        return outSideBpmConditionsTemplateVos;
    }

    public List<OutSideBpmConditionsTemplateVo> SelectConditionListByAppId(int appId)
    {
        List<OutSideBpmConditionsTemplate>? list = baseRepo
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
        bool existsTemplateMark = baseRepo
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

        // �ж�ģ�������Ƿ��ظ�
        bool existsTemplateName = baseRepo
            .Where(nameExpression)
            .Any();

        if (existsTemplateName)
        {
            throw new AFBizException("����ģ�������ظ����༭ʧ��");
        }

        DateTime now = DateTime.Now;
        string? currentUserId = SecurityUtils.GetLogInEmpIdSafe();
        string? currentUserName = SecurityUtils.GetLogInEmpName();

        if (vo.Id != null)
        {
            OutSideBpmConditionsTemplate entity = baseRepo.Where(a => a.Id == vo.Id).ToOne();
            if (entity != null)
            {
                // ����
                entity.TemplateName = vo.TemplateName;
                entity.TemplateMark = vo.TemplateMark;
                entity.Remark = vo.Remark;
                entity.ApplicationId = vo.ApplicationId;
                entity.BusinessPartyId = vo.BusinessPartyId;
                entity.UpdateUser = currentUserName;
                entity.UpdateTime = now;
                baseRepo.Update(entity);
            }
        }
        else
        {
            // �½�
            OutSideBpmConditionsTemplate? entity = new()
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

            baseRepo.Insert(entity);
        }
    }

    public void Delete(int id)
    {
        OutSideBpmConditionsTemplate outSideBpmConditionsTemplate = baseRepo.Where(a => a.Id == id).ToOne();


        OutSideBpmBusinessParty outSideBpmBusinessParty = _outSideBpmBusinessPartyService
            .baseRepo.Where(a => a.Id == outSideBpmConditionsTemplate.BusinessPartyId).ToOne();


        if (TemplateIsUsed(id, outSideBpmBusinessParty))
        {
            throw new AFBizException("条件模板正在使用此条件模板，无法删除");
        }

        Frsql
            .Update<OutSideBpmConditionsTemplate>()
            .Set(a => a.IsDel, 1)
            .Where(a => a.Id == id)
            .ExecuteAffrows();
    }

    private bool TemplateIsUsed(int id, OutSideBpmBusinessParty outSideBpmBusinessParty)
    {
        List<BpmnConf>? bpmnConfs = _bpmnConfService.baseRepo
            .Where(conf => conf.BusinessPartyId == outSideBpmBusinessParty.Id && conf.IsDel == 0 &&
                           conf.EffectiveStatus == 1)
            .ToList();

        if (!bpmnConfs.Any())
        {
            List<long>? confIds = bpmnConfs.Select(c => c.Id).ToList();
            List<int>? nodeTypeList = new()
            {
                (int)NodeTypeEnum.NODE_TYPE_CONDITIONS, (int)NodeTypeEnum.NODE_TYPE_OUT_SIDE_CONDITIONS
            };

            List<BpmnNode>? bpmnNodes = _bpmnNodeService.baseRepo
                .Where(node => confIds.Contains(node.ConfId) && nodeTypeList.Contains(node.NodeType))
                .ToList();

            if (bpmnNodes.Any())
            {
                List<long> nodeIds = bpmnNodes.Select(n => n.Id).ToList();

                List<BpmnNodeConditionsConf>? bpmnNodeConditionsConfs = _bpmnNodeConditionsConfService
                    .baseRepo
                    .Where(conf => nodeIds.Contains(conf.BpmnNodeId))
                    .ToList();

                if (bpmnNodeConditionsConfs.Any())
                {
                    int conditionTemplatemark = (int)ConditionTypeEnum.CONDITION_TEMPLATEMARK;
                    List<long>? conditionIds = bpmnNodeConditionsConfs.Select(c => c.Id).ToList();

                    List<BpmnNodeConditionsParamConf>? paramConfs = _bpmnNodeConditionsParamConfService.baseRepo
                        .Where(param => conditionIds.Contains(param.BpmnNodeConditionsId) &&
                                        param.ConditionParamType == conditionTemplatemark)
                        .ToList();

                    if (paramConfs.Any())
                    {
                        List<int>? usedTempList = new();

                        foreach (BpmnNodeConditionsParamConf? param in paramConfs)
                        {
                            string? json = param.ConditionParamJsom;
                            List<int>? list = JsonSerializer.Deserialize<List<int>>(json);
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