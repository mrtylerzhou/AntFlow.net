using System.Linq.Expressions;
using antflowcore.constant.enus;
using antflowcore.dto;
using antflowcore.entity;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class InformationTemplateService: AFBaseCurdRepositoryService<InformationTemplate>
{
    private readonly BpmnApproveRemindService _bpmnApproveRemindService;
    private readonly DefaultTemplateService _defaultTemplateService;
    private readonly BpmnTemplateService _bpmnTemplateService;

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
    public ResultAndPage<InformationTemplateVo> List(PageDto pageDto, InformationTemplateVo informationTemplateVo) {
        Page<InformationTemplateVo> page = PageUtils.GetPageByPageDto<InformationTemplateVo>(pageDto);
        Expression<Func<InformationTemplate, bool>> expression = a => a.IsDel == 0;
        if (!string.IsNullOrEmpty(informationTemplateVo.Name))
        {
            expression.And(a => a.Name.Contains(informationTemplateVo.Name));
        }

        List<InformationTemplate> informationTemplates = this.baseRepo
            .Where(expression)
            .Page(page.Current,page.Size)
            .ToList();
        List<InformationTemplateVo> results = new List<InformationTemplateVo>();
        foreach (InformationTemplate informationTemplate in informationTemplates)
        {
            InformationTemplateVo templateVo = informationTemplate.MapToVo();
            templateVo.JumpUrlValue = JumpUrlEnum.GetDescByCode(informationTemplate.JumpUrl);
            templateVo.StatusValue = informationTemplate.Status == 0 ? "启用" : "禁用";
            results.Add(templateVo);
        }

        page.Records = results;
        return PageUtils.GetResultAndPage(page);
    }

    public void Edit(InformationTemplateVo informationTemplateVo)
    {
        Expression<Func<InformationTemplate, bool>> expression = a =>
            a.IsDel == 0 && a.Name == informationTemplateVo.Name;
        if (informationTemplateVo.Id != null&&informationTemplateVo.Id>0)
        {
            expression.And(a => a.Id == informationTemplateVo.Id);
        }
         //to check whether the template's name is duplicated
        List<InformationTemplate> list = this.baseRepo
            .Where(expression)
            .ToList();
        if (list.Count>1) {
            throw new AFBizException("模板名称重复");
        }

        InformationTemplate informationTemplate = informationTemplateVo.MapToEntity();
        
        if (informationTemplate.Id>0) {
            //modify
            if (informationTemplate.Status==1) {

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
                    || defaultTemplates.Any()) {
                    throw new AFBizException("该模板正在使用中，不可禁用！");
                }
            }

            informationTemplate.UpdateUser = SecurityUtils.GetLogInEmpIdSafe();
        } else {
            //add
            informationTemplate.CreateUser=SecurityUtils.GetLogInEmpNameSafe();
            informationTemplate.UpdateUser=SecurityUtils.GetLogInEmpNameSafe();
            this.baseRepo.Insert(informationTemplate);
            informationTemplate.Name=("LCTZ_" + String.Format("%03d", informationTemplate.Id));
        }

        this.baseRepo.Update(informationTemplate);
    }

    public List<DefaultTemplateVo> GetList()
    {
        List<DefaultTemplate> defaultTemplates = _defaultTemplateService
            .baseRepo
            .Where(a=>a.IsDel==0)
            .ToList();
        Dictionary<int,long?> map =defaultTemplates
            .Where(a=>a.TemplateId!=null&&a.TemplateId>0)
            .GroupBy(t => t.Event)
            .ToDictionary(
                g => g.Key,
                g => g.First().TemplateId
            );

        
        Dictionary<long, string> templateMap = map.Any()
            ?this.baseRepo
                .Where(t => map.Values.Contains(t.Id))
                .ToDictionary(t => t.Id, t => t.Name)
            : new Dictionary<long, string>();

       
        var result = Enum.GetValues(typeof(EventTypeEnum))
            .Cast<EventTypeEnum>()
            .Select(o =>
            {
                var eventCode = (int)o;
                var templateId = map.TryGetValue(eventCode, out var id) ? (long?)id : null;
                var templateName = templateId.HasValue && templateMap.TryGetValue(templateId.Value, out var name)
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
}