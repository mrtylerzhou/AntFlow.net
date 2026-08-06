using System.Text.Json;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmVariableSignUpPersonnelService : IBpmVariableSignUpPersonnelService
{
    private readonly IBpmVariableService _bpmVariableService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;

    public BpmVariableSignUpPersonnelService(
        IBpmVariableService bpmVariableService,
        IBpmBusinessProcessService bpmBusinessProcessService)
    {
        _bpmVariableService = bpmVariableService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
    }

    /// <summary>
    /// 写入加批人员到 variable_config_json 的 signUps[].personnelByElement (read-modify-write)。
    /// </summary>
    public void InsertSignUpPersonnel(string processNumber, string taskTaskDefinitionKey, string assignee, List<BaseIdTranStruVo> signUpUsers)
    {
        if (signUpUsers == null || signUpUsers.Count == 0)
        {
            return;
        }

        BpmVariable bpmVariable = _bpmVariableService._repository.FindByProcessNum(processNumber);
        if (bpmVariable == null || string.IsNullOrEmpty(bpmVariable.VariableConfigJson))
        {
            return;
        }

        VariableConfigJson config = JsonSerializer.Deserialize<VariableConfigJson>(bpmVariable.VariableConfigJson, JsonConfUtil.Options);
        if (config == null || config.SignUps == null || config.SignUps.Count == 0)
        {
            return;
        }

        VariableSignUpItem signUp = config.SignUps.FirstOrDefault(s => taskTaskDefinitionKey == s.ElementId);
        if (signUp == null || string.IsNullOrEmpty(signUp.SubElements))
        {
            return;
        }

        List<BpmnConfCommonElementVo> subElementVos = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(signUp.SubElements, JsonConfUtil.Options);
        if (subElementVos == null || subElementVos.Count == 0)
        {
            return;
        }

        // 正向加批节点 (isBackSignUp == 0)
        BpmnConfCommonElementVo signUpElement = subElementVos.FirstOrDefault(o => o.IsBackSignUp == 0) ?? new BpmnConfCommonElementVo();

        List<VariablePersonnelItem> signUpPersonnel = signUpUsers
            .Select(o => new VariablePersonnelItem
            {
                Assignee = o.Id,
                AssigneeName = o.Name
            })
            .ToList();

        signUp.PersonnelByElement[signUpElement.ElementId] = signUpPersonnel;

        // afterSignUpWay == 1: 加批后回到加批人,需要写入回签节点人员
        if (signUp.AfterSignUpWay != null && signUp.AfterSignUpWay == 1)
        {
            BpmnConfCommonElementVo backSignUpElement = subElementVos.FirstOrDefault(o => o.IsBackSignUp == 1) ?? new BpmnConfCommonElementVo();
            signUp.PersonnelByElement[backSignUpElement.ElementId] = new List<VariablePersonnelItem>
            {
                new VariablePersonnelItem
                {
                    Assignee = assignee,
                    AssigneeName = SecurityUtils.GetLogInEmpName()
                }
            };
        }

        bpmVariable.VariableConfigJson = JsonSerializer.Serialize(config, JsonConfUtil.Options);
        _bpmVariableService._repository.Update(bpmVariable);
    }

    /// <summary>
    /// 从 variable_config_json 读取加批节点人员,供 TaskService.Complete 生成加批任务。
    /// 对应 master 版 FsBpmVariableSignUpPersonnelRepository.GetSignUpNodeAssigneeMap。
    /// </summary>
    public List<KeyValuePair<string, string>> GetSignUpNodeAssigneeMap(string procInstId, string elementId)
    {
        var result = new List<KeyValuePair<string, string>>();

        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcessByProcInstId(procInstId);
        if (bpmBusinessProcess == null)
        {
            return result;
        }

        BpmVariable bpmVariable = _bpmVariableService._repository.FindByProcessNum(bpmBusinessProcess.BusinessNumber);
        if (bpmVariable == null || string.IsNullOrEmpty(bpmVariable.VariableConfigJson))
        {
            return result;
        }

        VariableConfigJson config = JsonSerializer.Deserialize<VariableConfigJson>(bpmVariable.VariableConfigJson, JsonConfUtil.Options);
        if (config == null || config.SignUps == null)
        {
            return result;
        }

        foreach (VariableSignUpItem signUp in config.SignUps)
        {
            if (signUp.PersonnelByElement != null && signUp.PersonnelByElement.TryGetValue(elementId, out List<VariablePersonnelItem> personnelList))
            {
                foreach (VariablePersonnelItem p in personnelList)
                {
                    result.Add(new KeyValuePair<string, string>(p.Assignee, p.AssigneeName));
                }
                break;
            }
        }
        return result;
    }
}
