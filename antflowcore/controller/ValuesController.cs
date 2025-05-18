
using antflowcore.adaptor.personnel.provider;
using antflowcore.dto;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Mvc;

namespace antflowcore.controller;

[Route("user")]
public class ValuesController
{
    private readonly IFreeSql _free;
    private readonly IEnumerable<IBpmnPersonnelProviderService> _personnelProviderServices;
    private readonly UserService _userService;

    public ValuesController(IFreeSql free,
        IEnumerable<IBpmnPersonnelProviderService> personnelProviderServices,
        UserService userService)
    {
        _free = free;
        _personnelProviderServices = personnelProviderServices;
        _userService = userService;
    }
    [HttpGet("test")]
    public List<Student> testValue()
    {
        var select = _free.Select<Student>().ToList();
        _free.Select<Student>().Where(a => new[] { 3, 4, 5 }.Contains(a.Id));
        return select;
    }

    [HttpGet("caseTest")]
    public List<Student> caseTestValue()
    {
        return new List<Student>() { new Student { Age = 32 } };
    }
    [HttpGet("getUser")] 
    public Result<List<BaseIdTranStruVo>>  GetUserList()
    {
       return ResultHelper.Success(_userService.SelectAll());
    }

    [HttpPost("getUserPageList")]
    public ResultAndPage<BaseIdTranStruVo> GetUserPageList([FromBody] DetailRequestDto requestDto)
    {
        PageDto pageDto = requestDto.PageDto;
        Page<BaseIdTranStruVo> page = PageUtils.GetPageByPageDto<BaseIdTranStruVo>(pageDto);
        TaskMgmtVO taskMgmtVO = requestDto.TaskMgmtVO;
        List<BaseIdTranStruVo> baseIdTranStruVos = _userService.SelectUserPageList(page, taskMgmtVO);
        page.Records=baseIdTranStruVos;
        return PageUtils.GetResultAndPage(page);
    }
}
