using System.Text.Json;
using antflowcore.adaptor;
using AntFlowCore.Entity;
using antflowcore.factory;
using antflowcore.http;
using antflowcore.service.biz;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace antflowcore.controller;

[Route("BpmnConf")]
public class BpmnConfController
{
    private readonly ProcessApprovalService _processApprovalService;
    private readonly IFreeSql _freeSql;
    public BpmnConfBizService _bpmnConfBizService;

    public BpmnConfController(
        BpmnConfBizService bpmnConfBizService,
        ProcessApprovalService processApprovalService,
        IFreeSql freeSql
        )
    {
        _processApprovalService = processApprovalService;
        _freeSql = freeSql;
        _bpmnConfBizService = bpmnConfBizService;
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
  
}