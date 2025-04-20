using System.Linq.Expressions;
using antflowcore.constant.enus;
using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class OutSideBpmAdminPersonnelService: AFBaseCurdRepositoryService<OutSideBpmAdminPersonnel>
{
    public OutSideBpmAdminPersonnelService(IFreeSql freeSql) : base(freeSql)
    {
    }

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

    List<long> GetBusinessPartyIdByEmployeeId(String employeeId, List<int> types)
    {
        Expression<Func<OutSideBpmAdminPersonnel,bool>> expression= x => x.EmployeeId == employeeId;
        if (types.Count > 0)
        {
            expression = expression.And(x=>types.Contains(x.Type));
        }

        List<long> businessPartyIds = this
            .baseRepo
            .Where(expression)
            .Distinct()
            .ToList(x => x.BusinessPartyId);
        
        return businessPartyIds;
    }
}