using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using antflowcore.service.interf.repository;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmApproveTemplateService : IAntFlowRepositoryMix<OutSideBpmApproveTemplate, IOutSideBpmApproveTemplateRepository>
{
    ResultAndPage<OutSideBpmApproveTemplateVo> ListPage(PageDto pageDto, OutSideBpmApproveTemplateVo vo);
    List<OutSideBpmApproveTemplateVo> SelectListByTemp(int applicationId);
    OutSideBpmApproveTemplateVo Detail(long id);
    void Edit(OutSideBpmApproveTemplateVo vo);
    void Delete(long id);
}
