using System.Text.Json;
using System.Text.Json.Nodes;
using antflowcore.adaptor;
using antflowcore.dto;
using AntFlowCore.Entity;
using antflowcore.factory;
using antflowcore.http;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace antflowcore.controller;

[Route("BpmnConf")]
public class BpmnConfController
{
    private readonly ProcessApprovalService _processApprovalService;
    private readonly BpmVerifyInfoBizService _bpmVerifyInfoBizService;
    private readonly BpmnConfService _bpmnConfService;
    private readonly IFreeSql _freeSql;
    public BpmnConfBizService _bpmnConfBizService;
    private readonly BpmnConfCommonService _bpmnConfCommonService;

    public BpmnConfController(
        BpmnConfBizService bpmnConfBizService,
        BpmnConfCommonService bpmnConfCommonService,
        ProcessApprovalService processApprovalService,
        BpmVerifyInfoBizService bpmVerifyInfoBizService,
        BpmnConfService bpmnConfService,
        IFreeSql freeSql
        )
    {
        _processApprovalService = processApprovalService;
        _bpmVerifyInfoBizService = bpmVerifyInfoBizService;
        _bpmnConfService = bpmnConfService;
        _freeSql = freeSql;
        _bpmnConfBizService = bpmnConfBizService;
        _bpmnConfCommonService = bpmnConfCommonService;
    }
    [HttpPost("Edit")]
    public Result<String> Edit([FromBody]BpmnConfVo bpmnConfVo)
    {
        _freeSql.Ado.Transaction(()=> _bpmnConfBizService.Edit(bpmnConfVo));
        return Result<string>.Succ("ok");
    }

    [HttpPost("process/buttonsOperation")]
    public  Result<BusinessDataVo> ButtonsOperation([FromServices] IHttpContextAccessor accessor,[FromQuery] String formCode)
    {
        string values = accessor.HttpContext!.ReadRawBodyAsString();
        //BusinessDataVo vo=JsonSerializer.Deserialize<BusinessDataVo>(values);
        BusinessDataVo dataVo = _processApprovalService.ButtonsOperation(values,formCode);
        return Result<BusinessDataVo>.Succ(dataVo);
    }

    [HttpPost("preview")]
    public Result<PreviewNode> Preview([FromServices] IHttpContextAccessor accessor)
    {
        string values = accessor.HttpContext!.ReadRawBodyAsString();
        PreviewNode previewNode = _bpmnConfCommonService.PreviewNode(values);
        return Result<PreviewNode>.Succ(previewNode);
    }
    [HttpPost("startPagePreviewNode")]
    public Result<PreviewNode> StartPagePreviewNode([FromBody] string paramsJson)
    {
        JsonNode? jsonObject = JsonNode.Parse(paramsJson);
        bool isStartPreview = jsonObject?["isStartPreview"]?.GetValue<bool>() ?? false;

        if (isStartPreview)
        {
            return Result<PreviewNode>.Succ(_bpmnConfCommonService.StartPagePreviewNode(paramsJson));
        }
        else
        {
            return Result<PreviewNode>.Succ(_bpmnConfCommonService.TaskPagePreviewNode(paramsJson));
        }
    }
    [HttpGet("getBpmVerifyInfoVos")]
    public Result<List<BpmVerifyInfoVo>> GetBpmVerifyInfoVos(String processNumber) {
        return Result<List<BpmVerifyInfoVo>>.Succ(_bpmVerifyInfoBizService.GetBpmVerifyInfoVos(processNumber, false));
    }
    [HttpPost("process/viewBusinessProcess")]
    public Result<BusinessDataVo> viewBusinessProcess( String values, String formCode) {
        return Result<BusinessDataVo>.Succ(_processApprovalService.GetBusinessInfo(values, formCode));
    }
    [HttpPost("process/listPage/{type}")]
    public ResultAndPage<TaskMgmtVO> ViewPcProcessList([FromRoute] int type, [FromBody] PageRequestDTO<TaskMgmtVO> requestDto)
    {
        PageDto pageDto = requestDto.PageDto;
        TaskMgmtVO taskMgmtVO = requestDto.Entity;
        taskMgmtVO.Type=type;
        return _processApprovalService.FindPcProcessList(pageDto, taskMgmtVO);
    }
   
    [HttpGet("effectiveBpmn/{id}")]
    public Result<bool> EffectiveBpmn( int id) {
        _bpmnConfService.EffectiveBpmnConf(id);
        return Result<bool>.Succ(true);
    }
}