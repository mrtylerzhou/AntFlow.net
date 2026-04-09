using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmAccessBusinessService : IBaseRepositoryService<OutSideBpmAccessBusiness>
{
    void UpdateById(OutSideBpmAccessBusiness outSideBpmAccessBusiness);
    OutSideBpmAccessRespVo AccessBusinessStart(OutSideBpmAccessBusinessVo vo);
    ResultAndPage<BpmnConfVo> SelectOutSideFormCodePageList(PageDto pageDto, BpmnConfVo vo);
    void ProcessBreak(OutSideBpmAccessBusinessVo vo);
    List<OutSideBpmAccessProcessRecordVo> OutSideProcessRecord(string processNumber);
}
