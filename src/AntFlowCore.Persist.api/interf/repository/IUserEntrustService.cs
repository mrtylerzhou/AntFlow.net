using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Entity;
using AntFlowCore.Vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IUserEntrustService : IBaseRepositoryService<UserEntrust>
{
    ResultAndPage<Entrust> GetEntrustPageList(PageDto pageDto, Entrust vo, int type);
    void UpdateEntrustList(DataVo dataVo);
    BaseIdTranStruVo GetEntrustEmployee(string employeeId, string employeeName, string powerId);
    BaseIdTranStruVo GetEntrustEmployeeOnly(string employeeId, string employeeName, string powerId);
    UserEntrust GetEntrustDetail(int id);
}
