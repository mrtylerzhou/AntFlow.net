using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.util;

public static class MapperUtil
{
    
    public static BpmnConfVo MapToVo(this BpmnConf entity)
    {
        if (entity == null) return null;

        return new BpmnConfVo
        {
            Id = entity.Id,
            BpmnCode = entity.BpmnCode,
            BpmnName = entity.BpmnName,
            BpmnType = entity.BpmnType ?? 0,
            FormCode = entity.FormCode,
            AppId = entity.AppId,
            DeduplicationType = entity.DeduplicationType,
            EffectiveStatus = entity.EffectiveStatus,
            IsAll = entity.IsAll,
            IsOutSideProcess = entity.IsOutSideProcess,
            IsLowCodeFlow = entity.IsLowCodeFlow,
            BusinessPartyId = entity.BusinessPartyId,
            Remark = entity.Remark,
            IsDel = entity.IsDel,
            CreateUser = entity.CreateUser,
            CreateTime = entity.CreateTime,
            UpdateUser = entity.UpdateUser,
            UpdateTime = entity.UpdateTime
        };
    }

    public static BpmnConf MapToEntity(this BpmnConfVo vo)
    {
        if (vo == null) return null;

        return new BpmnConf
        {
            Id = vo.Id,
            BpmnCode = vo.BpmnCode,
            BpmnName = vo.BpmnName,
            BpmnType = vo.BpmnType,
            FormCode = vo.FormCode,
            AppId = vo.AppId,
            DeduplicationType = vo.DeduplicationType,
            EffectiveStatus = vo.EffectiveStatus,
            IsAll = vo.IsAll,
            IsOutSideProcess = vo.IsOutSideProcess,
            IsLowCodeFlow = vo.IsLowCodeFlow,
            BusinessPartyId = vo.BusinessPartyId,
            Remark = vo.Remark,
            IsDel = vo.IsDel,
            CreateUser = vo.CreateUser,
            CreateTime = vo.CreateTime,
            UpdateUser = vo.UpdateUser,
            UpdateTime = vo.UpdateTime
        };
    }
    
   
    public static BpmnNodeVo MapToVo(this BpmnNode node)
    {
        if (node == null) return null;
 
        return new BpmnNodeVo
        {
            Id = node.Id,
            ConfId = node.ConfId,
            NodeId = node.NodeId,
            NodeType = node.NodeType,
            NodeProperty = node.NodeProperty,
            NodeFrom = node.NodeFrom,
            BatchStatus = node.BatchStatus,
            ApprovalStandard = node.ApprovalStandard,
            NodeName = node.NodeName,
            NodeDisplayName = node.NodeDisplayName,
            Annotation = node.Annotation,
            IsDeduplication = node.IsDeduplication,
            IsSignUp = node.IsSignUp,
            Remark = node.Remark,
            IsDel = node.IsDel,
            CreateUser = node.CreateUser,
            CreateTime = node.CreateTime,
            UpdateUser = node.UpdateUser,
            UpdateTime = node.UpdateTime,
            NodeFroms = node.NodeFroms,
            IsOutSideProcess = node.IsOutSideProcess,
            IsLowCodeFlow = node.IsLowCodeFlow
        };
    }
 
   
    public static BpmnNode MapToEntity(this BpmnNodeVo vo)
    {
        if (vo == null) return null;
 
        return new BpmnNode
        {
            Id = vo.Id,
            ConfId = vo.ConfId,
            NodeId = vo.NodeId,
            NodeType = vo.NodeType,
            NodeProperty = vo.NodeProperty ?? 0,
            NodeFrom = vo.NodeFrom,
            BatchStatus = vo.BatchStatus,
            ApprovalStandard = vo.ApprovalStandard,
            NodeName = vo.NodeName,
            NodeDisplayName = vo.NodeDisplayName,
            Annotation = vo.Annotation,
            IsDeduplication = vo.IsDeduplication,
            IsSignUp = vo.IsSignUp,
            Remark = vo.Remark,
            IsDel = vo.IsDel,
            CreateUser = vo.CreateUser,
            CreateTime = vo.CreateTime,
            UpdateUser = vo.UpdateUser,
            UpdateTime = vo.UpdateTime,
            NodeFroms = vo.NodeFroms,
            IsOutSideProcess = vo.IsOutSideProcess,
            IsLowCodeFlow = vo.IsLowCodeFlow
        };
    }
    
   
    public static BpmnTemplateVo MapToVo(this BpmnTemplate template)
    {
        if (template == null) return null;
 
        return new BpmnTemplateVo
        {
            Id = template.Id,
            ConfId = template.ConfId,
            NodeId = template.NodeId ?? 0,
            Event = template.Event,
            Informs = template.Informs,
            Emps = template.Emps,
            Roles = template.Roles,
            Funcs = template.Funcs,
            TemplateId = template.TemplateId,
            IsDel = template.IsDel,
            CreateTime = template.CreateTime,
            CreateUser = template.CreateUser,
            UpdateTime = template.UpdateTime,
            UpdateUser = template.UpdateUser
        };
    }
 
  
    public static BpmnTemplate MapToEntity(this BpmnTemplateVo vo)
    {
        if (vo == null) return null;
 
        return new BpmnTemplate
        {
            Id = vo.Id,
            ConfId = vo.ConfId,
            NodeId = vo.NodeId,
            Event = vo.Event,
            Informs = vo.Informs,
            Emps = vo.Emps,
            Roles = vo.Roles,
            Funcs = vo.Funcs,
            TemplateId = vo.TemplateId,
            IsDel = vo.IsDel,
            CreateTime = vo.CreateTime,
            CreateUser = vo.CreateUser,
            UpdateTime = vo.UpdateTime,
            UpdateUser = vo.UpdateUser
        };
    }
   
