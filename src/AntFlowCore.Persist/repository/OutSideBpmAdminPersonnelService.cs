using System.Linq.Expressions;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class OutSideBpmAdminPersonnelService : IOutSideBpmAdminPersonnelService
{
    public OutSideBpmAdminPersonnelService(IOutSideBpmAdminPersonnelRepository repository)
    {
        _repository = repository;
    }

    public IOutSideBpmAdminPersonnelRepository _repository { get; }

    public List<long> GetBusinessPartyIdByEmployeeId(String employeeId, params string[] permCodes)
    {
        List<int> types = new List<int>();
        if(permCodes.Length>0){
            foreach (string permCode in permCodes)
            {
                AdminPersonnelTypeEnum enumByPermCode = AdminPersonnelTypeEnum.getEnumByPermCode(permCode);
                types.Add(enumByPermCode.Code);
            }
        }

        return GetBusinessPartyIdByEmployeeId(employeeId, types);
    }

    public List<long> GetBusinessPartyIdByEmployeeId(String employeeId, List<int> types)
    {
        Expression<Func<OutSideBpmAdminPersonnel,bool>> expression= x => x.EmployeeId == employeeId;
        if (types.Count > 0)
        {
            expression = expression.And(x=>types.Contains(x.Type));
        }

        List<long> businessPartyIds = _repository
            .Find(expression)
            .Select(x => x.BusinessPartyId)
            .Distinct()
            .ToList();
        
        return businessPartyIds;
    }
}
