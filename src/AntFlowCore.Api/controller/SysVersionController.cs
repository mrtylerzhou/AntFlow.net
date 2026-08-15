using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller
{
    /// <summary>
    /// App版本管理. 对应 Java SysVersionController(/appVersion).
    /// </summary>
    [Route("appVersion")]
    public class SysVersionController
    {
        private readonly ISysVersionManageService _sysVersionManageService;

        public SysVersionController(ISysVersionManageService sysVersionManageService)
        {
            _sysVersionManageService = sysVersionManageService;
        }

        /// <summary>
        /// App端升级检查
        /// </summary>
        [HttpGet("appVersion")]
        public Result<AppVersionVo> AppVersion([FromQuery] string application, [FromQuery] string appVersion)
        {
            AppVersionVo vo = _sysVersionManageService.GetAppVersion(application, appVersion);
            if (vo != null)
            {
                return ResultHelper.Success(vo);
            }
            return ResultHelper.Fail<AppVersionVo>("", "未找到应用版本信息", false, null);
        }

        /// <summary>
        /// App下载二维码
        /// </summary>
        [HttpGet("getQrCode")]
        public Result<SysVersionVo> GetQrCode()
        {
            return ResultHelper.Success(_sysVersionManageService.GetDownloadQrCode());
        }

        /// <summary>
        /// 版本分页列表
        /// </summary>
        [HttpGet("versionList")]
        public ResultAndPage<SysVersionVo> VersionList([FromQuery] int page = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string version = null)
        {
            return _sysVersionManageService.ListSysVersion(page, pageSize, version);
        }

        /// <summary>
        /// 更新版本基本信息(草稿全量/已发布仅运营参数, 服务端白名单校验)
        /// </summary>
        [HttpPost("{id}")]
        public Result<string> UpdateById([FromRoute] long id, [FromBody] SysVersionVo vo)
        {
            if (id == 0)
            {
                throw new AFBizException("400110", "id不能为空");
            }
            vo.Id = id;
            vo.UpdateUser = SecurityUtils.GetLogInEmpNameSafe();
            if (_sysVersionManageService.Edit(vo))
            {
                return ResultHelper.Success("ok");
            }
            throw new AFBizException("400111", "更新失败");
        }

        /// <summary>
        /// 保存系统版本(新增草稿, 支持inheritFromLast)
        /// </summary>
        [HttpPost("save")]
        public Result<string> Save([FromBody] SysVersionVo vo)
        {
            vo.CreateUser = SecurityUtils.GetLogInEmpNameSafe();
            vo.UpdateUser = SecurityUtils.GetLogInEmpNameSafe();
            if (_sysVersionManageService.Edit(vo))
            {
                return ResultHelper.Success("ok");
            }
            throw new AFBizException("400112", "保存失败");
        }

        /// <summary>
        /// 候选对象列表(1/2:图标应用与流程 3:快捷入口)
        /// </summary>
        [HttpGet("candidates")]
        public Result<List<BaseIdTranStruVo>> Candidates([FromQuery] int type,
            [FromQuery] string search = null, [FromQuery] int limitSize = 50)
        {
            return ResultHelper.Success(_sysVersionManageService.GetCandidates(type, search, limitSize));
        }

        /// <summary>
        /// 查询版本已关联数据(按sort排序)
        /// </summary>
        [HttpGet("appDatas")]
        public Result<List<AppDataSaveVo.AppDataItem>> AppDatas([FromQuery] long versionId, [FromQuery] int type)
        {
            return ResultHelper.Success(_sysVersionManageService.GetAppDatas(versionId, type));
        }

        /// <summary>
        /// 全量替换保存版本关联数据(仅草稿可用)
        /// </summary>
        [HttpPost("saveAppDatas")]
        public Result<string> SaveAppDatas([FromBody] AppDataSaveVo vo)
        {
            if (_sysVersionManageService.SaveAppDatas(vo))
            {
                return ResultHelper.Success("ok");
            }
            throw new AFBizException("400113", "保存失败");
        }

        /// <summary>
        /// 发布草稿版本
        /// </summary>
        [HttpPost("publish/{id}")]
        public Result<string> Publish([FromRoute] long id)
        {
            if (_sysVersionManageService.Publish(id))
            {
                return ResultHelper.Success("ok");
            }
            throw new AFBizException("400114", "发布失败");
        }

        /// <summary>
        /// 删除草稿版本(级联清理关联数据)
        /// </summary>
        [HttpPost("delete/{id}")]
        public Result<string> DeleteDraft([FromRoute] long id)
        {
            if (_sysVersionManageService.DeleteDraft(id))
            {
                return ResultHelper.Success("ok");
            }
            throw new AFBizException("400115", "删除失败");
        }
    }
}