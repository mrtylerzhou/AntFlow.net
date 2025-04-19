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

    public static InformationTemplateVo MapToVo(this InformationTemplate template)
    {
        return new InformationTemplateVo
        {
           
            Name = template.Name,
            Num = template.Num,
            SystemTitle = template.SystemTitle,
            SystemContent = template.SystemContent,
            MailTitle = template.MailTitle,
            MailContent = template.MailContent,
            NoteContent = template.NoteContent,
            JumpUrl = template.JumpUrl,
            Remark = template.Remark,
            Status = template.Status,
            IsDel = template.IsDel,
            CreateTime = template.CreateTime,
            CreateUser = template.CreateUser,
            UpdateTime = template.UpdateTime,
            UpdateUser = template.UpdateUser,
            WildcardCharacterMap = new Dictionary<int, string>()
        };
    }


    public static InformationTemplate MapToEntity(this InformationTemplateVo vo)
    {
        return new InformationTemplate
        {
           
            Name = vo.Name,
            Num = vo.Num,
            SystemTitle = vo.SystemTitle,
            SystemContent = vo.SystemContent,
            MailTitle = vo.MailTitle,
            MailContent = vo.MailContent,
            NoteContent = vo.NoteContent,
            JumpUrl = vo.JumpUrl ?? 0,
            Remark = vo.Remark,
            Status = vo.Status ?? 0,
            IsDel = vo.IsDel ?? 0,
            CreateTime = vo.CreateTime,
            CreateUser = vo.CreateUser,
            UpdateTime = vo.UpdateTime,
            UpdateUser = vo.UpdateUser
        };
    }

    public static void CopyPropertiesTo(this InformationTemplateVo vo, InformationTemplate informationTemplate)
    {
        if (vo == null || informationTemplate == null) return;

        if (informationTemplate.Id == default && vo.Id.HasValue)
        {
            informationTemplate.Id = vo.Id.Value;
        }

        if (string.IsNullOrEmpty(informationTemplate.Name) && !string.IsNullOrEmpty(vo.Name))
        {
            informationTemplate.Name = vo.Name;
        }

        if (string.IsNullOrEmpty(informationTemplate.Num) && !string.IsNullOrEmpty(vo.Num))
        {
            informationTemplate.Num = vo.Num;
        }

        if (string.IsNullOrEmpty(informationTemplate.SystemTitle) && !string.IsNullOrEmpty(vo.SystemTitle))
        {
            informationTemplate.SystemTitle = vo.SystemTitle;
        }

        if (string.IsNullOrEmpty(informationTemplate.SystemContent) && !string.IsNullOrEmpty(vo.SystemContent))
        {
            informationTemplate.SystemContent = vo.SystemContent;
        }

        if (string.IsNullOrEmpty(informationTemplate.MailTitle) && !string.IsNullOrEmpty(vo.MailTitle))
        {
            informationTemplate.MailTitle = vo.MailTitle;
        }

        if (string.IsNullOrEmpty(informationTemplate.MailContent) && !string.IsNullOrEmpty(vo.MailContent))
        {
            informationTemplate.MailContent = vo.MailContent;
        }

        if (string.IsNullOrEmpty(informationTemplate.NoteContent) && !string.IsNullOrEmpty(vo.NoteContent))
        {
            informationTemplate.NoteContent = vo.NoteContent;
        }

        if (informationTemplate.JumpUrl == default && vo.JumpUrl.HasValue)
        {
            informationTemplate.JumpUrl = vo.JumpUrl.Value;
        }

        if (string.IsNullOrEmpty(informationTemplate.Remark) && !string.IsNullOrEmpty(vo.Remark))
        {
            informationTemplate.Remark = vo.Remark;
        }

        if (informationTemplate.Status == default && vo.Status.HasValue)
        {
            informationTemplate.Status = vo.Status.Value;
        }

        if (informationTemplate.IsDel == default && vo.IsDel.HasValue)
        {
            informationTemplate.IsDel = vo.IsDel.Value;
        }

        if (informationTemplate.CreateTime == default && vo.CreateTime.HasValue)
        {
            informationTemplate.CreateTime = vo.CreateTime.Value;
        }

        if (string.IsNullOrEmpty(informationTemplate.CreateUser) && !string.IsNullOrEmpty(vo.CreateUser))
        {
            informationTemplate.CreateUser = vo.CreateUser;
        }

        if (informationTemplate.UpdateTime == default && vo.UpdateTime.HasValue)
        {
            informationTemplate.UpdateTime = vo.UpdateTime.Value;
        }

        if (string.IsNullOrEmpty(informationTemplate.UpdateUser) && !string.IsNullOrEmpty(vo.UpdateUser))
        {
            informationTemplate.UpdateUser = vo.UpdateUser;
        }
    }

    // 将 OutSideBpmAccessBusiness 转换为 OutSideBpmAccessBusinessVo
    public static OutSideBpmAccessBusinessVo MapToVo(this OutSideBpmAccessBusiness business)
    {
        return new OutSideBpmAccessBusinessVo
        {
           
            BusinessPartyId = business.BusinessPartyId,
            BpmnConfId = business.BpmnConfId, // 使用修正后的字段名称
            FormCode = business.FormCode,
            ProcessNumber = business.ProcessNumber,
            FormDataPc = business.FormDataPc,
            FormDataApp = business.FormDataApp,
            TemplateMark = business.TemplateMark,
            UserName = business.StartUsername,
            Remark = business.Remark,
            IsDel = business.IsDel,
            CreateUser = business.CreateUser,
            CreateTime = business.CreateTime,
            UpdateUser = business.UpdateUser,
            UpdateTime = business.UpdateTime,
        };
    }

    // 将 OutSideBpmAccessBusinessVo 转换为 OutSideBpmAccessBusiness
    public static OutSideBpmAccessBusiness MapToEntity(this OutSideBpmAccessBusinessVo vo)
    {
        return new OutSideBpmAccessBusiness
        {
            BusinessPartyId = vo.BusinessPartyId??0,
            BpmnConfId = vo.BpmnConfId??0,
            FormCode = vo.FormCode,
            ProcessNumber = vo.ProcessNumber,
            FormDataPc = vo.FormDataPc,
            FormDataApp = vo.FormDataApp,
            TemplateMark = vo.TemplateMark,
            StartUsername = vo.UserName,
            Remark = vo.Remark,
            IsDel = vo.IsDel,
            CreateUser = vo.CreateUser,
            CreateTime = vo.CreateTime,
            UpdateUser = vo.UpdateUser,
            UpdateTime = vo.UpdateTime
        };
    }
     public static void CopyPropertiesTo(this OutSideBpmAccessBusinessVo vo, OutSideBpmAccessBusiness business)
    {
        if (vo == null || business == null) return;

        if (business.Id == default && vo.Id != default)
        {
            business.Id = vo.Id;
        }

        if (business.BusinessPartyId==default && vo.BusinessPartyId!=null&&vo.BusinessPartyId>0)
        {
            business.BusinessPartyId = vo.BusinessPartyId.Value;
        }

        if (business.BpmnConfId==default && vo.BpmnConfId!=null&&vo.BpmnConfId>0)
        {
            business.BpmnConfId = vo.BpmnConfId.Value;
        }

        if (string.IsNullOrEmpty(business.FormCode) && !string.IsNullOrEmpty(vo.FormCode))
        {
            business.FormCode = vo.FormCode;
        }

        if (string.IsNullOrEmpty(business.ProcessNumber) && !string.IsNullOrEmpty(vo.ProcessNumber))
        {
            business.ProcessNumber = vo.ProcessNumber;
        }

        if (string.IsNullOrEmpty(business.FormDataPc) && !string.IsNullOrEmpty(vo.FormDataPc))
        {
            business.FormDataPc = vo.FormDataPc;
        }

        if (string.IsNullOrEmpty(business.FormDataApp) && !string.IsNullOrEmpty(vo.FormDataApp))
        {
            business.FormDataApp = vo.FormDataApp;
        }

        if (string.IsNullOrEmpty(business.TemplateMark) && !string.IsNullOrEmpty(vo.TemplateMark))
        {
            business.TemplateMark = vo.TemplateMark;
        }

        if (string.IsNullOrEmpty(business.StartUsername) && !string.IsNullOrEmpty(vo.UserName))
        {
            business.StartUsername = vo.UserName;
        }

        if (string.IsNullOrEmpty(business.Remark) && !string.IsNullOrEmpty(vo.Remark))
        {
            business.Remark = vo.Remark;
        }

        if (business.IsDel == default && vo.IsDel != default)
        {
            business.IsDel = vo.IsDel;
        }

        if (string.IsNullOrEmpty(business.CreateUser) && !string.IsNullOrEmpty(vo.CreateUser))
        {
            business.CreateUser = vo.CreateUser;
        }

        if (business.CreateTime == default && vo.CreateTime.HasValue)
        {
            business.CreateTime = vo.CreateTime.Value;
        }

        if (string.IsNullOrEmpty(business.UpdateUser) && !string.IsNullOrEmpty(vo.UpdateUser))
        {
            business.UpdateUser = vo.UpdateUser;
        }

        if (business.UpdateTime == default && vo.UpdateTime.HasValue)
        {
            business.UpdateTime = vo.UpdateTime.Value;
        }
    }
}