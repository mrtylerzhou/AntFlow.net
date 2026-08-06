using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IInformationTemplateService : IAntFlowRepositoryMix<InformationTemplate, IInformationTemplateRepository>
{
    ResultAndPage<InformationTemplateVo> List(PageDto pageDto, InformationTemplateVo informationTemplateVo);
    long Edit(InformationTemplateVo informationTemplateVo);
    List<InformationTemplateVo> GetList();
    void SetList(List<InformationTemplateVo> vos);
    InformationTemplateVo GetInformationTemplateById(long id);
}
