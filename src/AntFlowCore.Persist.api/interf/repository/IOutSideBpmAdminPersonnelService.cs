using AntFlowCore.Base.entity;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmAdminPersonnelService : IAntFlowRepositoryMix<OutSideBpmAdminPersonnel, IOutSideBpmAdminPersonnelRepository>
{
    List<long> GetBusinessPartyIdByEmployeeId(string employeeId, params string[] permCodes);
    List<long> GetBusinessPartyIdByEmployeeId(string employeeId, List<int> types);
}
