using System.Linq.Expressions;
using AntFlowCore.Abstraction;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Business.service;
using AntFlowCore.Engine.service.biz;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.VirtualNode.service;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

[Route("informationTemplates")]
public class InformationTemplateController
{
    private readonly IInformationTemplateService _informationTemplateService;
    private readonly BpmVariableApproveRemindBizService _approveRemindBizService;
    public InformationTemplateController(InformationTemplateService informationTemplateService,
        BpmVariableApproveRemindBizService approveRemindBizService)
    {
        _informationTemplateService = informationTemplateService;
        _approveRemindBizService = approveRemindBizService;
    }

    /// <summary>
    /// test timeout remind. must be scheduled by external scheduler at most once per day
    /// </summary>
    [HttpGet("testDoTimeoutReminder")]
    public Result<string> TestDoTimeoutReminder()
    {
        _approveRemindBizService.DoTimeoutReminder();
        return ResultHelper.Success("ok");
    }

    [HttpPost("listPage")]
    public ResultAndPage<InformationTemplateVo> List(PageDto pageDto, [FromBody] InformationTemplateVo informationTemplateVo)
    {
        return _informationTemplateService.List(pageDto, informationTemplateVo);
    }
    [HttpGet("getInformationTemplateById")]
    public Result<InformationTemplateVo> GetInformationTemplateById(long id)
    {
        return ResultHelper.Success(_informationTemplateService.GetInformationTemplateById(id));
    }
    [HttpPost("updateById")]
    public Result<string> UpdateById([FromBody] InformationTemplateVo informationTemplateVo)
    {
        _informationTemplateService.Edit(informationTemplateVo);
        return ResultHelper.Success("ok");
    }

    [HttpPost("save")]
    public Result<long> Save([FromBody] InformationTemplateVo informationTemplateVo)
    {
        long templateId = _informationTemplateService.Edit(informationTemplateVo);
        return ResultHelper.Success(templateId);
    }

    [HttpPost("deleteById")]
    public Result<string> DeleteById([FromQuery] long id)
    {
        _informationTemplateService._repository.Update(new InformationTemplate
        {
            Id = id,
            UpdateUser = SecurityUtils.GetLogInEmpNameSafe(),
            IsDel = 1
        });
        return ResultHelper.Success("ok");
    }

    [HttpGet("listByName")]
    public Result<List<InformationTemplate>> ListByName([FromQuery] string name = null)
    {

        Expression<Func<InformationTemplate, bool>> expression = a => a.IsDel == 0 && a.Status == 0;
        if (!string.IsNullOrEmpty(name))
        {
            expression.And(a => a.Name == name);
        }

        List<InformationTemplate> results = _informationTemplateService
            ._repository.Find(expression);
        return ResultHelper.Success(results);
    }

    [HttpGet("defaultTemplates")]
    public Result<List<InformationTemplateVo>> GetDefaultTemplates()
    {
        return ResultHelper.Success(_informationTemplateService.GetList());
    }

    [HttpPost("defaultTemplates")]
    public Result<string> SetDefaultTemplates([FromBody] InformationTemplateVo[] vos)
    {
        _informationTemplateService.SetList(vos.ToList());
        return ResultHelper.Success("ok");
    }

    [HttpGet("getWildcardCharacte")]
    public Result<List<EnumerateVo>> GetWildcardCharacter([FromQuery] string name = null)
    {
        IEnumerable<WildcardCharacterEnum> wildcardEnums = WildcardCharacterEnum.Values;
        var filteredEnums = !string.IsNullOrEmpty(name)
            ? wildcardEnums.Where(o => o.Desc.Contains(name))
            : wildcardEnums;

        List<EnumerateVo> results = filteredEnums.Select(o => new EnumerateVo
        {
            Code = o.Code,
            Desc = o.Desc
        }).ToList();

        return ResultHelper.Success(results);
    }

    [HttpGet("getWildcardCharacter")]
    public Result<List<EnumerateVo>> getWildcardCharacter([FromQuery] string name = null)
    {
        List<EnumerateVo> lists = WildcardCharacterEnum.Values.Where(o => string.IsNullOrEmpty(name) || o.Desc.Contains(name)).Select(wildcardCharacterEnum => new EnumerateVo
        {
            Code = wildcardCharacterEnum.Code,
            Desc = wildcardCharacterEnum.Desc,
        }).ToList();
        return ResultHelper.Success(lists);
    }


    [HttpGet("getProcessEvents")]
    public Result<List<BaseNumIdStruVo>> getAllProcessEvents()
    {
        List<BaseNumIdStruVo> lists = new List<BaseNumIdStruVo>();
        Dictionary<EventTypeEnum, EventTypeProperties> eventTypeMappings = EventTypeEnumExtensions.EventTypeMappings;
        foreach (var (key, eventTypeProperties) in eventTypeMappings)
        {
            BaseNumIdStruVo baseNumIdStruVo = new BaseNumIdStruVo
            {
                Id = (int)key,
                Name = eventTypeProperties.Description,
            };
            lists.Add(baseNumIdStruVo);
        }

        return ResultHelper.Success(lists);
    }


    [HttpGet("getAllNoticeTypes")]
    public Result<List<BaseNumIdStruVo>> getAllNoticeTypes()
    {
        List<BaseNumIdStruVo> lists = ProcessNoticeEnum.Values.Select(processNoticeEnum => new BaseNumIdStruVo
        {
            Id = processNoticeEnum.Code,
            Name = processNoticeEnum.Desc,
        }).ToList();
        return ResultHelper.Success(lists);
    }

}
