using antflowcore.constant.enus;
using antflowcore.dto;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Mvc;

namespace antflowcore.controller;

[Route("bpmnBusiness")]
public class BpmBusinessController
{
    private readonly TaskMgmtService _taskMgmtService;
    private readonly UserEntrustService _userEntrustService;
    private readonly BpmnNodeService _bpmnNodeService;

   public BpmBusinessController(TaskMgmtService taskMgmtService,
        UserEntrustService userEntrustService,
        BpmnNodeService bpmnNodeService)
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