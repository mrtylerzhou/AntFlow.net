using System.Runtime.InteropServices.JavaScript;
using AntFlowCore.Entity;
using antflowcore.util;
using antflowcore.vo;
using FreeSql;

namespace antflowcore.service.repository;

public class BpmnNodeSignUpConfService: AFBaseCurdRepositoryService<BpmnNodeSignUpConf>
{
    public BpmnNodeSignUpConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
    public void EditSignUpConf(BpmnNodeVo bpmnNodeVo, long bpmnNodeId) {
        if (bpmnNodeVo.IsSignUp != 1) {
            return;
        }

        BpmnNodeSignUpConf bpmnNodeSignUpConf = new BpmnNodeSignUpConf
        {
            BpmnNodeId = bpmnNodeId,
            AfterSignUpWay = bpmnNodeVo.Property.AfterSignUpWay,
            SignUpType = bpmnNodeVo.Property.SignUpType,
            CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
            Remark = "",
            CreateTime =   DateTime.Now
        };
        baseRepo.Insert(bpmnNodeSignUpConf);
        
    }
}