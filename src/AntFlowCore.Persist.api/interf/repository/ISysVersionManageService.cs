using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository
{
    /// <summary>
    /// App版本管理 业务服务接口. 对应 Java SysVersionServiceImpl(管理面).
    /// </summary>
    public interface ISysVersionManageService
    {
        /// <summary>
        /// App端升级检查(最新版本判断/强更/下载地址)
        /// </summary>
        AppVersionVo GetAppVersion(string application, string appVersion);

        /// <summary>
        /// App下载二维码(maxIndex已发布版本的downloadCode)
        /// </summary>
        SysVersionVo GetDownloadQrCode();

        /// <summary>
        /// 版本分页列表(is_del=0, id倒序)
        /// </summary>
        ResultAndPage<SysVersionVo> ListSysVersion(int page, int pageSize, string version);

        /// <summary>
        /// 保存版本(新增草稿/编辑; 新增支持inheritFromLast; 版本号唯一校验; 已发布仅运营参数白名单)
        /// </summary>
        bool Edit(SysVersionVo vo);

        /// <summary>
        /// 发布草稿版本(is_hide 1->0)
        /// </summary>
        bool Publish(long id);

        /// <summary>
        /// 逻辑删除草稿版本并级联清理关联数据
        /// </summary>
        bool DeleteDraft(long id);

        /// <summary>
        /// 全量替换保存版本关联数据(仅草稿可用)
        /// </summary>
        bool SaveAppDatas(AppDataSaveVo vo);

        /// <summary>
        /// 候选对象列表(1/2:图标应用与流程 3:快捷入口)
        /// </summary>
        List<BaseIdTranStruVo> GetCandidates(int type, string search, int limitSize);

        /// <summary>
        /// 查询版本已关联数据(按sort排序)
        /// </summary>
        List<AppDataSaveVo.AppDataItem> GetAppDatas(long versionId, int type);
    }
}