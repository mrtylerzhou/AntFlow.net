
using antflowcore.adaptor.personnel.provider;
using antflowcore.entity;
using AntFlowCore.Entity;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Mvc;

namespace antflowcore.controller;

[Route("user")]
public class ValuesController
{
    private readonly IFreeSql _free;
    private readonly IEnumerable<IBpmnPersonnelProviderService> _personnelProviderServices;

    public ValuesController(IFreeSql free,IEnumerable<IBpmnPersonnelProviderService> personnelProviderServices)
    {
        _free = free;
        _personnelProviderServices = personnelProviderServices;
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
    public Result<List<Student>>  getUserList()
    {
        var list = new List<Student>() { 
            new Student {  Id = 1,Name="张三" },
            new Student {  Id = 2,Name="李四"  }, 
            new Student {  Id = 3,Name="王五"  },
            new Student {  Id = 4,Name="蔡六"  }

        };
        return Result<List<Student>>.Succ(list);
    }
}
