using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;

namespace antflowcore.service.processor.lowcodeflow;

using System.Collections.Generic;
using System.Linq;

public class LFFieldControlPostProcessor : IAntFlowOrderPostProcessor<BpmnConfVo>
    {
        private readonly BpmnNodeLfFormdataFieldControlService _lfFormdataFieldControlService;

        public LFFieldControlPostProcessor(BpmnNodeLfFormdataFieldControlService lfFormdataFieldControlService)
        {
            _lfFormdataFieldControlService = lfFormdataFieldControlService;
        }

        public int Order() => 0;

        public void PostProcess(BpmnConfVo confVo)
        {
            if (confVo == null)
                return;

            int? isLowCodeFlow = confVo.IsLowCodeFlow;
            bool lowCodeFlowFlag = isLowCodeFlow == 1;

            if (!lowCodeFlowFlag)
                return;

            List<BpmnNodeVo> bpmnNodeVos = confVo.Nodes;
            long? lfFormDataId = confVo.LfFormDataId;
            List<BpmnNodeLfFormdataFieldControl> fieldControls = new List<BpmnNodeLfFormdataFieldControl>();

            foreach (var bpmnNodeVo in bpmnNodeVos)
            {
                var lfFieldControlVOs = bpmnNodeVo.LfFieldControlVOs;

                if (lfFieldControlVOs == null || !lfFieldControlVOs.Any())
                    continue;

                foreach (var lfFieldControlVO in lfFieldControlVOs)
                {
                    var fieldControl = new BpmnNodeLfFormdataFieldControl
                    {
                        FormdataId = lfFormDataId,
                        NodeId = bpmnNodeVo.Id,
                        FieldId = lfFieldControlVO.FieldId,
                        FieldName = lfFieldControlVO.FieldName,
                        Perm = lfFieldControlVO.Perm,
                        CreateUser = SecurityUtils.GetLogInEmpName(),
                        TenantId = MultiTenantUtil.GetCurrentTenantId(),
                    };

                    fieldControls.Add(fieldControl);
                }
            }

            int affrows = _lfFormdataFieldControlService.Frsql.Insert(fieldControls).ExecuteAffrows();
            if (affrows < 0)
            {
                throw new AFBizException("数据插入失败!");
            }
        }
    }

