using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using FreeSql.Internal.Model;
using System.Linq.Expressions;

namespace AntFlow.Core.Service.Repository;

public class InformationTemplateService : AFBaseCurdRepositoryService<InformationTemplate>, IInformationTemplateService
{
    private readonly BpmnApproveRemindService _bpmnApproveRemindService;
    private readonly BpmnTemplateService _bpmnTemplateService;
    private readonly DefaultTemplateService _defaultTemplateService;

    public InformationTemplateService(IFreeSql freeSql,
        BpmnApproveRemindService bpmnApproveRemindService,
        DefaultTemplateService defaultTemplateService,
        BpmnTemplateService bpmnTemplateService
    ) : base(freeSql)
    {
        _bpmnApproveRemindService = bpmnApproveRemindService;
        _defaultTemplateService = defaultTemplateService;
        _bpmnTemplateService = bpmnTemplateService;
    }

    public ResultAndPage<InformationTemplateVo> List(PageDto pageDto, InformationTemplateVo informationTemplateVo)
    {
        Page<InformationTemplateVo> page = PageUtils.GetPageByPageDto<InformationTemplateVo>(pageDto);
        Expression<Func<InformationTemplate, bool>> expression = a => a.IsDel == 0;
        if (!string.IsNullOrEmpty(informationTemplateVo.Name))
        {
            expression.And(a => a.Name.Contains(informationTemplateVo.Name));
        }

        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        List<InformationTemplate> informationTemplates = baseRepo
            .Where(expression)
            .Page(basePagingInfo)
            .ToList();
        List<InformationTemplateVo> results = new();
        foreach (InformationTemplate informationTemplate in informationTemplates)
        {
            InformationTemplateVo templateVo = informationTemplate.MapToVo();
            templateVo.JumpUrlValue = JumpUrlEnum.GetDescByCode(informationTemplate.JumpUrl);
            templateVo.StatusValue = informationTemplate.Status == 0 ? "禁用" : "启用";
            results.Add(templateVo);
        }

        return PageUtils.GetResultAndPage(page.Of(results, basePagingInfo));
    }

    public long Edit(InformationTemplateVo informationTemplateVo)
    {
        Expression<Func<InformationTemplate, bool>> expression = a =>
            a.IsDel == 0 && a.Name == informationTemplateVo.Name;
        if (informationTemplateVo.Id != null && informationTemplateVo.Id > 0)
        {
            expression.And(a => a.Id == informationTemplateVo.Id);
        }

        //to check whether the template's name is duplicated
        List<InformationTemplate> list = baseRepo
            .Where(expression)
            .ToList();
        if (list.Count > 1)
        {
            throw new AFBizException("模板名称重复");
        }

        InformationTemplate informationTemplate = informationTemplateVo.MapToEntity();

        if (informationTemplate.Id > 0)
        {
            //modify
            if (informationTemplate.Status == 1)
            {
                //to check whether the template is in use,if so then throw exception
                List<BpmnTemplate> templates = _bpmnTemplateService.baseRepo
                    .Where(a => a.IsDel == 0 && a.TemplateId == informationTemplate.Id)
                    .ToList();

                List<BpmnApproveRemind> approveReminds = _bpmnApproveRemindService.baseRepo
                    .Where(a => a.IsDel == 0 && a.TemplateId == informationTemplate.Id)
                    .ToList();

                List<DefaultTemplate> defaultTemplates = _defaultTemplateService.baseRepo
                    .Where(a => a.IsDel == 0 && a.TemplateId == informationTemplate.Id)
                    .ToList();

                if (templates.Any()
                    || approveReminds.Any()
                    || defaultTemplates.Any())
                {
                    throw new AFBizException("模板标识重复，请修改后重试");
                }
            }

            informationTemplate.UpdateUser = SecurityUtils.GetLogInEmpIdSafe();
        }
        else
        {
            //add
            informationTemplate.CreateUser = SecurityUtils.GetLogInEmpNameSafe();
            informationTemplate.UpdateUser = SecurityUtils.GetLogInEmpNameSafe();
            informationTemplate.TenantId = MultiTenantUtil.GetCurrentTenantId();
            baseRepo.Insert(informationTemplate);
            informationTemplate.Name = "LCTZ_" + string.Format("%03d", informationTemplate.Id);
        }

        baseRepo.Update(informationTemplate);
        return informationTemplate.Id;
    }

    public List<DefaultTemplateVo> GetList()
    {
        List<DefaultTemplate> defaultTemplates = _defaultTemplateService
            .baseRepo
            .Where(a => a.IsDel == 0)
            .ToList();
        Dictionary<int, long?> map = defaultTemplates
            .Where(a => a.TemplateId != null && a.TemplateId > 0)
            .GroupBy(t => t.Event)
            .ToDictionary(
                g => g.Key,
                g => g.First().TemplateId
            );


        Dictionary<long, string> templateMap = map.Any()
            ? baseRepo
                .Where(t => map.Values.Contains(t.Id))
                .ToDictionary(t => t.Id, t => t.Name)
            : new Dictionary<long, string>();


        List<DefaultTemplateVo>? result = Enum.GetValues(typeof(EventTypeEnum))
            .Cast<EventTypeEnum>()
            .Select(o =>
            {
                int eventCode = (int)o;
                long? templateId = map.TryGetValue(eventCode, out long? id) ? id : null;
                string? templateName =
                    templateId.HasValue && templateMap.TryGetValue(templateId.Value, out string? name)
                        ? name
                        : null;

                return new DefaultTemplateVo
                {
                    Event = eventCode,
                    EventValue = o.GetDescription(),
                    TemplateId = templateId,
                    TemplateName = templateName
                };
            })
            .ToList();

        return result;
    }

    public void SetList(List<DefaultTemplateVo> vos)
    {
        throw new NotImplementedException();
    }

    public InformationTemplateVo GetInformationTemplateById(long id)
    {
        InformationTemplate informationTemplate = baseRepo.Where(a => a.Id == id).ToOne();
        if (informationTemplate == null)
        {
            throw new AFBizException("模板不存在或已删除");
        }

        InformationTemplateVo templateVo = informationTemplate.MapToVo();
        templateVo.JumpUrlValue = JumpUrlEnum.GetDescByCode(informationTemplate.JumpUrl);
        templateVo.StatusValue = informationTemplate.Status == 0 ? "禁用" : "启用";
        return templateVo;
    }
}