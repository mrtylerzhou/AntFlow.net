using antflowcore.entity;
using AntOffice.Base.Util;
using FreeSql;

namespace antflowcore.service.repository;

public class BpmnConfService
{
    private readonly IFreeSql _freeSql;
    public IBaseRepository<BpmnConf> baseRepo{get;}

    public BpmnConfService(IFreeSql freeSql)
    {
        _freeSql = freeSql;
       baseRepo = freeSql.GetRepository<BpmnConf>();
    }
    public string GetMaxBpmnCode(String bpmnCodeParts)
    {
        //todo 注意验证生成的语句是什么样子的,是否有性能问题
        string maxBpmnCode = baseRepo
            .Select
            .Where(a=>a.BpmnName.EndsWith(bpmnCodeParts))
            .Max(a=>a.BpmnCode);
        return maxBpmnCode;
    }

    public String ReCheckBpmnCode(String bpmnCodeParts, String bpmnCode)
    {

        long count = baseRepo.Select.Where(a => a.BpmnCode.Equals(bpmnCode)).Count();

        if (count == 0)
        {
            return bpmnCode;
        }

        String reJoinedBpmnCode = StrUtils.JoinBpmnCode(bpmnCodeParts, bpmnCode);

        return ReCheckBpmnCode(bpmnCodeParts, reJoinedBpmnCode);
    }
    
}