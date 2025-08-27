using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlow.Core.Controller;

[Route("lowcode")]
public class LowCodeFlowController
{
    private readonly DictService _dictService;
    private readonly BpmnConfLFFormDataBizService _lfformDataBizService;

    public LowCodeFlowController(BpmnConfLFFormDataBizService lfformDataBizService, DictService dictService)
    {
        _lfformDataBizService = lfformDataBizService;
        _dictService = dictService;
    }

    [HttpPost("createLowCodeFormCode")]
    public Result<int> CreateLowCodeFormCode([FromBody] BaseKeyValueStruVo vo)
    {
        return Result<int>.Succ(_dictService.AddFormCode(vo));
    }

    [HttpGet("getLowCodeFlowFormCodes")]
    public Result<List<BaseKeyValueStruVo>> GetLowCodeFormCodes()
    {
        List<BaseKeyValueStruVo> lowCodeFlowFormCodes = _dictService.GetLowCodeFlowFormCodes();
        return ResultHelper.Success(lowCodeFlowFormCodes);
    }

    [HttpPost("getLFFormCodePageList")]
    public ResultAndPage<BaseKeyValueStruVo> GetLFFormCodePageList([FromBody] DetailRequestDto requestDto)
    {
        PageDto pageDto = requestDto.PageDto;
        TaskMgmtVO taskMgmtVO = requestDto.TaskMgmtVO;
        return _dictService.SelectLFFormCodePageList(pageDto, taskMgmtVO);
    }

    [HttpPost("getLFActiveFormCodePageList")]
    public ResultAndPage<BaseKeyValueStruVo> GetLFActiveFormCodePageList([FromBody] DetailRequestDto requestDto)
    {
        PageDto? pageDto = requestDto.PageDto;
        TaskMgmtVO? taskMgmtVO = requestDto.TaskMgmtVO;
        ResultAndPage<BaseKeyValueStruVo> resultAndPage =
            _dictService.SelectLFActiveFormCodePageList(pageDto, taskMgmtVO);
        return resultAndPage;
    }

    [HttpGet("getformDataByFormCode")]
    public Result<string> GetLFFormDataByFormCode(string formCode)
    {
        if (string.IsNullOrEmpty(formCode))
        {
            throw new AFBizException("�봫��formcode");
        }

        BpmnConfLfFormdata lfFormDataByFormCode = _lfformDataBizService.GetLFFormDataByFormCode(formCode);
        return ResultHelper.Success(lfFormDataByFormCode.Formdata);
    }
}