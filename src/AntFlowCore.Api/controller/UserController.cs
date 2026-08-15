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

    /// <summary>
    /// 角色列表: name 为空返回全量, 非空模糊查询(搜索下拉用)
    /// </summary>
    [HttpGet("getRoleInfo")]
    public Result<List<BaseIdTranStruVo>> GetRoleInfo([FromQuery] string? name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            return ResultHelper.Success(_roleService.QueryRoleByNameFuzzy(name));
        }
        List<BaseIdTranStruVo> roles = _roleService.GetAllRoles();
        return ResultHelper.Success(roles);
    }

    /// <summary>
    /// 用户名称模糊查询(搜索下拉用)
    /// </summary>
    [HttpGet("queryUserByNameFuzzy")]
    public Result<List<BaseIdTranStruVo>> QueryUserByNameFuzzy([FromQuery] string? userName)
    {
        return ResultHelper.Success(_userService.QueryUserByNameFuzzy(userName ?? ""));
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