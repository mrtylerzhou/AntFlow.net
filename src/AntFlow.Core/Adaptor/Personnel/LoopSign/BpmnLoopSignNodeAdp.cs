using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel;

public class BpmnLoopSignNodeAdp : AbstractOrderedSignNodeAdp
{
    private readonly UserService userService;


    public BpmnLoopSignNodeAdp(UserService userService, AssigneeVoBuildUtils assigneeVoBuildUtils) : base(
        assigneeVoBuildUtils)
    {
        this.userService = userService;
    }

    public override List<string> GetAssigneeIds(BpmnNodeVo nodeVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        BpmnNodePropertysVo? propertysVo = nodeVo.Property;
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
        HashSet<string>? loopEndPersonList = new();
        if (propertysVo.LoopEndPersonList != null && propertysVo.LoopEndPersonList.Any())
        {
            foreach (string? s in propertysVo.LoopEndPersonList)
            {
                loopEndPersonList.Add(s);
            }
        }

        if (!loopNumberPlies.HasValue && !loopEndGrade.HasValue)
        {
            throw new AFBizException("��֯�߲����������ʱ��������ζ�Ϊ�գ�");
        }

        List<BaseIdTranStruVo> baseIdTranStruVos = null;
        if (loopNumberPlies.HasValue)
        {
            baseIdTranStruVos = userService.QueryLeadersByEmployeeIdAndTier(startUserId, loopNumberPlies.Value);
            if (baseIdTranStruVos == null || !baseIdTranStruVos.Any())
            {
                throw new AFBizException("δ�ܸ��ݷ������ҵ������������Ϣ");
            }
        }

        if (loopEndGrade.HasValue)
        {
            baseIdTranStruVos = userService.QueryLeadersByEmployeeIdAndGrade(startUserId, loopEndGrade.Value);
            if (baseIdTranStruVos == null || !baseIdTranStruVos.Any())
            {
                throw new AFBizException("δ�ܸ��ݷ������ҵ��㱨����������Ϣ");
            }
        }

        if (baseIdTranStruVos == null || !baseIdTranStruVos.Any())
        {
            throw new AFBizException("δ�ܸ��ݷ������ҵ���������Ϣ");
        }

        List<string>? approverIds = baseIdTranStruVos.Select(vo => vo.Id).ToList();
        List<string>? finalApproverIds = new();

        foreach (string? approverId in approverIds)
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