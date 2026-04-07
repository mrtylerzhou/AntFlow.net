using AntFlowCore.Common.util;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Entity;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

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

        BpmnNodeSignUpConf bpmnNodeSignUpConf = new BpmnNodeSignUpConf
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