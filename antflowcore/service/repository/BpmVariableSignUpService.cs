using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmVariableSignUpService: AFBaseCurdRepositoryService<BpmVariableSignUp>
{
    public BpmVariableSignUpService(IFreeSql freeSql) : base(freeSql)
    {
    }
}