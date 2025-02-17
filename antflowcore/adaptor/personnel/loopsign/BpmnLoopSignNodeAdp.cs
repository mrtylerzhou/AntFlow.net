using antflowcore.constant.enus;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel;



    public class BpmnLoopSignNodeAdp : AbstractOrderedSignNodeAdp
    {
        private readonly UserService userService;
        
        
        public BpmnLoopSignNodeAdp(UserService userService,AssigneeVoBuildUtils assigneeVoBuildUtils) : base(assigneeVoBuildUtils)
        {
            this.userService = userService;
        }

        public override List<string> GetAssigneeIds(BpmnNodeVo nodeVo, BpmnStartConditionsVo bpmnStartConditions)
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

            var approverIds = baseIdTranStruVos.Select(vo => vo.Id).ToList();
            var finalApproverIds = new List<string>();

            foreach (var approverId in approverIds)
            {
                if (!loopEndPersonList.Contains(approverId))
                {
                    finalApproverIds.Add(approverId);
                }
            }

            return finalApproverIds;
        }

        public override void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(OrderNodeTypeEnum.LOOP_NODE);
        }
    }