    public static BpmProcessAppApplicationVo MapToVo(this BpmProcessAppApplication app)
    {
        if (app == null) return null;
 
        return new BpmProcessAppApplicationVo
        {
            Id = app.Id,
            BusinessCode = app.BusinessCode,
            Title = app.Title,
            ApplyType = app.ApplyType,
            PcIcon = app.PcIcon,
            EffectiveSource = app.EffectiveSource,
            IsSon = app.IsSon,
            LookUrl = app.LookUrl,
            SubmitUrl = app.SubmitUrl,
            ConditionUrl = app.ConditionUrl,
            ParentId = app.ParentId,
            ApplicationUrl = app.ApplicationUrl,
            Route = app.Route,
            ProcessKey = app.ProcessKey,
            PermissionsCode = app.PermissionsCode,
            IsDel = app.IsDel,
            CreateUserId = app.CreateUserId,
            CreateTime = app.CreateTime,
            UpdateUser = app.UpdateUser,
            UpdateTime = app.UpdateTime,
            IsAll = app.IsAll,
            State = app.State,
            Sort = app.Sort,
            Source = app.Source
        };
    }
 
  
    public static BpmProcessAppApplication MapToEntity(this BpmProcessAppApplicationVo vo)
    {
        if (vo == null) return null;
 
        return new BpmProcessAppApplication
        {
            Id = vo.Id ?? 0, 
            BusinessCode = vo.BusinessCode,
            Title = vo.Title,
            ApplyType = vo.ApplyType ?? 0,
            PcIcon = vo.PcIcon,
            EffectiveSource = vo.EffectiveSource,
            IsSon = vo.IsSon ?? 0,
            LookUrl = vo.LookUrl,
            SubmitUrl = vo.SubmitUrl,
            ConditionUrl = vo.ConditionUrl,
            ParentId = vo.ParentId ?? 0,
            ApplicationUrl = vo.ApplicationUrl,
            Route = vo.Route,
            ProcessKey = vo.ProcessKey,
            PermissionsCode = vo.PermissionsCode,
            IsDel = vo.IsDel ?? 0,
            CreateUserId = vo.CreateUserId,
            CreateTime = vo.CreateTime,
            UpdateUser = vo.UpdateUser,
            UpdateTime = vo.UpdateTime,
            IsAll = vo.IsAll ?? 0,
            State = vo.State ?? 0,
            Sort = vo.Sort ?? 0,
            Source = vo.Source
        };
    }
   
    public static BpmnApproveRemindVo MapToVo(this BpmnApproveRemind remind)
    {
        if (remind == null) return null;
 
        return new BpmnApproveRemindVo
        {
            Id = remind.Id,
            ConfId = remind.ConfId,
            NodeId = remind.NodeId,
            TemplateId = remind.TemplateId,
            Days = remind.Days,
            IsDel = remind.IsDel,
            CreateTime = remind.CreateTime,
            CreateUser = remind.CreateUser,
            UpdateTime = remind.UpdateTime,
            UpdateUser = remind.UpdateUser
        };
    }
 
   
    public static BpmnApproveRemind MapToEntity(this BpmnApproveRemindVo vo)
    {
        if (vo == null) return null;
 
        return new BpmnApproveRemind
        {
            Id = vo.Id,
            ConfId = vo.ConfId,
            NodeId = vo.NodeId,
            TemplateId = vo.TemplateId,
            Days = vo.Days,
            IsDel = vo.IsDel,
            CreateTime = vo.CreateTime,
            CreateUser = vo.CreateUser,
            UpdateTime = vo.UpdateTime,
            UpdateUser = vo.UpdateUser
        };
    }
    
  
    public static ThirdPartyAccountApplyVo MapToVo(this ThirdPartyAccountApply apply)
    {
        if (apply == null) return null;
 
        return new ThirdPartyAccountApplyVo
        {
            AccountType = apply.AccountType,
            AccountOwnerName = apply.AccountOwnerName,
            Remark = apply.Remark
        };
    }
 
   
    public static ThirdPartyAccountApply MapToEntity(this ThirdPartyAccountApplyVo vo)
    {
        if (vo == null) return null;
 
        return new ThirdPartyAccountApply
        {
            AccountType = vo.AccountType,
            AccountOwnerName = vo.AccountOwnerName,
            Remark = vo.Remark
        };
    }
}