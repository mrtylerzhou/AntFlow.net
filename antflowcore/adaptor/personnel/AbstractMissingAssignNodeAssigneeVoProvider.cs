using antflowcore.adaptor.personnel.provider;
using antflowcore.constant.enums;
using antflowcore.constant.enus;
using antflowcore.exception;
using antflowcore.service.biz;
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
        if (nodeVo.Property != null)
        {
            var property = nodeVo.Property;
            List<ExtraSignInfoVo> additionalSignInfoList = property.AdditionalSignInfoList;
            List<BaseIdTranStruVo> additionalAssigneeListToAdd=null;
            List<BaseIdTranStruVo> additionalAssigneeListToDel=null;
            if (!additionalSignInfoList.IsEmpty())
            {
                foreach (ExtraSignInfoVo signInfoVo in additionalSignInfoList)
                {
                    int? nodeProperty = signInfoVo.NodeProperty;
                    int? propertyType = signInfoVo.PropertyType;
                    NodePropertyEnum nodePropertyEnum = (NodePropertyEnum)nodeProperty;
                    switch (nodePropertyEnum)
                    {
                        case NodePropertyEnum.NODE_PROPERTY_ROLE:
                            List<BaseIdTranStruVo> roleList = signInfoVo.OtherSignInfos;
                            if(propertyType==1){//add
                                if(additionalAssigneeListToAdd==null){
                                    additionalAssigneeListToAdd=new List<BaseIdTranStruVo>();
                                }
                                additionalAssigneeListToAdd.AddRange(roleList);
                            }else if (propertyType==2){//del
                                if(additionalAssigneeListToDel==null){
                                    additionalAssigneeListToDel=new List<BaseIdTranStruVo>();
                                }
                                additionalAssigneeListToDel.AddRange(roleList);
                            }
                            break;
                        case NodePropertyEnum.NODE_PROPERTY_PERSONNEL:
                            List<BaseIdTranStruVo> personnelList = signInfoVo.SignInfos;
                            if(propertyType==1) {//add
                                if(additionalAssigneeListToAdd==null){
                                    additionalAssigneeListToAdd=new List<BaseIdTranStruVo>();
                                }
                                additionalAssigneeListToAdd.AddRange(personnelList);
                            }else if (propertyType==2){//del
                                if(additionalAssigneeListToDel==null){
                                    additionalAssigneeListToDel=new List<BaseIdTranStruVo>();
                                }
                                additionalAssigneeListToDel.AddRange(personnelList);
                            }
                            break;
                    }
                }
            }
            if(!additionalAssigneeListToAdd.IsEmpty())
            {
                foreach (BaseIdTranStruVo addsign in additionalAssigneeListToAdd)
                {
                    emplList.Add(addsign);
                }
            }
            if(!additionalAssigneeListToDel.IsEmpty()){
                foreach (BaseIdTranStruVo toDel in additionalAssigneeListToDel)
                {
                    List<BaseIdTranStruVo> toDelList = emplList.Where(a=>a.Id==toDel.Id).ToList();
                       
                    if(!toDelList.IsEmpty()){
                        foreach (BaseIdTranStruVo delSign in toDelList)
                        {
                            emplList.Remove(delSign);
                        }
                    }
                }
            }
        }

        emplList = 
        #if NET6_0_OR_GREATER
            emplList.DistinctBy(a => a.Id).ToList();
        #else
            emplList.Distinct(new BaseIdStructVoComparer()).ToList();
        #endif
       
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