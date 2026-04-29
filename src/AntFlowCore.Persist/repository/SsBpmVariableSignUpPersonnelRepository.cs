using System.Text.Json;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using SqlSugar;

namespace AntFlowCore.Persist.repository;

public class SsBpmVariableSignUpPersonnelRepository : RepositoryBase<BpmVariableSignUpPersonnel>, IBpmVariableSignUpPersonnelRepository
{
    public SsBpmVariableSignUpPersonnelRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void InsertSignUpPersonnel(ITaskService taskService, string taskId, string processNumber, string taskTaskDefinitionKey, string assignee, List<BaseIdTranStruVo> signUpUsers)
    {
        List<BpmVariableSignUp> bpmVariableSignUps = Db.Queryable<BpmVariableSignUp>()
            .InnerJoin<BpmVariable>((b, a) => a.Id == b.VariableId)
            .Where((b, a) => a.ProcessNum == processNumber && a.IsDel == 0 && b.ElementId == taskTaskDefinitionKey)
            .Select((b, a) => b)
            .ToList();
        if (!bpmVariableSignUps.Any())
        {
            throw new Exception($"can not get node sign up conf by process number {processNumber}");
        }

        BpmVariableSignUp bpmVariableSignUp = bpmVariableSignUps[0];
        string subElements = bpmVariableSignUp.SubElements;
        if (string.IsNullOrEmpty(subElements))
        {
            throw new AFBizException($"can not get node sign up conf by sub elements,process number {processNumber},element id {taskTaskDefinitionKey}");
        }
        List<BpmnConfCommonElementVo>? subElementVos = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(subElements);
        if (subElementVos == null)
        {
            throw new AFBizException($"can not deserialize node sign up conf by elements,process number {processNumber},element id {taskTaskDefinitionKey}");
        }

        BpmnConfCommonElementVo bpmnConfCommonElementVo = subElementVos[0];
        List<BpmVariableSignUpPersonnel> bpmVariableSignUpPersonnels = signUpUsers.Select(a => new BpmVariableSignUpPersonnel()

        {
            VariableId = bpmVariableSignUp.VariableId,
            Assignee = a.Id,
            AssigneeName = a.Name,
            ElementId = bpmnConfCommonElementVo.ElementId,
            Remark = "",
            CreateTime = DateTime.Now,
        }).ToList();
        Db.Insertable(bpmVariableSignUpPersonnels).ExecuteCommand();
    }

    public List<KeyValuePair<string, string>> GetSignUpNodeAssigneeMap(string procInstId, string elementId)
    {
        List<KeyValuePair<string, string>> signupNodeAssigneeMap = Db.Queryable<BpmBusinessProcess>()
            .InnerJoin<BpmVariable>((a, b) => a.BusinessNumber == b.ProcessNum)
            .InnerJoin<BpmVariableSignUpPersonnel>((a, b, c) => b.Id == c.VariableId)
            .Where((a, b, c) => a.ProcInstId == procInstId && c.ElementId == elementId)
            .Select((a, b, c) => new KeyValuePair<string, string>(c.Assignee, c.AssigneeName))
            .ToList();
        return signupNodeAssigneeMap;
    }
}
