using antflowcore.dto;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.service.interf.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Entity;
using FreeSql;
using FreeSql.Internal.Model;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.repository;

public class OutSideBpmApproveTemplateService : AFBaseCurdRepositoryService<OutSideBpmApproveTemplate>, IOutSideBpmApproveTemplateService
{
   private IFreeSql freeSql;
   public OutSideBpmApproveTemplateService(IFreeSql freeSql) : base(freeSql)
   {
      this.freeSql = freeSql;
   }

   /// <summary>
   /// 分页查询审批模板
   /// </summary>
   public ResultAndPage<OutSideBpmApproveTemplateVo> ListPage(PageDto pageDto, OutSideBpmApproveTemplateVo vo)
   {
      Page<OutSideBpmApproveTemplateVo> page = PageUtils.GetPageByPageDto<OutSideBpmApproveTemplateVo>(pageDto);
      BasePagingInfo basePagingInfo = page.ToPagingInfo();
      List<OutSideBpmApproveTemplateVo> outSideBpmApproveTemplateVos = Frsql
          .Select<OutSideBpmApproveTemplate>()
          .Where(a => a.IsDel == 0)
          .WhereIf(!string.IsNullOrWhiteSpace(vo.ApproveTypeName),
              a => a.ApproveTypeName.Contains(vo.ApproveTypeName))
          .OrderByDescending(a => a.CreateTime)
          .Page(basePagingInfo)
          .ToList()
          .Select(a => new OutSideBpmApproveTemplateVo
          {
             Id = a.Id,
             BusinessPartyId = a.BusinessPartyId,
             ApplicationId = a.ApplicationId,
             ApproveTypeId = a.ApproveTypeId,
             ApproveTypeName = a.ApproveTypeName,
             ApiClientId = a.ApiClientId,
             ApiClientSecret = a.ApiClientSecret,
             ApiToken = a.ApiToken,
             ApiUrl = a.ApiUrl,
             Remark = a.Remark,
             IsDel = a.IsDel,
             CreateUserId = a.CreateUserId,
             CreateUser = a.CreateUser,
             CreateTime = a.CreateTime
          }).ToList();

      return PageUtils.GetResultAndPage(page.Of(outSideBpmApproveTemplateVos, basePagingInfo));
   }

   /// <summary>
   /// 根据 applicationId 获取模板列表
   /// </summary>
   public List<OutSideBpmApproveTemplateVo> SelectListByTemp(int applicationId)
   {
      List<OutSideBpmApproveTemplate> templates = this.baseRepo
          .Where(t => t.IsDel == 0 && t.ApplicationId == applicationId)
          .ToList();

      if (templates == null || !templates.Any())
      {
         return new List<OutSideBpmApproveTemplateVo>();
      }
      IBaseRepository<BpmProcessAppApplication> appRepo = freeSql.GetRepository<BpmProcessAppApplication>();
      var app = appRepo.Where(t => t.Id == applicationId && t.IsDel == 0).First();
      return templates
          .Select(o => new OutSideBpmApproveTemplateVo
          {
             Id = o.Id,
             ApplicationId = o.ApplicationId,
             ApplicationName=app.BusinessCode,
             ApproveTypeId = o.ApproveTypeId,
             ApproveTypeName = o.ApproveTypeName,
             ApiClientId = o.ApiClientId,
             ApiClientSecret = o.ApiClientSecret,
             ApiToken = o.ApiToken,
             ApiUrl = o.ApiUrl,
             Remark = o.Remark,
             CreateTime = o.CreateTime
          })
          .ToList();
   }

   /// <summary>
   /// 查询详情
   /// </summary>
   public OutSideBpmApproveTemplateVo Detail(long id)
   {
      OutSideBpmApproveTemplate entity = this.baseRepo
          .Where(a => a.Id == id)
          .ToOne();
      OutSideBpmApproveTemplateVo vo = entity.MapToVo();
      return vo;
   }

   /// <summary>
   /// 编辑或新增模板
   /// </summary>
   public void Edit(OutSideBpmApproveTemplateVo vo)
   {
      long exist = this.baseRepo
          .Where(x => x.IsDel == 0
                      && x.ApplicationId == vo.ApplicationId
                      && x.ApproveTypeId == vo.ApproveTypeId)
          .Count();

      if (exist > 0)
      {
        // throw new AFBizException($"{vo.ApproveTypeName} 审批模板已存在");
      }

      var now = DateTime.Now;

      OutSideBpmApproveTemplate templateEntity = this.baseRepo
              .Where(x => x.Id == exist)
              .First();

      if (templateEntity != null)
      {
         vo.CopyTo(templateEntity);
         templateEntity.UpdateUser = SecurityUtils.GetLogInEmpNameSafe();
         templateEntity.UpdateTime = now;
         this.baseRepo.Update(templateEntity);
         return;
      }

      var newEntity = new OutSideBpmApproveTemplate();
      vo.CopyTo(newEntity);
      newEntity.IsDel = 0;
      newEntity.CreateUser = SecurityUtils.GetLogInEmpNameSafe();
      newEntity.CreateUserId = SecurityUtils.GetLogInEmpIdSafe();
      newEntity.CreateTime = now;
      newEntity.UpdateUser = newEntity.CreateUser;
      newEntity.UpdateTime = now;

      this.baseRepo.Insert(newEntity);
   }

   /// <summary>
   /// 删除（逻辑删除）
   /// </summary>
   public void Delete(long id)
   {
      this.Frsql.Update<OutSideBpmApproveTemplate>()
          .Set(x => x.IsDel, 1)
          .Where(x => x.Id == id)
          .ExecuteAffrows();
   }
}