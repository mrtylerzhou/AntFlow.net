using AntFlowCore.Abstraction;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Engine.service.processor.lowcodeflow;

public class LFFieldControlPostProcessor : IAntFlowOrderPostProcessor<BpmnConfVo>
    {
        public int Order() => 0;

        public void PostProcess(BpmnConfVo confVo)
        {
            // IBpmnNodeLfFormdataFieldControlService has been removed; LF field control post-processing is no longer supported
        }
    }
