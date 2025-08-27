using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;
using System.Linq.Expressions;

namespace AntFlow.Core.Service.Repository;

public class OutSideBpmAdminPersonnelService : AFBaseCurdRepositoryService<OutSideBpmAdminPersonnel>,
    IOutSideBpmAdminPersonnelService
{
    public OutSideBpmAdminPersonnelService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<long> GetBusinessPartyIdByEmployeeId(string employeeId, params string[] permCodes)
    {
        List<int> types = new();
        if (permCodes.Length > 0)
        {
            foreach (string permCode in permCodes)
            {
                AdminPersonnelTypeEnum enumByPermCode = AdminPersonnelTypeEnum.getEnumByPermCode(permCode);
                types.Add(enumByPermCode.Code);
            }
        }

        return GetBusinessPartyIdByEmployeeId(employeeId, types);
    }

    private List<long> GetBusinessPartyIdByEmployeeId(string employeeId, List<int> types)
    {
        Expression<Func<OutSideBpmAdminPersonnel, bool>> expression = x => x.EmployeeId == employeeId;
        if (types.Count > 0)
        {
            expression = expression.And(x => types.Contains(x.Type));
        }

        List<long> businessPartyIds = baseRepo
            .Where(expression)
            .Distinct()
            .ToList(x => x.BusinessPartyId);

        return businessPartyIds;
    }
}