using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class FsBpmnNodeSignUpConfRepository : RepositoryBase<BpmnNodeSignUpConf>, IBpmnNodeSignUpConfRepository
{
    public FsBpmnNodeSignUpConfRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void EditSignUpConf(BpmnNodeVo bpmnNodeVo, long bpmnNodeId)
    {
        if (bpmnNodeVo.IsSignUp != 1)
        {
            return;
        }

        BpmnNodeSignUpConf bpmnNodeSignUpConf = new BpmnNodeSignUpConf
        {
            BpmnNodeId = bpmnNodeId,
            AfterSignUpWay = bpmnNodeVo.Property.AfterSignUpWay,
            SignUpType = bpmnNodeVo.Property.SignUpType,
            CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
            Remark = "",
            CreateTime = DateTime.Now
        };
        Add(bpmnNodeSignUpConf);
    }
}
