using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.nodetypecondition;

public class BpmnTemplateMarkAdaptor: IBpmnNodeConditionsAdaptor
{
    private readonly IOutSideBpmConditionsTemplateService _outSideBpmConditionsTemplateService;


    public BpmnTemplateMarkAdaptor(IOutSideBpmConditionsTemplateService outSideBpmConditionsTemplateService)
    {
        _outSideBpmConditionsTemplateService = outSideBpmConditionsTemplateService;
    }
    public void SetConditionsResps(BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo)
    {
        List<int> templateMarks = bpmnNodeConditionsConfBaseVo.TemplateMarks;
        List<long> longTemplateMarks = templateMarks.Select(a=>(long)a).ToList();
        if (!ObjectUtils.IsEmpty(templateMarks))
        {
            List<OutSideBpmConditionsTemplate> outSideBpmConditionsTemplates = _outSideBpmConditionsTemplateService
                .baseRepo.Where(a => longTemplateMarks.Contains(a.Id)).ToList();
            
            if (!ObjectUtils.IsEmpty(outSideBpmConditionsTemplates))
            {
                List<int> collect = outSideBpmConditionsTemplates.Select(o => (int)o.Id).ToList();
                bpmnNodeConditionsConfBaseVo.TemplateMarks=collect;
            }
        }
    }
}