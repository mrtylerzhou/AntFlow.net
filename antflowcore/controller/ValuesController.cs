
using antflowcore.adaptor.personnel.provider;
using antflowcore.entity;
using Microsoft.AspNetCore.Mvc;

namespace antflowcore.controller;

[Route("value")]
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
}
