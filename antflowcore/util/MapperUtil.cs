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
         IsLowCodeFlow = node.IsLowCodeFlow,
         IsDynamicCondition = node.IsDynamicCondition,
         IsParallel = node.IsParallel,
         NoHeaderAction = node.NoHeaderAction,
         ExtraFlags = node.ExtraFlags
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
         IsLowCodeFlow = vo.IsLowCodeFlow,
         NoHeaderAction = vo.NoHeaderAction,
         ExtraFlags = vo.ExtraFlags
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
         Id = vo.Id,
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
         Id = template.Id,
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
         Id = business.Id,
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
         BusinessPartyId = vo.BusinessPartyId ?? 0,
         BpmnConfId = vo.BpmnConfId ?? 0,
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

      if (business.BusinessPartyId == default && vo.BusinessPartyId != null && vo.BusinessPartyId > 0)
      {
         business.BusinessPartyId = vo.BusinessPartyId.Value;
      }

      if (business.BpmnConfId == default && vo.BpmnConfId != null && vo.BpmnConfId > 0)
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
   public static OutSideBpmApproveTemplateVo MapToVo(this OutSideBpmApproveTemplate entity)
   {
      if (entity == null) return null;

      return new OutSideBpmApproveTemplateVo
      {
         Id = entity.Id,
         BusinessPartyId = entity.BusinessPartyId,
         ApplicationId = entity.ApplicationId,
         ApproveTypeId = entity.ApproveTypeId,
         ApproveTypeName = entity.ApproveTypeName,
         ApiClientId = entity.ApiClientId,
         ApiClientSecret = entity.ApiClientSecret,
         ApiToken = entity.ApiToken,
         ApiUrl = entity.ApiUrl,
         Remark = entity.Remark,
         IsDel = entity.IsDel,
         CreateUser = entity.CreateUser,
         CreateTime = entity.CreateTime,
         UpdateUser = entity.UpdateUser,
         UpdateTime = entity.UpdateTime,
         CreateUserId = entity.CreateUserId
      };
   }

   public static OutSideBpmApproveTemplate MapToEntity(this OutSideBpmApproveTemplateVo vo)
   {
      if (vo == null) return null;

      return new OutSideBpmApproveTemplate
      {

         BusinessPartyId = vo.BusinessPartyId,
         ApplicationId = vo.ApplicationId,
         ApproveTypeId = vo.ApproveTypeId,
         ApproveTypeName = vo.ApproveTypeName,
         ApiClientId = vo.ApiClientId,
         ApiClientSecret = vo.ApiClientSecret,
         ApiToken = vo.ApiToken,
         ApiUrl = vo.ApiUrl,
         Remark = vo.Remark,
         IsDel = vo.IsDel,
         CreateUser = vo.CreateUser,
         CreateTime = vo.CreateTime,
         UpdateUser = vo.UpdateUser,
         UpdateTime = vo.UpdateTime,
         CreateUserId = vo.CreateUserId
      };
   }
   public static void CopyTo(this OutSideBpmApproveTemplateVo vo, OutSideBpmApproveTemplate entity)
   {
      if (entity == null || vo == null) return;

      if (entity.BusinessPartyId == default && vo.BusinessPartyId != null)
         entity.BusinessPartyId = vo.BusinessPartyId.Value;

      if (entity.ApplicationId == default && vo.ApplicationId.HasValue)
         entity.ApplicationId = vo.ApplicationId.Value;

      if (entity.ApproveTypeId == default && vo.ApproveTypeId.HasValue)
         entity.ApproveTypeId = vo.ApproveTypeId.Value;

      if (string.IsNullOrWhiteSpace(entity.ApproveTypeName) && !string.IsNullOrWhiteSpace(vo.ApproveTypeName))
         entity.ApproveTypeName = vo.ApproveTypeName;

      if (string.IsNullOrWhiteSpace(entity.ApiClientId) && !string.IsNullOrWhiteSpace(vo.ApiClientId))
         entity.ApiClientId = vo.ApiClientId;

      if (string.IsNullOrWhiteSpace(entity.ApiClientSecret) && !string.IsNullOrWhiteSpace(vo.ApiClientSecret))
         entity.ApiClientSecret = vo.ApiClientSecret;

      if (string.IsNullOrWhiteSpace(entity.ApiToken) && !string.IsNullOrWhiteSpace(vo.ApiToken))
         entity.ApiToken = vo.ApiToken;

      if (string.IsNullOrWhiteSpace(entity.ApiUrl) && !string.IsNullOrWhiteSpace(vo.ApiUrl))
         entity.ApiUrl = vo.ApiUrl;

      if (string.IsNullOrWhiteSpace(entity.Remark) && !string.IsNullOrWhiteSpace(vo.Remark))
         entity.Remark = vo.Remark;

      if (string.IsNullOrWhiteSpace(entity.CreateUser) && !string.IsNullOrWhiteSpace(vo.CreateUser))
         entity.CreateUser = vo.CreateUser;

      if (string.IsNullOrWhiteSpace(entity.CreateUserId) && !string.IsNullOrWhiteSpace(vo.CreateUserId))
         entity.CreateUserId = vo.CreateUserId;

      if (entity.CreateTime == default && vo.CreateTime.HasValue)
         entity.CreateTime = vo.CreateTime.Value;

      if (string.IsNullOrWhiteSpace(entity.UpdateUser) && !string.IsNullOrWhiteSpace(vo.UpdateUser))
         entity.UpdateUser = vo.UpdateUser;

      if (entity.UpdateTime == default && vo.UpdateTime.HasValue)
         entity.UpdateTime = vo.UpdateTime.Value;
   }
   #region Entity -> VO 转换
   public static OutSideBpmBusinessPartyVo MapToVo(this OutSideBpmBusinessParty entity)
   {
      if (entity == null) return null;

      return new OutSideBpmBusinessPartyVo
      {
         Id = entity.Id,
         BusinessPartyMark = entity.BusinessPartyMark,
         Name = entity.Name,
         Type = entity.Type,
         Remark = entity.Remark,
         IsDel = entity.IsDel,
         CreateUser = entity.CreateUser,
         CreateTime = entity.CreateTime,
         UpdateUser = entity.UpdateUser,
         UpdateTime = entity.UpdateTime,

      };
   }
   #endregion

   #region VO -> Entity 转换
   public static OutSideBpmBusinessParty MapToEntity(this OutSideBpmBusinessPartyVo vo)
   {
      if (vo == null) return null;

      return new OutSideBpmBusinessParty
      {
         BusinessPartyMark = vo.BusinessPartyMark,
         Name = vo.Name,
         Type = vo.Type ?? 2,
         Remark = vo.Remark,
         IsDel = vo.IsDel,
         CreateUser = vo.CreateUser,
         CreateTime = vo.CreateTime,
         UpdateUser = vo.UpdateUser,
         UpdateTime = vo.UpdateTime,
      };
   }
   #endregion

   public static void CopyTo(this OutSideBpmBusinessPartyVo vo, OutSideBpmBusinessParty entity)
   {
      if (string.IsNullOrEmpty(entity.BusinessPartyMark) && !string.IsNullOrEmpty(vo.BusinessPartyMark))
         entity.BusinessPartyMark = vo.BusinessPartyMark;

      if (string.IsNullOrEmpty(entity.Name) && !string.IsNullOrEmpty(vo.Name))
         entity.Name = vo.Name;

      if (entity.Type == 0 && vo.Type.HasValue && vo.Type.Value != 0)
         entity.Type = vo.Type.Value;

      if (string.IsNullOrEmpty(entity.Remark) && !string.IsNullOrEmpty(vo.Remark))
         entity.Remark = vo.Remark;

      if (entity.IsDel == 0 && vo.IsDel != 0)
         entity.IsDel = vo.IsDel;

      if (string.IsNullOrEmpty(entity.CreateUser) && !string.IsNullOrEmpty(vo.CreateUser))
         entity.CreateUser = vo.CreateUser;

      if (entity.CreateTime == default && vo.CreateTime != default)
         entity.CreateTime = vo.CreateTime;

      if (string.IsNullOrEmpty(entity.UpdateUser) && !string.IsNullOrEmpty(vo.UpdateUser))
         entity.UpdateUser = vo.UpdateUser;

      if (entity.UpdateTime == default && vo.UpdateTime != default)
         entity.UpdateTime = vo.UpdateTime;
      entity.Name = vo.Name;
      entity.Remark = vo.Remark;
   }
   public static OutSideBpmCallbackUrlConfVo MapToVo(this OutSideBpmCallbackUrlConf entity)
   {
      if (entity == null) return null;

      return new OutSideBpmCallbackUrlConfVo
      {
         Id = entity.Id,
         BusinessPartyId = entity.BusinessPartyId,
         BpmnConfId = entity.BpmnConfId,
         FormCode = entity.FormCode,
         BpmConfCallbackUrl = entity.BpmConfCallbackUrl,
         BpmFlowCallbackUrl = entity.BpmFlowCallbackUrl,
         ApiClientId = entity.ApiClientId,
         ApiClientSecrent = entity.ApiClientSecret,
         Status = entity.Status,
         Remark = entity.Remark,
         IsDel = entity.IsDel,
         CreateUser = entity.CreateUser,
         CreateTime = entity.CreateTime,
         UpdateUser = entity.UpdateUser,
         UpdateTime = entity.UpdateTime,
      };
   }
   public static OutSideBpmCallbackUrlConf MapToEntity(this OutSideBpmCallbackUrlConfVo vo)
   {
      if (vo == null) return null;
      OutSideBpmCallbackUrlConf entity = new OutSideBpmCallbackUrlConf
      {

         BusinessPartyId = vo.BusinessPartyId ?? 0,
         BpmnConfId = vo.BpmnConfId ?? 0,
         FormCode = vo.FormCode,
         BpmConfCallbackUrl = vo.BpmConfCallbackUrl,
         BpmFlowCallbackUrl = vo.BpmFlowCallbackUrl,
         ApiClientId = vo.ApiClientId,
         ApiClientSecret = vo.ApiClientSecrent,
         Status = vo.Status ?? 0,
         Remark = vo.Remark,
         IsDel = vo.IsDel ?? 0,
         CreateUser = vo.CreateUser,
         CreateTime = vo.CreateTime,
         UpdateUser = vo.UpdateUser,
         UpdateTime = vo.UpdateTime
      };
      return entity;
   }
   public static void CopyToEntity(this OutSideBpmCallbackUrlConfVo vo, OutSideBpmCallbackUrlConf entity)
   {
      if (vo == null || entity == null) return;

      if (string.IsNullOrEmpty(entity.FormCode) && !string.IsNullOrEmpty(vo.FormCode))
         entity.FormCode = vo.FormCode;

      if (entity.Id == 0 && vo.Id != null)
         entity.Id = vo.Id.Value;

      if (entity.BusinessPartyId == 0 && vo.BusinessPartyId != null)
         entity.BusinessPartyId = vo.BusinessPartyId.Value;

      if ((entity.BpmnConfId == null || entity.BpmnConfId == 0) && vo.BpmnConfId != null && vo.BpmnConfId != 0)
         entity.BpmnConfId = vo.BpmnConfId.Value;

      if (string.IsNullOrEmpty(entity.BpmConfCallbackUrl) && !string.IsNullOrEmpty(vo.BpmConfCallbackUrl))
         entity.BpmConfCallbackUrl = vo.BpmConfCallbackUrl;

      if (string.IsNullOrEmpty(entity.BpmFlowCallbackUrl) && !string.IsNullOrEmpty(vo.BpmFlowCallbackUrl))
         entity.BpmFlowCallbackUrl = vo.BpmFlowCallbackUrl;

      if (string.IsNullOrEmpty(entity.ApiClientId) && !string.IsNullOrEmpty(vo.ApiClientId))
         entity.ApiClientId = vo.ApiClientId;

      if (string.IsNullOrEmpty(entity.ApiClientSecret) && !string.IsNullOrEmpty(vo.ApiClientSecrent))
         entity.ApiClientSecret = vo.ApiClientSecrent;

      if (entity.Status == 0 && vo.Status != null)
         entity.Status = vo.Status.Value;

      if (string.IsNullOrEmpty(entity.Remark) && !string.IsNullOrEmpty(vo.Remark))
         entity.Remark = vo.Remark;

      if (entity.IsDel == 0 && vo.IsDel != null)
         entity.IsDel = vo.IsDel.Value;

      if (string.IsNullOrEmpty(entity.CreateUser) && !string.IsNullOrEmpty(vo.CreateUser))
         entity.CreateUser = vo.CreateUser;

      if (entity.CreateTime == default && vo.CreateTime != null)
         entity.CreateTime = vo.CreateTime;

      if (string.IsNullOrEmpty(entity.UpdateUser) && !string.IsNullOrEmpty(vo.UpdateUser))
         entity.UpdateUser = vo.UpdateUser;

      if (entity.UpdateTime == default && vo.UpdateTime != null)
         entity.UpdateTime = vo.UpdateTime;
   }

}