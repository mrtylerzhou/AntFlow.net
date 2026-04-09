using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using FreeSql;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IInformationTemplateService : IBaseRepositoryService<InformationTemplate>
{
    ResultAndPage<InformationTemplateVo> List(PageDto pageDto, InformationTemplateVo informationTemplateVo);
    long Edit(InformationTemplateVo informationTemplateVo);
    List<DefaultTemplateVo> GetList();
    void SetList(List<DefaultTemplateVo> vos);
    InformationTemplateVo GetInformationTemplateById(long id);
}
