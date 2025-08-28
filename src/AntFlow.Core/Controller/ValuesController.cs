using AntFlow.Core.Adaptor.Personnel.Provider;
using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlow.Core.Controller;

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
        List<Student>? select = _free.Select<Student>().ToList();
        _free.Select<Student>().Where(a => new[] { 3, 4, 5 }.Contains(a.Id));
        return select;
    }

    [HttpGet("caseTest")]
    public List<Student> caseTestValue()
    {
        return new List<Student> { new() { Age = 32 } };
    }

    [HttpGet("getUser")]
    public Result<List<BaseIdTranStruVo>> GetUserList()
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