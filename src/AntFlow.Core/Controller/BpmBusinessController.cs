using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlow.Core.Controller;

[Route("bpmnBusiness")]
public class BpmBusinessController
{
    private readonly BpmnNodeService _bpmnNodeService;
    private readonly TaskMgmtService _taskMgmtService;
    private readonly UserEntrustService _userEntrustService;

    public BpmBusinessController(TaskMgmtService taskMgmtService,
        UserEntrustService userEntrustService,
        BpmnNodeService bpmnNodeService)
    {
        _taskMgmtService = taskMgmtService;
        _userEntrustService = userEntrustService;
        _bpmnNodeService = bpmnNodeService;
    }

    /// <summary>
    ///     ��ȡ�Զ�����DIY FormCode List
    /// </summary>
    /// <param name="desc"></param>
    /// <returns></returns>
    [HttpGet("getDIYFormCodeList")]
    public Result<List<DIYProcessInfoDTO>> GetDIYFormCodeList(string desc)
    {
        List<DIYProcessInfoDTO> diyProcessInfoDTOS = _taskMgmtService.ViewProcessInfo(desc);
        return ResultHelper.Success(diyProcessInfoDTOS);
    }

    /// <summary>
    ///     ��ȡί���б�
    /// </summary>
    /// <param name="requestDto"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [HttpPost("entrustlist/{type}")]
    public ResultAndPage<Entrust> EntrustList([FromBody] DetailRequestDto requestDto, [FromRoute] int type)
    {
        PageDto pageDto = requestDto.PageDto;
        Entrust vo = new();
        return _userEntrustService.GetEntrustPageList(pageDto, vo, type);
    }

    /// <summary>
    ///     ��ȡί������
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("entrustDetail/{id}")]
    public Result<UserEntrust> EntrustDetail([FromRoute] int id)
    {
        UserEntrust detail = _userEntrustService.GetEntrustDetail(id);
        return ResultHelper.Success(detail);
    }

    /// <summary>
    ///     �༭ί��
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
    ///     ��ȡ������ѡ�����˽ڵ�
    /// </summary>
    /// <param name="formCode"></param>
    /// <returns></returns>
    /// <exception cref="AFBizException"></exception>
    [HttpGet("getStartUserChooseModules")]
    public Result<List<BpmnNodeVo>> GetStartUserChooseModules([FromQuery] string formCode)
    {
        if (string.IsNullOrWhiteSpace(formCode))
        {
            throw new AFBizException("����formCode����Ϊ��!");
        }

        List<BpmnNode> nodes = _bpmnNodeService.GetNodesByFormCodeAndProperty(
            formCode, (int)NodePropertyEnum.NODE_PROPERTY_CUSTOMIZE
        );
        List<BpmnNodeVo> nodeVos = nodes.Select(a => new BpmnNodeVo { Id = a.Id, NodeName = a.NodeName }).ToList();

        return ResultHelper.Success(nodeVos);
    }
}