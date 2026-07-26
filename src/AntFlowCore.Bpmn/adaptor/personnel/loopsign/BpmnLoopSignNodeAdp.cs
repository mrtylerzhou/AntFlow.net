using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.personnel.loopsign;



    /// <summary>
    /// 层层审批找人适配器。
    ///
    /// <para>当前为 demo 实现:底层 <see cref="IUserService.QueryLeadersByEmployeeIdAndTier"/> /
    /// <see cref="IUserService.QueryLeadersByEmployeeIdAndGrade"/> 仍返回扁平 <c>List&lt;BaseIdTranStruVo&gt;</c>,
    /// 每层只有 1 个领导。这里把它包装成 <c>[[a],[b],[c]]</c>(每层 1 人),
    /// 行为与改造前一致。</para>
    ///
    /// <para>真正业务侧需要"每层多人"时,重写本类,返回 <c>List&lt;List&lt;string&gt;&gt;</c>,
    /// 外层=层,内层=层内多人。框架层已支持多层多人。</para>
    /// </summary>
    public class BpmnLoopSignNodeAdp : AbstractOrderedSignNodeAdp
    {
        private readonly IUserService userService;


        public BpmnLoopSignNodeAdp(IUserService userService,AssigneeVoBuildUtils assigneeVoBuildUtils) : base(assigneeVoBuildUtils)
        {
            this.userService = userService;
        }

        public override List<List<string>> GetAssigneeIds(BpmnNodeVo nodeVo, BpmnStartConditionsVo bpmnStartConditions)
        {
            var propertysVo = nodeVo.Property;
            if (propertysVo == null)
            {
                throw new AFBizException("loop sign failure, node has no property!");
            }

            string startUserId = bpmnStartConditions.StartUserId;

            // type organization line, reporting line, you can also give it other meaning
            // it is just a property, it is only meaningful when you use it in your business
            int? loopEndType = propertysVo.LoopEndType;

            // two parameters, cannot be both empty
            // how many levels
            int? loopNumberPlies = propertysVo.LoopNumberPlies;
            // end levels
            int? loopEndGrade = propertysVo.LoopEndGrade;
            // end person
            var loopEndPersonList = new HashSet<string>();
            if (propertysVo.LoopEndPersonList != null && propertysVo.LoopEndPersonList.Any())
            {
                foreach (var s in propertysVo.LoopEndPersonList)
                {
                    loopEndPersonList.Add(s.ToString());
                }
            }

            if (!loopNumberPlies.HasValue && !loopEndGrade.HasValue)
            {
                throw new AFBizException("组织线层层审批找人时，两个入参都为空！");
            }

            List<BaseIdTranStruVo> baseIdTranStruVos = null;
            if (loopNumberPlies.HasValue)
            {
                baseIdTranStruVos = userService.QueryLeadersByEmployeeIdAndTier(startUserId, loopNumberPlies.Value);
                if (baseIdTranStruVos == null || !baseIdTranStruVos.Any())
                {
                    throw new AFBizException("未能根据发起人找到层层审批人信息");
                }
            }
            if (loopEndGrade.HasValue)
            {
                baseIdTranStruVos = userService.QueryLeadersByEmployeeIdAndGrade(startUserId, loopEndGrade.Value);
                if (baseIdTranStruVos == null || !baseIdTranStruVos.Any())
                {
                    throw new AFBizException("未能根据发起人找到汇报线审批人信息");
                }
            }
            if (baseIdTranStruVos == null || !baseIdTranStruVos.Any())
            {
                throw new AFBizException("未能根据发起人找到审批人信息");
            }

            //loopEndPersonList 跳过"人",不跳过"层":遇到 endPerson 就跳过他本人,层里其他人继续
            var finalApproverIds = new List<string>();
            foreach (var vo in baseIdTranStruVos)
            {
                if (!loopEndPersonList.Contains(vo.Id))
                {
                    finalApproverIds.Add(vo.Id);
                }
            }

            //扁平 list 包装成 [[a],[b],[c]]:每层 1 人,行为与改造前一致
            var result = new List<List<string>>();
            foreach (var approverId in finalApproverIds)
            {
                result.Add(new List<string> { approverId });
            }

            return result;
        }

        public override void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(OrderNodeTypeEnum.LOOP_NODE);
        }
    }
