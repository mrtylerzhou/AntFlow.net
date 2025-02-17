using AntFlowCore.Entity;
using antflowcore.exception;

namespace antflowcore.service.repository;

public class OutSideBpmCallbackUrlConfService: AFBaseCurdRepositoryService<OutSideBpmCallbackUrlConf>
{
    public OutSideBpmCallbackUrlConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
    public OutSideBpmCallbackUrlConf GetOutSideBpmCallbackUrlConf(int businessPartyId) {

        OutSideBpmCallbackUrlConf outSideBpmCallbackUrlConf = baseRepo.Where(a=>a.BusinessPartyId == businessPartyId&&a.Status==1).First();
        
        if (outSideBpmCallbackUrlConf==null) {
            throw new AFBizException("流程回调URL未配置，方法执行失败");
        }

        return outSideBpmCallbackUrlConf;
    }
}