using AntFlow.Core.Entity;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Collections.Concurrent;

namespace AntFlow.Core.Service.Repository;

public class BpmProcessNameService : AFBaseCurdRepositoryService<BpmProcessName>, IBpmProcessNameService
{
    private static readonly IDictionary<string, BpmProcessVo> processVoMap =
        new ConcurrentDictionary<string, BpmProcessVo>();

    private readonly BpmProcessNameRelevancyService _bpmProcessNameRelevancyService;

    public BpmProcessNameService(
        IFreeSql freeSql,
        BpmProcessNameRelevancyService bpmProcessNameRelevancyService
    ) : base(freeSql)
    {
        _bpmProcessNameRelevancyService = bpmProcessNameRelevancyService;
    }

    public BpmProcessName GetBpmProcessName(string processKey)
    {
        BpmProcessNameRelevancy processNameRelevancy =
            _bpmProcessNameRelevancyService.FindProcessNameRelevancy(processKey);
        if (processNameRelevancy == null)
        {
            return new BpmProcessName();
        }

        BpmProcessName bpmProcessName = baseRepo.Where(a => a.Id.Equals(processNameRelevancy.ProcessNameId)).ToOne();
        return bpmProcessName;
    }

    public void EditProcessName(BpmnConf bpmnConfByCode)
    {
        BpmProcessName processName = FindProcessName(bpmnConfByCode.BpmnName);
        BpmProcessNameRelevancyService bpmProcessNameRelevancyService =
            ServiceProviderUtils.GetService<BpmProcessNameRelevancyService>();
        bool flag = bpmProcessNameRelevancyService.SelectCount(bpmnConfByCode.FormCode);

        if (processName?.Id != 0)
        {
            if (!flag)
            {
                bpmProcessNameRelevancyService.Add(new BpmProcessNameRelevancy
                {
                    ProcessKey = bpmnConfByCode.FormCode, ProcessNameId = processName.Id
                });
            }
        }
        else
        {
            if (flag)
            {
                BpmProcessNameRelevancy processNameRelevancy =
                    bpmProcessNameRelevancyService.FindProcessNameRelevancy(bpmnConfByCode.FormCode);
                BpmProcessName bpmProcessName = baseRepo.Where(p => p.Id == processNameRelevancy.ProcessNameId).First();
                bpmProcessName.ProcessName = bpmnConfByCode.BpmnName;
                baseRepo.Update(bpmProcessName);
            }
            else
            {
                BpmProcessName process = new() { ProcessName = bpmnConfByCode.BpmnName };
                baseRepo.Insert(process);

                long id = process.Id;
                bpmProcessNameRelevancyService.Add(new BpmProcessNameRelevancy
                {
                    ProcessKey = bpmnConfByCode.FormCode, ProcessNameId = id
                });
            }
        }
    }

    private BpmProcessName FindProcessName(string processName)
    {
        List<BpmProcessName> bpmProcessNames = baseRepo
            .Where(a => a.ProcessName == processName && a.IsDel == 0)
            .ToList();
        if (bpmProcessNames.Count > 0)
        {
            return bpmProcessNames[0];
        }

        return new BpmProcessName();
    }

    public BpmProcessVo Get(string processKey)
    {
        processVoMap.TryGetValue(processKey, out BpmProcessVo? bpmProcessVo);
        if (bpmProcessVo != null)
        {
            return bpmProcessVo;
        }

        //加载流程名称缓存
        LoadProcessName();
        processVoMap.TryGetValue(processKey, out BpmProcessVo? processVo);
        return processVo ?? new BpmProcessVo();
    }

    private void LoadProcessName()
    {
        Dictionary<string, BpmProcessVo> map = new();
        List<BpmProcessVo> list = AllProcess();
        foreach (BpmProcessVo next in list)
        {
            if (!string.IsNullOrEmpty(next.ProcessKey))
            {
                if (!processVoMap.ContainsKey(next.ProcessKey))
                {
                    processVoMap.Add(next.ProcessKey, next);
                }
            }
        }
    }

    private List<BpmProcessVo> AllProcess()
    {
        List<BpmProcessVo> bpmProcessVos = Frsql
            .Select<BpmProcessName, BpmProcessNameRelevancy>()
            .LeftJoin((a, b) => b.ProcessNameId == a.Id)
            .ToList<BpmProcessVo>((b, s) => new BpmProcessVo
            {
                ProcessName = b.ProcessName, ProcessKey = s.ProcessKey
            });
        return bpmProcessVos;
    }
}