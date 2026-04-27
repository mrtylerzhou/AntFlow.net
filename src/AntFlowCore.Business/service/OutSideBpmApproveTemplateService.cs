using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class OutSideBpmApproveTemplateService : IOutSideBpmApproveTemplateService
{
    private readonly IBpmProcessAppApplicationRepository _bpmProcessAppApplicationRepository;

    public OutSideBpmApproveTemplateService(
        IOutSideBpmApproveTemplateRepository repository,
        IBpmProcessAppApplicationRepository bpmProcessAppApplicationRepository
    )
    {
        _repository = repository;
        _bpmProcessAppApplicationRepository = bpmProcessAppApplicationRepository;
    }

    public IOutSideBpmApproveTemplateRepository _repository { get; }

    public ResultAndPage<OutSideBpmApproveTemplateVo> ListPage(PageDto pageDto, OutSideBpmApproveTemplateVo vo)
    {
        Page<OutSideBpmApproveTemplateVo> page = PageUtils.GetPageByPageDto<OutSideBpmApproveTemplateVo>(pageDto);
        PagingInfo pagingInfo = page.ToPagingInfo();

        var templates = _repository.ListPage(a => a.IsDel == 0, pagingInfo);

        List<OutSideBpmApproveTemplateVo> outSideBpmApproveTemplateVos = templates
            .Where(a => string.IsNullOrWhiteSpace(vo.ApproveTypeName) || a.ApproveTypeName.Contains(vo.ApproveTypeName))
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

        return PageUtils.GetResultAndPage(page.Of(outSideBpmApproveTemplateVos, pagingInfo));
    }

    public List<OutSideBpmApproveTemplateVo> SelectListByTemp(int applicationId)
    {
        List<OutSideBpmApproveTemplate> templates = _repository.Find(t => t.IsDel == 0 && t.ApplicationId == applicationId);

        if (templates == null || !templates.Any())
        {
            return new List<OutSideBpmApproveTemplateVo>();
        }
        var app = _bpmProcessAppApplicationRepository.GetQueryable()
            .Where(t => t.Id == applicationId && t.IsDel == 0)
            .FirstOrDefault();
        return templates
            .Select(o => new OutSideBpmApproveTemplateVo
            {
                Id = o.Id,
                ApplicationId = o.ApplicationId,
                ApplicationName = app?.BusinessCode,
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

    public OutSideBpmApproveTemplateVo Detail(long id)
    {
        OutSideBpmApproveTemplate entity = _repository.Find(a => a.Id == id).FirstOrDefault() ?? new OutSideBpmApproveTemplate();
        OutSideBpmApproveTemplateVo vo = entity.MapToVo();
        return vo;
    }

    public void Edit(OutSideBpmApproveTemplateVo vo)
    {
        long exist = _repository.Count(a => a.IsDel == 0
                    && a.ApplicationId == vo.ApplicationId
                    && a.ApproveTypeId == vo.ApproveTypeId);

        if (exist > 0)
        {
        }

        var now = DateTime.Now;

        OutSideBpmApproveTemplate? templateEntity = _repository.Find(a => a.Id == vo.Id).FirstOrDefault();

        if (templateEntity != null)
        {
            vo.CopyTo(templateEntity);
            templateEntity.UpdateUser = SecurityUtils.GetLogInEmpNameSafe();
            templateEntity.UpdateTime = now;
            _repository.Update(templateEntity);
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

        _repository.Add(newEntity);
    }

    public void Delete(long id)
    {
        _repository.DeleteById(id);
    }
}
