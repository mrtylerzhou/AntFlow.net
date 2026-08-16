using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller
{
    /// <summary>
    /// 字典管理. 对应 Java DictDataController(/dictData).
    /// 规则: lowcodeflow 系统数据禁止编辑/删除; dict_type+dict_label+dict_value 唯一性校验.
    /// </summary>
    [Route("dictData")]
    public class DictDataController
    {
        private readonly IDictDataBizService _dictDataBizService;

        public DictDataController(IDictDataBizService dictDataBizService)
        {
            _dictDataBizService = dictDataBizService;
        }

        /// <summary>
        /// 分页列表
        /// </summary>
        [HttpPost("listPage")]
        public ResultAndPage<DictDataVo> ListPage([FromBody] DictDataPageReq req)
        {
            return _dictDataBizService.ListPage(req);
        }

        /// <summary>
        /// 新增
        /// </summary>
        [HttpPost("save")]
        public Result<long> Save([FromBody] DictDataSaveVo vo)
        {
            return ResultHelper.Success(_dictDataBizService.Save(vo));
        }

        /// <summary>
        /// 编辑
        /// </summary>
        [HttpPost("update")]
        public Result<string> Update([FromBody] DictDataSaveVo vo)
        {
            _dictDataBizService.Update(vo);
            return ResultHelper.Success("ok");
        }

        /// <summary>
        /// 删除(逻辑删除 is_del=1, lowcodeflow 系统数据拒绝)
        /// </summary>
        [HttpGet("delete/{id}")]
        public Result<string> Delete([FromRoute] long id)
        {
            _dictDataBizService.Delete(id);
            return ResultHelper.Success("ok");
        }
    }
}