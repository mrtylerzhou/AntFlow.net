using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Engine.service.biz;

public class BpmnConfLFFormDataBizService : IBpmnConfLFFormDataBizService
{
    private readonly IBpmnConfLfFormdataService _bpmnConfLfFormdataService;
    private readonly IBpmnConfService _bpmnConfService;

    public BpmnConfLFFormDataBizService(
        IBpmnConfLfFormdataService bpmnConfLfFormdataService,
        IBpmnConfService bpmnConfService)
    {
        _bpmnConfLfFormdataService = bpmnConfLfFormdataService;
        _bpmnConfService = bpmnConfService;
    }
    public BpmnConfLfFormdata GetLFFormDataByFormCode(String formCode)
    {
        BpmnConf bpmnConf = _bpmnConfService._repository.GetBpmnConfByFormCode(formCode);
        BpmnConfConfigJson? confConfig = JsonConfUtil.ParseConfConfig(bpmnConf?.ConfConfigJson);
        if (!string.IsNullOrWhiteSpace(confConfig?.LowCodeFormConfig?.Formdata))
        {
            return new BpmnConfLfFormdata
            {
                BpmnConfId = bpmnConf.Id,
                Formdata = confConfig.LowCodeFormConfig.Formdata
            };
        }

        BpmnConfLfFormdata bpmnConfLfFormdata = _bpmnConfLfFormdataService
            ._repository
            .GetLFFormDataByFormCode(formCode);
        return bpmnConfLfFormdata;
    }
    
}
