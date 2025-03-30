using System.Web;
using AntFlowCore.Entity;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class ApplicationService: AFBaseCurdRepositoryService<BpmProcessAppApplication>
{
    public ApplicationService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public BpmProcessAppApplicationVo GetApplicationUrl(String businessCode, String processKey)
    {
        if (string.IsNullOrEmpty(businessCode) && string.IsNullOrEmpty(processKey))
        {
            return null;
        }

        List<BpmProcessAppApplication> list = this.baseRepo.Where(a=>a.BusinessCode.Equals(businessCode)&&a.ProcessKey.Equals(processKey)&&a.IsDel==0).ToList();
        if (ObjectUtils.IsEmpty(list))
        {
            return null;
        }
        
        BpmProcessAppApplication application = list[0];
        BpmProcessAppApplicationVo vo = new BpmProcessAppApplicationVo();
        mapper.Map(application, vo);
        if (!string.IsNullOrEmpty(vo.LookUrl)) {
            vo.LookUrl=HttpUtility.HtmlDecode(vo.LookUrl);
        }
        if (!string.IsNullOrEmpty(vo.SubmitUrl)) {
            vo.SubmitUrl=HttpUtility.HtmlDecode(vo.SubmitUrl);
        }
        if (!string.IsNullOrEmpty(vo.ConditionUrl)) {
            vo.ConditionUrl=HttpUtility.HtmlDecode(vo.ConditionUrl);
        }

        return vo;
    }

    public List<BpmProcessAppApplication> SelectApplicationList()
    {
        List<BpmProcessAppApplication> bpmProcessAppApplications = this.baseRepo.Where(a=>true).ToList();
        return bpmProcessAppApplications;
    }
}