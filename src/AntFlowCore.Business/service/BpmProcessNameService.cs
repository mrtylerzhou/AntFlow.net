using System.Collections.Concurrent;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class BpmProcessNameService : IBpmProcessNameService
{
    private readonly IBpmProcessNameRelevancyService _bpmProcessNameRelevancyService;

    public BpmProcessNameService(
        IBpmProcessNameRepository repository,
        IBpmProcessNameRelevancyService bpmProcessNameRelevancyService
        )
    {
        _repository = repository;
        _bpmProcessNameRelevancyService = bpmProcessNameRelevancyService;
    }

    public IBpmProcessNameRepository _repository { get; }

    private static IDictionary<String, BpmProcessVo> processVoMap = new ConcurrentDictionary<string, BpmProcessVo>();

    public BpmProcessName GetBpmProcessName(String processKey)
    {
        BpmProcessNameRelevancy processNameRelevancy = _bpmProcessNameRelevancyService.FindProcessNameRelevancy(processKey);
        if (processNameRelevancy == null)
        {
            return new BpmProcessName();
        }

        BpmProcessName bpmProcessName = _repository.FirstOrDefault(a => a.Id.Equals(processNameRelevancy.ProcessNameId));
        return bpmProcessName;
    }

    public void EditProcessName(BpmnConf bpmnConfByCode)
    {
        BpmProcessName processName = FindProcessName(bpmnConfByCode.BpmnName);
        bool flag = _bpmProcessNameRelevancyService.SelectCount(bpmnConfByCode.FormCode);

        if (processName?.Id != 0)
        {
            if (!flag)
            {
                _bpmProcessNameRelevancyService.Add(new BpmProcessNameRelevancy
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
                BpmProcessNameRelevancy processNameRelevancy = _bpmProcessNameRelevancyService.FindProcessNameRelevancy(bpmnConfByCode.FormCode);
                BpmProcessName bpmProcessName = _repository.FirstOrDefault(p => p.Id == processNameRelevancy.ProcessNameId);
                bpmProcessName.ProcessName = bpmnConfByCode.BpmnName;
                _repository.Update(bpmProcessName);
            }
            else
            {
                BpmProcessName process = new BpmProcessName { ProcessName = bpmnConfByCode.BpmnName, CreateTime = DateTime.Now };
                _repository.Add(process);

                long id = process.Id;
                _bpmProcessNameRelevancyService.Add(new BpmProcessNameRelevancy
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
        List<BpmProcessName> bpmProcessNames = _repository
            .Find(a => a.ProcessName == processName && a.IsDel == 0);
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
        //将流程和名称查询缓存
        LoadProcessName();
        processVoMap.TryGetValue(processKey, out BpmProcessVo? processVo);
        return processVo ?? new BpmProcessVo();
    }

    private void LoadProcessName()
    {
        List<BpmProcessVo> list = _repository.GetAllProcessVo();
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
}
