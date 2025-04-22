using antflowcore.bpmn.service;
using AntFlowCore.Entity;
using antflowcore.vo;

namespace antflowcore.service.repository;

public class BpmVariableSignUpPersonnelService: AFBaseCurdRepositoryService<BpmVariableSignUpPersonnel>
{
    public BpmVariableSignUpPersonnelService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void InsertSignUpPersonnel(TaskService taskService, string taskId, string voProcessNumber, string taskTaskDefinitionKey, string taskAssignee, List<BaseIdTranStruVo> voSignUpUsers)
    {
        //todo to be implemented
        throw new NotImplementedException();
    }
}