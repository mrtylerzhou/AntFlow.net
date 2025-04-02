using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.util;

namespace antflowcore.service.repository;

public class BpmProcessNameService: AFBaseCurdRepositoryService<BpmProcessName>
{
    private readonly BpmProcessNameRelevancyService _bpmProcessNameRelevancyService;

    public BpmProcessNameService(
        IFreeSql freeSql,
        BpmProcessNameRelevancyService bpmProcessNameRelevancyService
        ) : base(freeSql)
    {
        _bpmProcessNameRelevancyService = bpmProcessNameRelevancyService;
    }
    
    public BpmProcessName GetBpmProcessName(String processKey) {
        BpmProcessNameRelevancy processNameRelevancy = _bpmProcessNameRelevancyService.FindProcessNameRelevancy(processKey);
        if (processNameRelevancy==null) {
            return new BpmProcessName();
        }

        BpmProcessName bpmProcessName = baseRepo.Where(a=>a.Id.Equals(processNameRelevancy.ProcessNameId)).ToOne();
        return bpmProcessName;
    }

    public void EditProcessName(BpmnConf bpmnConfByCode)
    {
        BpmProcessName processName = FindProcessName(bpmnConfByCode.BpmnName);
        BpmProcessNameRelevancyService bpmProcessNameRelevancyService = ServiceProviderUtils.GetService<BpmProcessNameRelevancyService>();
        bool flag = bpmProcessNameRelevancyService.SelectCount(bpmnConfByCode.FormCode);
    
        if (processName?.Id != null)
        {
            if (!flag)
            {
                bpmProcessNameRelevancyService.Add(new BpmProcessNameRelevancy
                {
                    ProcessKey = bpmnConfByCode.FormCode,
                    ProcessNameId = processName.Id
                });
            }
        }
        else
        {
            if (flag)
            {
                BpmProcessNameRelevancy processNameRelevancy = bpmProcessNameRelevancyService.FindProcessNameRelevancy(bpmnConfByCode.FormCode);
                BpmProcessName bpmProcessName = this.baseRepo.Where(p => p.Id == processNameRelevancy.ProcessNameId).First();
                bpmProcessName.ProcessName = bpmnConfByCode.BpmnName;
                this.baseRepo.Update(bpmProcessName);
            }
            else
            {
                BpmProcessName process = new BpmProcessName { ProcessName = bpmnConfByCode.BpmnName };
                this.baseRepo.Insert(process);

                long id = process.Id;
                bpmProcessNameRelevancyService.Add(new BpmProcessNameRelevancy
                {
                    ProcessKey = bpmnConfByCode.FormCode,
                    ProcessNameId = id
                });
            }
        }
    }

    private BpmProcessName FindProcessName(string processName)
    {
        List<BpmProcessName> bpmProcessNames = this.baseRepo
            .Where(a=>a.ProcessName==processName&&a.IsDel==0)
            .ToList();
        if (bpmProcessNames.Count > 0)
        {
            return bpmProcessNames[0];
        }
        else
        {
            return new BpmProcessName();
        }
    }
}