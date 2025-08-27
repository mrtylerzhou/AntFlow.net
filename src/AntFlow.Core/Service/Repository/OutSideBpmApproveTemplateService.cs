using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using FreeSql.Internal.Model;

namespace AntFlow.Core.Service.Repository;

public class OutSideBpmApproveTemplateService : AFBaseCurdRepositoryService<OutSideBpmApproveTemplate>,
    IOutSideBpmApproveTemplateService
{
    public OutSideBpmApproveTemplateService(IFreeSql freeSql) : base(freeSql)
    {
    }

    /// <summary>
    ///     分页查询审批模板列表
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
    ///     根据 applicationId 查询模板列表
    /// </summary>
    public List<OutSideBpmApproveTemplateVo> SelectListByTemp(int applicationId)
    {
        List<OutSideBpmApproveTemplate> templates = baseRepo
            .Where(t => t.IsDel == 0 && t.ApplicationId == applicationId)
            .ToList();

        if (templates == null || !templates.Any())
        {
            return new List<OutSideBpmApproveTemplateVo>();
        }

        return templates
            .Select(o => new OutSideBpmApproveTemplateVo
            {
                Id = o.Id,
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
    ///     查询详情
    /// </summary>
    public OutSideBpmApproveTemplateVo Detail(long id)
    {
        OutSideBpmApproveTemplate entity = baseRepo
            .Where(a => a.Id == id)
            .ToOne();
        OutSideBpmApproveTemplateVo vo = entity.MapToVo();
        return vo;
    }

    /// <summary>
    ///     编辑审批模板
    /// </summary>
    public void Edit(OutSideBpmApproveTemplateVo vo)
    {
        long exist = baseRepo
            .Where(x => x.IsDel == 0
                        && x.ApplicationId == vo.ApplicationId
                        && x.ApproveTypeId == vo.ApproveTypeId)
            .Count();

        if (exist > 0)
        {
            throw new AFBizException($"{vo.ApproveTypeName} 审批类型已存在");
        }

        DateTime now = DateTime.Now;

        OutSideBpmApproveTemplate templateEntity = baseRepo
            .Where(x => x.Id == vo.Id)
            .First();

        if (templateEntity != null)
        {
            vo.CopyTo(templateEntity);
            templateEntity.UpdateUser = SecurityUtils.GetLogInEmpNameSafe();
            templateEntity.UpdateTime = now;
            baseRepo.Update(templateEntity);
            return;
        }

        OutSideBpmApproveTemplate? newEntity = new();
        vo.CopyTo(newEntity);
        newEntity.IsDel = 0;
        newEntity.CreateUser = SecurityUtils.GetLogInEmpNameSafe();
        newEntity.CreateUserId = SecurityUtils.GetLogInEmpIdSafe();
        newEntity.CreateTime = now;
        newEntity.UpdateUser = newEntity.CreateUser;
        newEntity.UpdateTime = now;

        baseRepo.Insert(newEntity);
    }

    /// <summary>
    ///     删除审批模板
    /// </summary>
    public void Delete(long id)
    {
        Frsql.Update<OutSideBpmApproveTemplate>()
            .Set(x => x.IsDel, 1)
            .Where(x => x.Id == id)
            .ExecuteAffrows();
    }
}