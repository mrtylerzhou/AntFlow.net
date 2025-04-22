using System.Diagnostics;
using antflowcore.constant.enus;
using antflowcore.entity;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnelementadp;

using System.Collections.Generic;
using Microsoft.Extensions.Logging;


    public class BpmnAddFlowElementLoopAdaptor : IBpmnAddFlowElementAdaptor
    {
        private readonly ILogger<BpmnAddFlowElementLoopAdaptor> _logger;

        public BpmnAddFlowElementLoopAdaptor(ILogger<BpmnAddFlowElementLoopAdaptor> logger)
        {
            _logger = logger;
        }

        public  void AddFlowElement(BpmnConfCommonElementVo elementVo, AFProcess process, 
            Dictionary<string, object> startParamMap, BpmnStartConditionsVo bpmnStartConditions)
        {
            // 获取元素的代码
            var elementCode = ProcessNodeEnum.GetCodeByDesc(elementVo.ElementId);

            // 构建任务执行人参数名称和结束条件标记
            var assigneeParamName = $"loopUser{elementCode}";
            var endLoopMark = $"${{endLoopMark{elementCode}==true}}";

            // 添加循环任务元素到流程中
            process.AddFlowElement(BpmnBuildUtils.CreateLoopUserTask(
                elementVo.ElementId, 
                elementVo.ElementName, 
                assigneeParamName, 
                endLoopMark
            ));

            // 将默认的结束标记添加到参数映射中
            startParamMap[$"endLoopMark{elementCode}"] = false;

            _logger.LogInformation($"Loop User Task added with ID: {elementVo.ElementId}, Name: {elementVo.ElementName}");
        }
        
    }
