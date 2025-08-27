using AntFlow.Core.Configuration.DependencyInjection;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel.Provider;

[NamedService(nameof(LevelPersonnelProvider))]
public class LevelPersonnelProvider : AbstractNodeAssigneeVoProvider
{
    private readonly UserService _userService;

    public LevelPersonnelProvider(UserService userService, AssigneeVoBuildUtils assigneeVoBuildUtils) : base(
        assigneeVoBuildUtils)
    {
        _userService = userService;
    }

    public override List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo startConditionsVo)
    {
        BpmnNodePropertysVo? propertysVo = bpmnNodeVo.Property;
        string startUserId = startConditionsVo.StartUserId;

        if (propertysVo == null || propertysVo.AssignLevelGrade == null || startUserId == null)
        {
            throw new AFBizException("ָ���㼶����������ȫ���޷����ˣ�");
        }

        int assignLevelType = propertysVo.AssignLevelType.GetValueOrDefault();
        int assignLevelGrade = propertysVo.AssignLevelGrade.GetValueOrDefault();

        BaseIdTranStruVo baseIdTranStruVo = _userService.QueryLeaderByEmployeeIdAndLevel(startUserId, assignLevelGrade);

        if (baseIdTranStruVo == null)
        {
            throw new AFBizException("δ�ܸ��ݷ����˺�ָ���㼶�ҵ���������Ϣ");
        }

        List<string>? userIds = new() { baseIdTranStruVo.Id };
        return ProvideAssigneeList(bpmnNodeVo, userIds);
    }
}