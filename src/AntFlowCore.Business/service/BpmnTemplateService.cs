using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class BpmnTemplateService : IBpmnTemplateService
{
    public IBpmnTemplateRepository _repository { get; }

    public BpmnTemplateService(IBpmnTemplateRepository repository)
    {
        _repository = repository;
    }

    public void EditBpmnTemplate(BpmnConfVo bpmnConfVo, long confId)
    {
        List<BpmnTemplateVo> templateVos = bpmnConfVo.TemplateVos;
        if (ObjectUtils.IsEmpty(templateVos))
        {
            return;
        }

        List<BpmnTemplate> bpmnTemplateList = MapVos(templateVos, confId, bpmnConfVo.FormCode);
        _repository.AddRange(bpmnTemplateList);
    }

    public void EditBpmnTemplate(BpmnNodeVo bpmnNodeVo)
    {
        List<BpmnTemplateVo> templateVos = bpmnNodeVo.TemplateVos;
        if (ObjectUtils.IsEmpty(templateVos))
        {
            return;
        }

        List<BpmnTemplate> bpmnTemplates = MapVos(templateVos, bpmnNodeVo.ConfId, bpmnNodeVo.FormCode);
        _repository.AddRange(bpmnTemplates);
    }

    private List<BpmnTemplate> MapVos(List<BpmnTemplateVo> templateVos, long confId, string formCode)
    {
        List<BpmnTemplate> bpmnTemplates = new List<BpmnTemplate>();
        string logInEmpNameSafe = SecurityUtils.GetLogInEmpNameSafe();
        foreach (BpmnTemplateVo bpmnTemplateVo in templateVos)
        {
            BpmnTemplate bpmnTemplate = bpmnTemplateVo.MapToEntity();
            bpmnTemplate.ConfId = confId;
            bpmnTemplate.Informs = string.Join(",", bpmnTemplateVo.InformList?.Select(x=>x.Id)??new List<string>());
            bpmnTemplate.Emps = string.Join(",", bpmnTemplateVo.EmpList?.Select(x=>x.Id)??new List<string>());
            bpmnTemplate.Roles = string.Join(",", bpmnTemplateVo.RoleList?.Select(x=>x.Id)??new List<string>());
            bpmnTemplate.Funcs = string.Join(",", bpmnTemplateVo.FuncList?.Select(x=>x.Id)??new List<string>());
            bpmnTemplate.MessageSendType = string.Join(",",
                bpmnTemplateVo.MessageSendTypeList?.Select(x => x.Id) ?? new List<long>());
            bpmnTemplate.FormCode = formCode;
            bpmnTemplate.CreateUser = logInEmpNameSafe;
            bpmnTemplate.TenantId = MultiTenantUtil.GetCurrentTenantId();
            bpmnTemplates.Add(bpmnTemplate);
        }

        return bpmnTemplates;
    }
}
