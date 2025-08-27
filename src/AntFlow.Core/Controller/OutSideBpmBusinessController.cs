using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlow.Core.Controller;

[Route("outSideBpm")]
public class OutSideBpmBusinessController
{
    private readonly BpmProcessAppApplicationService _bpmProcessAppApplicationService;
    private readonly OutSideBpmApproveTemplateService _outSideBpmApproveTemplateService;
    private readonly OutSideBpmBusinessPartyService _outSideBpmBusinessPartyService;
    private readonly OutSideBpmConditionsTemplateService _outSideBpmConditionsTemplateService;

    public OutSideBpmBusinessController(
        OutSideBpmBusinessPartyService outSideBpmBusinessPartyService,
        BpmProcessAppApplicationService bpmProcessAppApplicationService,
        OutSideBpmConditionsTemplateService outSideBpmConditionsTemplateService,
        OutSideBpmApproveTemplateService outSideBpmApproveTemplateService)
    {
        _outSideBpmBusinessPartyService = outSideBpmBusinessPartyService;
        _bpmProcessAppApplicationService = bpmProcessAppApplicationService;
        _outSideBpmConditionsTemplateService = outSideBpmConditionsTemplateService;
        _outSideBpmApproveTemplateService = outSideBpmApproveTemplateService;
    }

    /// <summary>
    ///     ???????????????result,??????????????????,????????
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("businessParty/listPage")]
    public Result<ResultAndPage<OutSideBpmBusinessPartyVo>> ListPage([FromBody] PageRequestDto<BpmnConfVo> dto)
    {
        PageDto? page = dto.PageDto;
        BpmnConfVo? vo = dto.Entity;
        OutSideBpmBusinessPartyVo? searchVo = new();

        if (!string.IsNullOrEmpty(vo.Remark))
        {
            searchVo.Name = vo.Remark;
            searchVo.Remark = vo.Remark;
        }

        ResultAndPage<OutSideBpmBusinessPartyVo> resultAndPage =
            _outSideBpmBusinessPartyService.ListPage(page, searchVo);
        return ResultHelper.Success(resultAndPage);
    }

    [HttpGet("businessParty/detail/{id}")]
    public Result<OutSideBpmBusinessPartyVo> Detail(int id)
    {
        return ResultHelper.Success(_outSideBpmBusinessPartyService.Detail(id));
    }

    [HttpPost("businessParty/edit")]
    public Result<string> Edit([FromBody] OutSideBpmBusinessPartyVo vo)
    {
        _outSideBpmBusinessPartyService.Edit(vo);
        return ResultHelper.Success("ok");
    }

    [HttpPost("businessParty/applicationsPageList")]
    public Result<ResultAndPage<BpmProcessAppApplicationVo>> ApplicationsPageList(
        [FromBody] PageRequestDto<BpmnConfVo> dto)
    {
        PageDto? page = dto.PageDto;
        BpmnConfVo? vo = dto.Entity;
        BpmProcessAppApplicationVo searchVo = new();

        if (!string.IsNullOrEmpty(vo.Remark))
        {
            searchVo.BusinessName = vo.Remark;
            searchVo.ProcessKey = vo.Remark;
        }

        if (string.IsNullOrEmpty(page.OrderColumn))
        {
            page.OrderColumn = "id";
        }

        ResultAndPage<BpmProcessAppApplicationVo> applicationsPageList =
            _bpmProcessAppApplicationService.ApplicationsPageList(page, searchVo);
        return ResultHelper.Success(applicationsPageList);
    }

    /// <summary>
    ///     ?????????????????
    /// </summary>
    /// <param name="vo"></param>
    /// <returns></returns>
    [HttpPost("businessParty/addBpmProcessAppApplication")]
    public Result<bool> AddBpmProcessAppApplication([FromBody] BpmProcessAppApplicationVo vo)
    {
        bool ret = _bpmProcessAppApplicationService.AddBpmProcessAppApplication(vo);
        return ResultHelper.Success(ret);
    }

    /// <summary>
    ///     ????????????????
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("businessParty/applicationDetail/{id}")]
    public Result<BpmProcessAppApplication> ApplicationDetail(int id)
    {
        BpmProcessAppApplication bpmProcessAppApplication = _bpmProcessAppApplicationService
            .baseRepo
            .Where(a => a.Id == id)
            .ToOne();
        return ResultHelper.Success(bpmProcessAppApplication);
    }

    /// <summary>
    ///     ?????????????????????
    /// </summary>
    /// <param name="page"></param>
    /// <param name="vo"></param>
    /// <returns></returns>
    [HttpGet("conditionTemplate/listPage")]
    public Result<ResultAndPage<OutSideBpmConditionsTemplateVo>> ConditionTemplateListPage([FromQuery] PageDto page,
        [FromQuery] OutSideBpmConditionsTemplateVo vo)
    {
        return ResultHelper.Success(_outSideBpmConditionsTemplateService.ListPage(page, vo));
    }

    /// <summary>
    ///     ??????¨°?????????????
    /// </summary>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    [HttpGet("conditionTemplate/selectListByTemp/{applicationId}")]
    public Result<List<OutSideBpmConditionsTemplateVo>> SelectConditionListByAppId(int applicationId)
    {
        return ResultHelper.Success(_outSideBpmConditionsTemplateService.SelectConditionListByAppId(applicationId));
    }

    /// <summary>
    ///     ?????????????????
    /// </summary>
    /// <param name="vo"></param>
    /// <returns></returns>
    [HttpPost("conditionTemplate/edit")]
    public Result<string> EditConditionTemplate([FromBody] OutSideBpmConditionsTemplateVo vo)
    {
        _outSideBpmConditionsTemplateService.Edit(vo);
        return ResultHelper.Success("ok");
    }

    /// <summary>
    ///     ??????????????????
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("conditionTemplate/delete/{id}")]
    public Result<string> DeleteConditionTemplate(int id)
    {
        _outSideBpmConditionsTemplateService.Delete(id);
        return ResultHelper.Success("ok");
    }

    [HttpGet("approveTemplate/listPage")]
    public Result<ResultAndPage<OutSideBpmApproveTemplateVo>> ApproveTemplateListPage([FromQuery] PageDto page,
        [FromQuery] OutSideBpmApproveTemplateVo vo)
    {
        return ResultHelper.Success(_outSideBpmApproveTemplateService.ListPage(page, vo));
    }

    [HttpGet("approveTemplate/selectListByTemp/{applicationId}")]
    public Result<List<OutSideBpmApproveTemplateVo>> SelectApproveListByPartMarkIdAndAppId(int applicationId)
    {
        return ResultHelper.Success(_outSideBpmApproveTemplateService.SelectListByTemp(applicationId));
    }

    [HttpPost("approveTemplate/edit")]
    public Result<string> ApproveTemplateEdit([FromBody] OutSideBpmApproveTemplateVo vo)
    {
        _outSideBpmApproveTemplateService.Edit(vo);
        return ResultHelper.Success("ok");
    }

    [HttpGet("approveTemplate/detail/{id}")]
    public Result<OutSideBpmApproveTemplateVo> ApproveTemplateDetail(int id)
    {
        return ResultHelper.Success(_outSideBpmApproveTemplateService.Detail(id));
    }
}