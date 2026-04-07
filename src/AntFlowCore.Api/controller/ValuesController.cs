using AntFlowCore.Abstraction;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Common.util;
using AntFlowCore.Core;
using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Entity;
using AntFlowCore.Extensions;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

[Route("user")]
public class ValuesController
{
    private readonly IFreeSql _free;
    private readonly IEnumerable<IBpmnPersonnelProviderService> _personnelProviderServices;
    private readonly IUserService _userService;

    public ValuesController(IFreeSql free,
        IEnumerable<IBpmnPersonnelProviderService> personnelProviderServices,
        IUserService userService)
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
        ResultAndPage<BaseIdTranStruVo> selectUserPageList = _userService.SelectUserPageList(page, taskMgmtVO);
        return selectUserPageList;
    }
}
