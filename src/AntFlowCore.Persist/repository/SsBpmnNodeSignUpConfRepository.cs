using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnNodeSignUpConfRepository : RepositoryBase<BpmnNodeSignUpConf>, IBpmnNodeSignUpConfRepository
{
    public SsBpmnNodeSignUpConfRepository(AntFlowOrmContext context) : base(context)
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
        Db.Insertable(bpmnNodeSignUpConf).ExecuteCommand();
    }
}
