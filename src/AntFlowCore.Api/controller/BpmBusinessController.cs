using AntFlowCore.Abstraction;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.exception;
using AntFlowCore.Common.util;
using AntFlowCore.Core;
using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Engine.service.biz;
using AntFlowCore.Entity;
using AntFlowCore.Extensions;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

[Route("bpmnBusiness")]
public class BpmBusinessController
{
    private readonly ITaskMgmtService _taskMgmtService;
    private readonly IUserEntrustService _userEntrustService;
    private readonly IBpmnNodeService _bpmnNodeService;

   public BpmBusinessController(ITaskMgmtService taskMgmtService,
        IUserEntrustService userEntrustService,
        IBpmnNodeService bpmnNodeService)
    {
        _taskMgmtService = taskMgmtService;
        _userEntrustService = userEntrustService;
        _bpmnNodeService = bpmnNodeService;
    }
    
    /// <summary>
    /// 获取自定义表单DIY FormCode List
    /// </summary>
    /// <param name="desc"></param>
    /// <returns></returns>
    [HttpGet("getDIYFormCodeList")]
    public Result<List<DIYProcessInfoDTO>> GetDIYFormCodeList(String desc) {
        List<DIYProcessInfoDTO> diyProcessInfoDTOS = _taskMgmtService.ViewProcessInfo(desc);
        return ResultHelper.Success(diyProcessInfoDTOS);
    }
   /// <summary>
   /// 获取委托列表
   /// </summary>
   /// <param name="requestDto"></param>
   /// <param name="type"></param>
   /// <returns></returns>
    [HttpPost("entrustlist/{type}")]
    public ResultAndPage<Entrust> EntrustList([FromBody] DetailRequestDto requestDto, [FromRoute] int type) {

        PageDto pageDto = requestDto.PageDto;
        Entrust vo = new Entrust();
        return _userEntrustService.GetEntrustPageList(pageDto, vo, type);
    }
   
   /// <summary>
   /// 获取委托详情
   /// </summary>
   /// <param name="id"></param>
   /// <returns></returns>
    [HttpGet("entrustDetail/{id}")]
    public Result<UserEntrust> EntrustDetail([FromRoute] int id) {
        UserEntrust detail = _userEntrustService.GetEntrustDetail(id);
        return ResultHelper.Success(detail);
    }
   
   /// <summary>
   /// 编辑委托
   /// </summary>
   /// <param name="dataVo"></param>
   /// <returns></returns>
    [HttpPost("editEntrust")]
    public Result<string> EditEntrust([FromBody] DataVo dataVo)
    {
        _userEntrustService.UpdateEntrustList(dataVo);
        return ResultHelper.Success("ok");
    }
   
   /// <summary>
   /// 获取流程自选审批人节点
   /// </summary>
   /// <param name="formCode"></param>
   /// <returns></returns>
   /// <exception cref="AFBizException"></exception>
    [HttpGet("getStartUserChooseModules")]
    public Result<List<BpmnNodeVo>> GetStartUserChooseModules([FromQuery] string formCode)
    {
        if (string.IsNullOrWhiteSpace(formCode))
        {
            throw new AFBizException("参数formCode不能为空!");
        }
        List<BpmnNode> nodes = _bpmnNodeService.GetNodesByFormCodeAndProperty(
            formCode, (int)NodePropertyEnum.NODE_PROPERTY_CUSTOMIZE
        );
        List<BpmnNodeVo> nodeVos = nodes.Select(a => new BpmnNodeVo
        {
            Id = a.Id,
            NodeName = a.NodeName
        }).ToList();

        return ResultHelper.Success(nodeVos);
    }
}