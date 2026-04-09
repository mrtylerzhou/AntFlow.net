using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmConditionsTemplateService : IBaseRepositoryService<OutSideBpmConditionsTemplate>
{
    ResultAndPage<OutSideBpmConditionsTemplateVo> ListPage(PageDto pageDto, OutSideBpmConditionsTemplateVo vo);
    List<OutSideBpmConditionsTemplateVo> SelectConditionListByAppId(int appId);
    void Edit(OutSideBpmConditionsTemplateVo vo);
    void Delete(int id);
}
