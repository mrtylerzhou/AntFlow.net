using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmConditionsTemplateService : IAntFlowRepositoryMix<OutSideBpmConditionsTemplate, IOutSideBpmConditionsTemplateRepository>
{
    ResultAndPage<OutSideBpmConditionsTemplateVo> ListPage(PageDto pageDto, OutSideBpmConditionsTemplateVo vo);
    List<OutSideBpmConditionsTemplateVo> SelectConditionListByAppId(int appId);
    void Edit(OutSideBpmConditionsTemplateVo vo);
    void Delete(int id);
}
