using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmVariableViewPageButtonService: AFBaseCurdRepositoryService<BpmVariableViewPageButton>,IBpmVariableViewPageButtonService
{
    public BpmVariableViewPageButtonService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmVariableViewPageButton> GetButtonsByProcessNumber(string processNum)
    {
        List<BpmVariableViewPageButton> bpmVariableViewPageButtons = Frsql.Select<BpmVariableViewPageButton, BpmVariable>()
            .LeftJoin((a, b) => a.VariableId == b.Id)
            .Where((a, b) => b.ProcessNum == processNum)
            .ToList();
        return bpmVariableViewPageButtons;
    }
}