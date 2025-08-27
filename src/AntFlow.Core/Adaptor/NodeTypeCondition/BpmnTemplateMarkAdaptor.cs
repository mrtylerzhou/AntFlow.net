using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.NodeTypeCondition;

public class BpmnTemplateMarkAdaptor : IBpmnNodeConditionsAdaptor
{
    private readonly OutSideBpmConditionsTemplateService _outSideBpmConditionsTemplateService;


    public BpmnTemplateMarkAdaptor(OutSideBpmConditionsTemplateService outSideBpmConditionsTemplateService)
    {
        _outSideBpmConditionsTemplateService = outSideBpmConditionsTemplateService;
    }

    public void SetConditionsResps(BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo)
    {
        List<int> templateMarks = bpmnNodeConditionsConfBaseVo.TemplateMarks;
        List<long> longTemplateMarks = templateMarks.Select(a => (long)a).ToList();
        if (!ObjectUtils.IsEmpty(templateMarks))
        {
            List<OutSideBpmConditionsTemplate> outSideBpmConditionsTemplates = _outSideBpmConditionsTemplateService
                .baseRepo.Where(a => longTemplateMarks.Contains(a.Id)).ToList();

            if (!ObjectUtils.IsEmpty(outSideBpmConditionsTemplates))
            {
                List<int> collect = outSideBpmConditionsTemplates.Select(o => (int)o.Id).ToList();
                bpmnNodeConditionsConfBaseVo.TemplateMarks = collect;
            }
        }
    }
}