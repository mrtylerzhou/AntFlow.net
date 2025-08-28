using AntFlow.Core.Bpmn.Service;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Vo;
using System.Text.Json;

namespace AntFlow.Core.Service.Repository;

public class BpmVariableSignUpPersonnelService : AFBaseCurdRepositoryService<BpmVariableSignUpPersonnel>,
    IBpmVariableSignUpPersonnelService
{
    public BpmVariableSignUpPersonnelService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void InsertSignUpPersonnel(TaskService taskService, string taskId, string processNumber,
        string taskTaskDefinitionKey, string assignee, List<BaseIdTranStruVo> signUpUsers)
    {
        List<BpmVariableSignUp> bpmVariableSignUps = Frsql.Select<BpmVariable, BpmVariableSignUp>()
            .InnerJoin((a, b) => a.Id == b.VariableId)
            .Where((a, b) => a.ProcessNum == processNumber && a.IsDel == 0 && b.ElementId == taskTaskDefinitionKey)
            .ToList<BpmVariableSignUp>((a, b) => b);
        if (!bpmVariableSignUps.Any())
        {
            throw new System.Exception($"can not get node sign up conf by process number {processNumber}");
        }

        BpmVariableSignUp bpmVariableSignUp = bpmVariableSignUps[0];
        string subElements = bpmVariableSignUp.SubElements;
        if (string.IsNullOrEmpty(subElements))
        {
            throw new AFBizException(
                $"can not get node sign up conf by sub elements,process number {processNumber},element id {taskTaskDefinitionKey}");
        }

        List<BpmnConfCommonElementVo>? subElementVos =
            JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(subElements);
        if (subElementVos == null)
        {
            throw new AFBizException(
                $"can not deserialize node sign up conf by elements,process number {processNumber},element id {taskTaskDefinitionKey}");
        }

        BpmnConfCommonElementVo bpmnConfCommonElementVo = subElementVos[0];
        List<BpmVariableSignUpPersonnel> bpmVariableSignUpPersonnels = signUpUsers.Select(a =>
            new BpmVariableSignUpPersonnel
            {
                VariableId = bpmVariableSignUp.VariableId,
                Assignee = a.Id,
                AssigneeName = a.Name,
                ElementId = bpmnConfCommonElementVo.ElementId,
                Remark = ""
            }).ToList();
        baseRepo.Insert(bpmVariableSignUpPersonnels);
    }
}