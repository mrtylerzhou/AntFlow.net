using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IUserEntrustService : IAntFlowRepositoryMix<UserEntrust, IUserEntrustRepository>
{
    ResultAndPage<Entrust> GetEntrustPageList(PageDto pageDto, Entrust vo, int type);
    void UpdateEntrustList(DataVo dataVo);
    BaseIdTranStruVo GetEntrustEmployee(string employeeId, string employeeName, string powerId);
    BaseIdTranStruVo GetEntrustEmployeeOnly(string employeeId, string employeeName, string powerId);
    UserEntrust GetEntrustDetail(int id);
}
