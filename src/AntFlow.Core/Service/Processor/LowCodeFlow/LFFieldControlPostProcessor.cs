using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Processor.LowCodeFlow;

public class LFFieldControlPostProcessor : IAntFlowOrderPostProcessor<BpmnConfVo>
{
    private readonly BpmnNodeLfFormdataFieldControlService _lfFormdataFieldControlService;

    public LFFieldControlPostProcessor(BpmnNodeLfFormdataFieldControlService lfFormdataFieldControlService)
    {
        _lfFormdataFieldControlService = lfFormdataFieldControlService;
    }

    public int Order()
    {
        return 0;
    }

    public void PostProcess(BpmnConfVo confVo)
    {
        if (confVo == null)
        {
            return;
        }

        int? isLowCodeFlow = confVo.IsLowCodeFlow;
        bool lowCodeFlowFlag = isLowCodeFlow == 1;

        if (!lowCodeFlowFlag)
        {
            return;
        }

        List<BpmnNodeVo> bpmnNodeVos = confVo.Nodes;
        long? lfFormDataId = confVo.LfFormDataId;
        List<BpmnNodeLfFormdataFieldControl> fieldControls = new();

        foreach (BpmnNodeVo? bpmnNodeVo in bpmnNodeVos)
        {
            List<LFFieldControlVO>? lfFieldControlVOs = bpmnNodeVo.LfFieldControlVOs;

            if (lfFieldControlVOs == null || !lfFieldControlVOs.Any())
            {
                continue;
            }

            foreach (LFFieldControlVO? lfFieldControlVO in lfFieldControlVOs)
            {
                BpmnNodeLfFormdataFieldControl? fieldControl = new()
                {
                    FormdataId = lfFormDataId,
                    NodeId = bpmnNodeVo.Id,
                    FieldId = lfFieldControlVO.FieldId,
                    FieldName = lfFieldControlVO.FieldName,
                    Perm = lfFieldControlVO.Perm,
                    CreateUser = SecurityUtils.GetLogInEmpName(),
                    TenantId = MultiTenantUtil.GetCurrentTenantId()
                };

                fieldControls.Add(fieldControl);
            }
        }

        int affrows = _lfFormdataFieldControlService.Frsql.Insert(fieldControls).ExecuteAffrows();
        if (affrows < 0)
        {
            throw new AFBizException("字段控制保存失败!");
        }
    }
}