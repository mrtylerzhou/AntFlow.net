using AntFlowCore.Core.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

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