using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Repository;

public class BpmnNodeSignUpConfService : AFBaseCurdRepositoryService<BpmnNodeSignUpConf>, IBpmnNodeSignUpConfService
{
    public BpmnNodeSignUpConfService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void EditSignUpConf(BpmnNodeVo bpmnNodeVo, long bpmnNodeId)
    {
        if (bpmnNodeVo.IsSignUp != 1)
        {
            return;
        }

        BpmnNodeSignUpConf bpmnNodeSignUpConf = new()
        {
            BpmnNodeId = bpmnNodeId,
            AfterSignUpWay = bpmnNodeVo.Property.AfterSignUpWay,
            SignUpType = bpmnNodeVo.Property.SignUpType,
            CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
            Remark = "",
            CreateTime = DateTime.Now
        };
        baseRepo.Insert(bpmnNodeSignUpConf);
    }
}