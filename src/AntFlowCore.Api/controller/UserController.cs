using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

[Route("user")]
public class UserController
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public UserController(IUserService userService, IRoleService roleService)
    {
        _userService = userService;
        _roleService = roleService;
    }

    [HttpGet("getUser")]
    public Result<List<BaseIdTranStruVo>> GetUser()
    {
        List<BaseIdTranStruVo> users = _userService.SelectAll();
        return ResultHelper.Success(users);
    }

    [HttpGet("getRoleInfo")]
    public Result<List<BaseIdTranStruVo>> GetRoleInfo()
    { 
        List<BaseIdTranStruVo> roles = _roleService.GetAllRoles(); 
        return ResultHelper.Success(roles);
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