using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Repository;

public class BpmnTemplateService : AFBaseCurdRepositoryService<BpmnTemplate>, IBpmnTemplateService
{
    public BpmnTemplateService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void EditBpmnTemplate(BpmnConfVo bpmnConfVo, long confId)
    {
        List<BpmnTemplateVo> templateVos = bpmnConfVo.TemplateVos;
        if (ObjectUtils.IsEmpty(templateVos))
        {
            return;
        }

        List<BpmnTemplate> bpmnTemplateList = MapVos(templateVos, confId, bpmnConfVo.FormCode);
        int executeAffrows = Frsql.Insert(bpmnTemplateList).ExecuteAffrows();
        if (executeAffrows <= 0)
        {
            throw new AFBizException("t_bpmn_template插入失败");
        }
    }

    public void EditBpmnTemplate(BpmnNodeVo bpmnNodeVo)
    {
        List<BpmnTemplateVo> templateVos = bpmnNodeVo.TemplateVos;
        if (ObjectUtils.IsEmpty(templateVos))
        {
            return;
        }

        List<BpmnTemplate> bpmnTemplates = MapVos(templateVos, bpmnNodeVo.ConfId, bpmnNodeVo.FormCode);
        int executeAffrows = Frsql.Insert(bpmnTemplates).ExecuteAffrows();
        if (executeAffrows <= 0)
        {
            throw new AFBizException("t_bpmn_template插入失败");
        }
    }

    private List<BpmnTemplate> MapVos(List<BpmnTemplateVo> templateVos, long confId, string formCode)
    {
        List<BpmnTemplate> bpmnTemplates = new();
        string logInEmpNameSafe = SecurityUtils.GetLogInEmpNameSafe();
        foreach (BpmnTemplateVo bpmnTemplateVo in templateVos)
        {
            BpmnTemplate bpmnTemplate = bpmnTemplateVo.MapToEntity();
            bpmnTemplate.ConfId = confId;
            bpmnTemplate.Informs = string.Join(",", bpmnTemplateVo.InformList);
            bpmnTemplate.Emps = string.Join(",", bpmnTemplateVo.EmpList);
            bpmnTemplate.Roles = string.Join(",", bpmnTemplateVo.RoleList);
            bpmnTemplate.Funcs = string.Join(",", bpmnTemplateVo.FuncList);
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