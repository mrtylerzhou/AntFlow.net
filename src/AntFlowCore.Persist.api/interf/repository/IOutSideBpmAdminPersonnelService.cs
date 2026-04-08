using AntFlowCore.Base.entity;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmAdminPersonnelService : IBaseRepositoryService<OutSideBpmAdminPersonnel>
{
    List<long> GetBusinessPartyIdByEmployeeId(string employeeId, params string[] permCodes);
    List<long> GetBusinessPartyIdByEmployeeId(string employeeId, List<int> types);
}
