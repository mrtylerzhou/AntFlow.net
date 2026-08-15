using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller
{
    /// <summary>
    /// 流程权限管理. 对应 Java BpmProcessPermissionsController.
    /// </summary>
    [Route("processPermissions")]
    public class ProcessPermissionsController
    {
        private readonly IProcessPermissionsService _processPermissionsService;

        public ProcessPermissionsController(IProcessPermissionsService processPermissionsService)
        {
            _processPermissionsService = processPermissionsService;
        }

        /// <summary>
        /// 分页列表
        /// </summary>
        [HttpPost("listPage")]
        public ResultAndPage<ProcessPermissionsListVo> ListPage([FromBody] ProcessPermissionsPageReq req)
        {
            return _processPermissionsService.ListPage(req);
        }

        /// <summary>
        /// 批量保存(流程×授权对象×权限类型 笛卡尔积, 已存在跳过)
        /// </summary>
        [HttpPost("save")]
        public Result<ProcessPermissionsSaveResult> Save([FromBody] ProcessPermissionsSaveVo vo)
        {
            return ResultHelper.Success(_processPermissionsService.Save(vo));
        }

        /// <summary>
        /// 删除(物理)
        /// </summary>
        [HttpGet("delete/{id}")]
        public Result<string> Delete([FromRoute] long id)
        {
            _processPermissionsService.Delete(id);
            return ResultHelper.Success("ok");
        }
    }
}