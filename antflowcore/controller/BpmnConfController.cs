using System.Text.Json;
using System.Text.Json.Nodes;
using antflowcore.adaptor;
using antflowcore.dto;
using AntFlowCore.Entity;
using antflowcore.factory;
using antflowcore.http;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util;
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
    /// <summary>
    /// 流程模板新增/编辑
    /// </summary>
    /// <param name="bpmnConfVo"></param>
    /// <returns></returns>
    [HttpPost("Edit")]
    public Result<String> Edit([FromBody]BpmnConfVo bpmnConfVo)
    {
        _freeSql.Ado.Transaction(()=> _bpmnConfBizService.Edit(bpmnConfVo));
        return Result<string>.Succ("ok");
    }

    /// <summary>
    /// 流程操作核心方法,流程同意,不同意,拒绝,加拒,变更处理人等出口都在此方法,通过策略模板实现处理逻辑分发
    /// </summary>
    /// <param name="accessor"></param>
    /// <param name="formCode"></param>
    /// <returns></returns>
    [HttpPost("process/buttonsOperation")]
    public  Result<BusinessDataVo> ButtonsOperation([FromServices] IHttpContextAccessor accessor,[FromQuery] String formCode)
    {
        string values = accessor.HttpContext!.ReadRawBodyAsString();
        //BusinessDataVo vo=JsonSerializer.Deserialize<BusinessDataVo>(values);
        BusinessDataVo dataVo = _processApprovalService.ButtonsOperation(values,formCode);
        return Result<BusinessDataVo>.Succ(dataVo);
    }

    /// <summary>
    /// 流程模板列表
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("listPage")]
    public Result<ResultAndPage<BpmnConfVo>> ListPage([FromBody] PageRequestDto<BpmnConfVo> dto)
    {
        PageDto page = dto.PageDto;
        BpmnConfVo vo = dto.Entity;
        return Result<ResultAndPage<BpmnConfVo>>.Succ(_bpmnConfBizService.SelectPage(page, vo));
    }
    
    /// <summary>
    /// 流程预览接口
    /// </summary>
    /// <param name="accessor"></param>
    /// <returns></returns>
    [HttpPost("preview")]
    public Result<PreviewNode> Preview([FromServices] IHttpContextAccessor accessor)
    {
        string values = accessor.HttpContext!.ReadRawBodyAsString();
        PreviewNode previewNode = _bpmnConfCommonService.PreviewNode(values);
        return Result<PreviewNode>.Succ(previewNode);
    }
    
    /// <summary>
    /// 发起页预览接口
    /// </summary>
    /// <param name="accessor"></param>
    /// <returns></returns>
    [HttpPost("startPagePreviewNode")]
    public Result<PreviewNode> StartPagePreviewNode([FromServices] IHttpContextAccessor accessor)
    {
        string paramsJson = accessor.HttpContext!.ReadRawBodyAsString();
        JsonNode? jsonObject = JsonNodeHelper.SafeParse(paramsJson);
        bool isStartPreview = jsonObject?["isStartPreview"]?.GetValue<bool>() ?? false;

        if (isStartPreview)
        {
            return Result<PreviewNode>.Succ(_bpmnConfCommonService.StartPagePreviewNode(paramsJson));
        }

        return Result<PreviewNode>.Succ(_bpmnConfCommonService.TaskPagePreviewNode(paramsJson));
    }
    
    /// <summary>
    /// 查看流程审批路径
    /// </summary>
    /// <param name="processNumber"></param>
    /// <returns></returns>
    [HttpGet("getBpmVerifyInfoVos")]
    public Result<List<BpmVerifyInfoVo>> GetBpmVerifyInfoVos(String processNumber) {
        return Result<List<BpmVerifyInfoVo>>.Succ(_bpmVerifyInfoBizService.GetBpmVerifyInfoVos(processNumber, false));
    }
    [HttpPost("process/viewBusinessProcess")]
    public Result<dynamic> ViewBusinessProcess( [FromServices] IHttpContextAccessor accessor, String formCode) {
        string values = accessor.HttpContext!.ReadRawBodyAsString();
        return Result<dynamic>.Succ(_processApprovalService.GetBusinessInfo(values, formCode));
    }
    
    /// <summary>
    /// 流程我的待办/已办等列表页面出口方法,此方法根据type区分请求类型,但是没有使用策略模板,而是简单的switch case,这里面没有很复杂的逻辑,基本上都是稍复杂一些的查询
    /// </summary>
    /// <param name="type"></param>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    [HttpPost("process/listPage/{type}")]
    public ResultAndPage<TaskMgmtVO> ViewPcProcessList([FromRoute] int type, [FromBody] DetailRequestDto requestDto)
    {
        PageDto pageDto = requestDto.PageDto;
        TaskMgmtVO taskMgmtVO = requestDto.TaskMgmtVO;
        taskMgmtVO.Type=type;
        return _processApprovalService.FindPcProcessList(pageDto, taskMgmtVO);
    }

    /// <summary>
    /// 查看流程模板详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [AcceptVerbs("GET", "POST")]
    [Route("detail/{id}")]
    public Result<BpmnConfVo> Detail([FromRoute] long id)
    {
        return Result<BpmnConfVo>.Succ(_bpmnConfBizService.Detail(id));
    }
    /// <summary>
    /// 生效一个流程模板,流程模板配置以后,默认是不生效的,要在页面上点击生效按钮方可生效
    /// 为什么要这样设计?因为新设计的流程模板设计完之后管理员还要核对,看看是否要问题,如果设计完直接生效有错误就没法反悔了
    /// antflow流程设计可以交给非专业人员来做,完全不需要程序员参与.一般的新的流程上线前,流程管理员就会配置上流程模板,待后端上线以后发布,并且灰度试行(怎么设计灰度?联系作者交流)
    /// antflow流程模板相当于传统审批流里面的流程图
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("effectiveBpmn/{id}")]
    public Result<bool> EffectiveBpmn( int id) {
        _bpmnConfService.EffectiveBpmnConf(id);
        return Result<bool>.Succ(true);
    }
    /// <summary>
    /// 通用用于办公界面首页流程相关的统计信息,用户非必须使用,可以酌情考虑
    /// </summary>
    /// <returns></returns>
    [HttpGet("todoList")]
    public Result<TaskMgmtVO> TodoList() {
        TaskMgmtVO taskMgmtVO = _processApprovalService.ProcessStatistics();
        return ResultHelper.Success(taskMgmtVO);
    }
}