using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Http;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace AntFlow.Core.Controller;

[Route("BpmnConf")]
public class BpmnConfController
{
    private readonly BpmnConfCommonService _bpmnConfCommonService;
    private readonly BpmnConfService _bpmnConfService;
    private readonly BpmVerifyInfoBizService _bpmVerifyInfoBizService;
    private readonly IFreeSql _freeSql;
    private readonly ProcessApprovalService _processApprovalService;
    public BpmnConfBizService _bpmnConfBizService;

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
    ///     ����ģ������/�༭
    /// </summary>
    /// <param name="bpmnConfVo"></param>
    /// <returns></returns>
    [HttpPost("Edit")]
    public Result<string> Edit([FromBody] BpmnConfVo bpmnConfVo)
    {
        _freeSql.Ado.Transaction(() => _bpmnConfBizService.Edit(bpmnConfVo));
        return Result<string>.Succ("ok");
    }

    /// <summary>
    ///     ���̲������ķ���,����ͬ��,��ͬ��,�ܾ�,�Ӿ�,��������˵ȳ��ڶ��ڴ˷���?ͨ������ģ��ʵ�ִ����߼��ַ�
    /// </summary>
    /// <param name="accessor"></param>
    /// <param name="formCode"></param>
    /// <returns></returns>
    [HttpPost("process/buttonsOperation")]
    public Result<BusinessDataVo> ButtonsOperation([FromServices] IHttpContextAccessor accessor,
        [FromQuery] string formCode)
    {
        string values = accessor.HttpContext!.ReadRawBodyAsString();
        //BusinessDataVo vo=JsonSerializer.Deserialize<BusinessDataVo>(values);
        BusinessDataVo dataVo = _processApprovalService.ButtonsOperation(values, formCode);
        return Result<BusinessDataVo>.Succ(dataVo);
    }

    /// <summary>
    ///     ����ģ���б�
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
    ///     ����Ԥ���ӿ�
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
    ///     ����ҳԤ���ӿ�
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
    ///     查看流程审批路径
    /// </summary>
    /// <param name="processNumber"></param>
    /// <returns></returns>
    [HttpGet("getBpmVerifyInfoVos")]
    public Result<List<BpmVerifyInfoVo>> GetBpmVerifyInfoVos(string processNumber)
    {
        return Result<List<BpmVerifyInfoVo>>.Succ(_bpmVerifyInfoBizService.GetBpmVerifyInfoVos(processNumber, false));
    }

    [HttpPost("process/viewBusinessProcess")]
    public Result<dynamic> ViewBusinessProcess([FromServices] IHttpContextAccessor accessor, string formCode)
    {
        string values = accessor.HttpContext!.ReadRawBodyAsString();
        return Result<dynamic>.Succ(_processApprovalService.GetBusinessInfo(values, formCode));
    }

    /// <summary>
    ///     查询我的待办/已办列表页面用于返回,此方法根据type来区分不同类型,这里没有使用策略模式,而是简单的switch case,因为这里没有很复杂的逻辑,所以暂时可以改进一些的查询
    /// </summary>
    /// <param name="type"></param>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    [HttpPost("process/listPage/{type}")]
    public ResultAndPage<TaskMgmtVO> ViewPcProcessList([FromRoute] int type, [FromBody] DetailRequestDto requestDto)
    {
        PageDto pageDto = requestDto.PageDto;
        TaskMgmtVO taskMgmtVO = requestDto.TaskMgmtVO;
        taskMgmtVO.Type = type;
        return _processApprovalService.FindPcProcessList(pageDto, taskMgmtVO);
    }

    /// <summary>
    ///     查看流程模板详情
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
    ///     生效一个流程模板,流程模板创建之后,默认是不生效的,要在页面上点击生效按钮才生效
    ///     为什么要这样设计?因为流程频繁的修改模板,修改之后需要员工要审核?审核是否要生效,审核完成后直接生效或待审核完成后再生效
    ///     antflow流程设计可以交给专业的员工设计?安全起见要审核员工设计.一般流程的流程设计完成?流程管理员就会设计流程模板,流程设计完成后发布
    ///     就像一叶子流程(怎么设计叶子?联系技术解决)
    ///     antflow流程模板相当于传统的流程设计器设计流程
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("effectiveBpmn/{id}")]
    public Result<bool> EffectiveBpmn(int id)
    {
        _bpmnConfService.EffectiveBpmnConf(id);
        return Result<bool>.Succ(true);
    }

    /// <summary>
    ///     通过用于办公系统首页面返回的统计数据?用户是必须使用,所以这里考虑
    /// </summary>
    /// <returns></returns>
    [HttpGet("todoList")]
    public Result<TaskMgmtVO> TodoList()
    {
        TaskMgmtVO taskMgmtVO = _processApprovalService.ProcessStatistics();
        return ResultHelper.Success(taskMgmtVO);
    }
}