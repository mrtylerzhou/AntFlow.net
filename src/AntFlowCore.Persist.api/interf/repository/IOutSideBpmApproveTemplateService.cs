using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmApproveTemplateService : IBaseRepositoryService<OutSideBpmApproveTemplate>
{
    ResultAndPage<OutSideBpmApproveTemplateVo> ListPage(PageDto pageDto, OutSideBpmApproveTemplateVo vo);
    List<OutSideBpmApproveTemplateVo> SelectListByTemp(int applicationId);
    OutSideBpmApproveTemplateVo Detail(long id);
    void Edit(OutSideBpmApproveTemplateVo vo);
    void Delete(long id);
}
