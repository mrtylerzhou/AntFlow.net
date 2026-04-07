using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IOutSideBpmConditionsTemplateService : IBaseRepositoryService<OutSideBpmConditionsTemplate>
{
    ResultAndPage<OutSideBpmConditionsTemplateVo> ListPage(PageDto pageDto, OutSideBpmConditionsTemplateVo vo);
    List<OutSideBpmConditionsTemplateVo> SelectConditionListByAppId(int appId);
    void Edit(OutSideBpmConditionsTemplateVo vo);
    void Delete(int id);
}
