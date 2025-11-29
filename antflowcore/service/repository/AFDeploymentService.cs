using System.Text.Json;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.service.interf;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class AFDeploymentService: AFBaseCurdRepositoryService<BpmAfDeployment>,IAFDeploymentService
{
    public AFDeploymentService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void UpdateNodeAssignee(BusinessDataVo businessDataVo)
    {
        string processNumber = businessDataVo.ProcessNumber;
        if (string.IsNullOrEmpty(processNumber))
        {
            throw new AFBizException("请传入流程编号");
        }
        BpmAfDeployment bpmAfDeployment = this.Frsql
            .Select<BpmBusinessProcess,BpmAfTask,BpmAfDeployment>()
            .InnerJoin((a,b,c)=>a.ProcInstId==b.ProcInstId)
            .InnerJoin((a,b,c)=>b.ProcDefId==c.Id)
            .Where((a,b,c)=>a.BusinessNumber==businessDataVo.ProcessNumber)
            .ToList<BpmAfDeployment>((a,b,c)=>c)
            .First();
        if (bpmAfDeployment == null)
        {
            throw new AFBizException($"未能根据流程编号:{processNumber}找到流程定义!");
        }

        string content = bpmAfDeployment.Content;
        if (string.IsNullOrEmpty(content))
        {
            throw new AFBizException($"根据流程编号:{processNumber}查找到流程定义内容为空!");
        }
        List<BpmnConfCommonElementVo> elements = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
        List<BaseIdTranStruVo> userInfos = businessDataVo.UserInfos;
        if (userInfos == null)
        {
            userInfos = new List<BaseIdTranStruVo>();
        }

        if (userInfos.Count == 0)
        {
            userInfos.Add(new BaseIdTranStruVo
            {
                Id = "0",
                Name = "跳过",
            });
        }

        string nodeId = businessDataVo.NodeId;
        if (string.IsNullOrEmpty(nodeId))
        {
            throw new AFBizException("传入节点为空,请检查入参");
        }

        BpmnConfCommonElementVo bpmnConfCommonElementVo = elements.First(a => a.NodeId==nodeId);
        if (bpmnConfCommonElementVo == null)
        {
            throw new AFBizException($"流程编号:{processNumber},节点id:{nodeId},未在流程图中找到对应定义");
        }

        List<string> newCollectionValue = new List<string>();
        IDictionary<string, string> assigneeMap = new SortedDictionary<string, string>();
        
        foreach (BaseIdTranStruVo baseIdTranStruVo in userInfos)
        {
            newCollectionValue.Add(baseIdTranStruVo.Id);
            assigneeMap[baseIdTranStruVo.Id] = baseIdTranStruVo.Name;
        }

        bpmnConfCommonElementVo.CollectionValue = newCollectionValue;
        bpmnConfCommonElementVo.AssigneeMap = assigneeMap;
        bpmAfDeployment.Content = JsonSerializer.Serialize(elements);
        bpmAfDeployment.Rev += 1;
        bpmAfDeployment.UpdateTime=DateTime.Now;
        bpmAfDeployment.UpdateUser = SecurityUtils.GetLogInEmpId();
        this.baseRepo.Update(bpmAfDeployment);
    }

    public List<BpmnConfCommonElementVo> GetDeploymentByProcessNumber(string processNumber)
    {
        if (string.IsNullOrEmpty(processNumber))
        {
            return null;
        }
        BpmAfDeployment bpmAfDeployment = this.Frsql
            .Select<BpmBusinessProcess,BpmAfTask,BpmAfDeployment>()
            .InnerJoin((a,b,c)=>a.ProcInstId==b.ProcInstId)
            .InnerJoin((a,b,c)=>b.ProcDefId==c.Id)
            .Where((a,b,c)=>a.BusinessNumber==processNumber)
            .ToList<BpmAfDeployment>((a,b,c)=>c)
            .First();
        if (bpmAfDeployment == null)
        {
            throw new AFBizException($"未能根据流程编号:{processNumber}找到流程定义!");
        }

        string content = bpmAfDeployment.Content;
        if (string.IsNullOrEmpty(content))
        {
            throw new AFBizException($"根据流程编号:{processNumber}查找到流程定义内容为空!");
        }
        List<BpmnConfCommonElementVo> elements = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
        return elements;
    }
}