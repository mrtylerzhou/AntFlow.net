using System.Linq.Expressions;
using AntFlowCore.Abstraction;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.util;
using AntFlowCore.Core;
using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using AntFlowCore.Extensions;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Persist.repository;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

[Route("informationTemplates")]
public class InformationTemplateController
{
    private readonly IInformationTemplateService _informationTemplateService;
    private readonly IBpmVariableApproveRemindService _bpmVariableApproveRemindService;
    public InformationTemplateController(InformationTemplateService informationTemplateService, 
        BpmVariableApproveRemindService bpmVariableApproveRemindService)
    {
        _informationTemplateService = informationTemplateService;
        _bpmVariableApproveRemindService = bpmVariableApproveRemindService;
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
        _informationTemplateService.baseRepo.Update(new InformationTemplate
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
            .baseRepo
            .Where(expression)
            .ToList();
        return ResultHelper.Success(results);
    }

    [HttpGet("defaultTemplates")]
    public Result<List<DefaultTemplateVo>> GetDefaultTemplates()
    {
        return ResultHelper.Success(_informationTemplateService.GetList());
    }

    [HttpPost("defaultTemplates")]
    public Result<string> SetDefaultTemplates([FromBody] DefaultTemplateVo[] vos)
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
    [HttpGet("getProcessEvents")]
    public Result<List<BaseNumIdStruVo>> getAllProcessEvents()
    {
        List<BaseNumIdStruVo> lists = new List<BaseNumIdStruVo>();
        Dictionary<EventTypeEnum,EventTypeProperties> eventTypeMappings = EventTypeEnumExtensions.EventTypeMappings;
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
}
