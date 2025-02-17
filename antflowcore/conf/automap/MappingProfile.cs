using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.conf.automap;

using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BpmnConfVo, BpmnConf>();
        CreateMap<BpmnConf, BpmnConfVo>();
        
        CreateMap<BpmnNodeVo, BpmnNode>();
        CreateMap<BpmnNode, BpmnNodeVo>();
        
        CreateMap<BpmnTemplateVo, BpmnTemplate>();
        CreateMap<BpmnTemplate, BpmnTemplateVo>();
        
        CreateMap<BpmProcessAppApplication, BpmProcessAppApplicationVo>();
        CreateMap<BpmProcessAppApplicationVo, BpmProcessAppApplication>();
        
        CreateMap<BpmnApproveRemind, BpmnApproveRemindVo>();
        CreateMap<BpmnApproveRemindVo, BpmnApproveRemind>();

        CreateMap<ThirdPartyAccountApply, ThirdPartyAccountApplyVo>();
        CreateMap<ThirdPartyAccountApplyVo, ThirdPartyAccountApply>();
        
    }
}