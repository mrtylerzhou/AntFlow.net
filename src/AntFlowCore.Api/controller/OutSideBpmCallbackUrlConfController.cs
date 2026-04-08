using AntFlowCore.Abstraction;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Common.util;
using AntFlowCore.Core;
using AntFlowCore.Core.entity;
using AntFlowCore.Extensions;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Api.controller;


    [Route("outSideBpm")]
    public class OutSideBpmCallbackUrlConfController
    {
        private readonly ILogger<OutSideBpmCallbackUrlConfController> _logger;
        private readonly IOutSideBpmCallbackUrlConfService _outSideBpmCallbackUrlConfService;

        public OutSideBpmCallbackUrlConfController(
            ILogger<OutSideBpmCallbackUrlConfController> logger,
            IOutSideBpmCallbackUrlConfService outSideBpmCallbackUrlConfService)
        {
            _logger = logger;
            _outSideBpmCallbackUrlConfService = outSideBpmCallbackUrlConfService;
        }

        /// <summary>
        /// 根据表单编码查询回调配置列表
        /// </summary>
        [HttpGet("callbackUrlConf/list/{formCode}")]
        public Result<List<OutSideBpmCallbackUrlConf>> List(string formCode)
        {
            var result = _outSideBpmCallbackUrlConfService.SelectListByFormCode(formCode);
            return ResultHelper.Success(result);
        }

       
        /// <summary>
        /// 查询指定回调配置详情
        /// </summary>
        [HttpGet("callbackUrlConf/detail/{id}")]
        public Result<OutSideBpmCallbackUrlConfVo> Detail(int id)
        {
            var result = _outSideBpmCallbackUrlConfService.Detail(id);
            return ResultHelper.Success(result);
        }

        /// <summary>
        /// 编辑回调配置
        /// </summary>
        [HttpPost("callbackUrlConf/edit")]
        public Result<string> Edit([FromBody] OutSideBpmCallbackUrlConfVo vo)
        {
            _outSideBpmCallbackUrlConfService.Edit(vo);
            return ResultHelper.Success("ok");
        }
    }