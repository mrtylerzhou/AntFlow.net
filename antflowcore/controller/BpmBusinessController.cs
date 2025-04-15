using antflowcore.dto;
using AntFlowCore.Entity;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Mvc;

namespace antflowcore.controller;

[Route("BpmnConf")]
public class BpmBusinessController
{
    private readonly TaskMgmtService _taskMgmtService;
    private readonly UserEntrustService _userEntrustService;

    BpmBusinessController(TaskMgmtService taskMgmtService,
        UserEntrustService userEntrustService)
    {
        _taskMgmtService = taskMgmtService;
        _userEntrustService = userEntrustService;
    }
    [HttpGet("GetDIYFormCodeList")]
    public Result<List<DIYProcessInfoDTO>> GetDIYFormCodeList(String desc) {
        List<DIYProcessInfoDTO> diyProcessInfoDTOS = _taskMgmtService.ViewProcessInfo(desc);
        return ResultHelper.Success(diyProcessInfoDTOS);
    }
   
    [HttpPost("entrustlist/{type}")]
    public ResultAndPage<Entrust> EntrustList([FromBody] DetailRequestDto requestDto, [FromRoute] int type) {

        PageDto pageDto = requestDto.PageDto;
        Entrust vo = new Entrust();
        return _userEntrustService.GetEntrustPageList(pageDto, vo, type);
    }
    [HttpGet("entrustDetail/{id}")]
    public Result<UserEntrust> EntrustDetail([FromRoute] int id) {
        UserEntrust detail = _userEntrustService.GetEntrustDetail(id);
        return ResultHelper.Success(detail);
    }
    [HttpPost("editEntrust")]
    public Result<string> EditEntrust([FromBody] DataVo dataVo)
    {
        _userEntrustService.UpdateEntrustList(dataVo);
        return ResultHelper.Success("ok");
    }
}