using System.Collections.Concurrent;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.interf.repository;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class BpmProcessNameService: AFBaseCurdRepositoryService<BpmProcessName>,IBpmProcessNameService
{
    private readonly BpmProcessNameRelevancyService _bpmProcessNameRelevancyService;

    public BpmProcessNameService(
        IFreeSql freeSql,
        BpmProcessNameRelevancyService bpmProcessNameRelevancyService
        ) : base(freeSql)
    {
        _bpmProcessNameRelevancyService = bpmProcessNameRelevancyService;
    }
    private static IDictionary<String, BpmProcessVo> processVoMap = new ConcurrentDictionary<string, BpmProcessVo>();
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
    
        if (processName?.Id!=0)
        {
            if (!flag)
            {
                bpmProcessNameRelevancyService.Add(new BpmProcessNameRelevancy
                {
                    ProcessKey = bpmnConfByCode.FormCode,
                    ProcessNameId = processName.Id,
                    CreateTime = DateTime.Now,
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
                    ProcessNameId = id,
                    CreateTime = DateTime.Now,
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
        return new BpmProcessName();
    }

    public BpmProcessVo Get(string processKey)
    {
        processVoMap.TryGetValue(processKey,out  BpmProcessVo? bpmProcessVo);
        if (bpmProcessVo != null)
        {
            return bpmProcessVo;
        }
        //将流程和名称查询缓存
        LoadProcessName();
        processVoMap.TryGetValue(processKey, out BpmProcessVo? processVo);
        return processVo ?? new BpmProcessVo();
    }

    private void LoadProcessName()
    {
        Dictionary<String, BpmProcessVo> map = new Dictionary<String, BpmProcessVo>();
        List<BpmProcessVo> list = this.AllProcess();
        foreach (BpmProcessVo next in list)
        {
            
            if (!string.IsNullOrEmpty(next.ProcessKey))
            {
              
                if (!processVoMap.ContainsKey(next.ProcessKey))
                {
                    processVoMap.Add(next.ProcessKey,next);
                }
            }
        }
    }

    private List<BpmProcessVo> AllProcess()
    {
        List<BpmProcessVo> bpmProcessVos = this.Frsql
            .Select<BpmProcessName,BpmProcessNameRelevancy>()
            .LeftJoin((a,b)=>b.ProcessNameId==a.Id)
            .ToList<BpmProcessVo>((b,s)=>new BpmProcessVo()
            {
                ProcessName = b.ProcessName,
                ProcessKey = s.ProcessKey
            });
        return bpmProcessVos;
    }
}