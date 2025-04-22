using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class BpmVariableViewPageButtonService : AFBaseCurdRepositoryService<BpmVariableViewPageButton>
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