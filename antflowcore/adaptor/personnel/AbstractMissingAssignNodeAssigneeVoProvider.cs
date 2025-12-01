using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enums;
using antflowcore.exception;
using antflowcore.util.Extension;
using antflowcore.vo;

namespace antflowcore.adaptor.personnel;

public abstract class AbstractMissingAssignNodeAssigneeVoProvider: AbstractNodeAssigneeVoProvider, IMissAssigneeProcessing
{
    protected readonly IBpmnProcessAdminProvider _processAdminProvider;

    protected AbstractMissingAssignNodeAssigneeVoProvider(AssigneeVoBuildUtils assigneeVoBuildUtils,
    IBpmnProcessAdminProvider processAdminProvider) : base(assigneeVoBuildUtils)
    {
        _processAdminProvider = processAdminProvider;
    }

    protected override List<BpmnNodeParamsAssigneeVo> ProvideAssigneeList(BpmnNodeVo nodeVo, ICollection<BaseIdTranStruVo> emplList)
    {
        int? missingAssigneeDealWay = nodeVo.NoHeaderAction;
        emplList = emplList.Where(a => a != null).ToList();
        if(!emplList.IsEmpty()){
            return base.ProvideAssigneeList(nodeVo, emplList);
        }
        
        missingAssigneeDealWay=missingAssigneeDealWay!=null?missingAssigneeDealWay:MissingAssigneeProcessStrategyEnum.NOTALLOWED.Code;
        BaseIdTranStruVo baseIdTranStruVo = ProcessMissAssignee(missingAssigneeDealWay);
        emplList.Add(baseIdTranStruVo);
        if (MissingAssigneeProcessStrategyEnum.GetByCode(missingAssigneeDealWay)?.Code==MissingAssigneeProcessStrategyEnum.SKIP.Code) {
            nodeVo.Params.IsNodeDeduplication=1;
        }
        return base.ProvideAssigneeList(nodeVo, emplList);
    }
    
    public BaseIdTranStruVo ProcessMissAssignee(int? processingWay){
        MissingAssigneeProcessStrategyEnum? processingStrategy = MissingAssigneeProcessStrategyEnum.GetByCode(processingWay);
        
        if(processingStrategy==null){
            return null;
        }

        if (processingStrategy.Code == MissingAssigneeProcessStrategyEnum.SKIP.Code)
        {
            return new BaseIdTranStruVo
            {
                Id = AFSpecialAssigneeEnum.TO_BE_REMOVED.Id,
                Name = AFSpecialAssigneeEnum.TO_BE_REMOVED.Desc
            };
        }else if (processingStrategy.Code == MissingAssigneeProcessStrategyEnum.TRANSFERTOADMIN.Code)
        {
            BaseIdTranStruVo processAdminAndOutsideProcess = _processAdminProvider.ProvideProcessAdminInfo();
            return processAdminAndOutsideProcess;
        }else if (processingStrategy.Code == MissingAssigneeProcessStrategyEnum.NOTALLOWED.Code)
        {
            throw new AFBizException("current not has no assignee!");
        }
        else
        {
            return null;
        }
    }
}