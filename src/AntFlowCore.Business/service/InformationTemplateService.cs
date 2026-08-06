using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service
{
    public class InformationTemplateService : IInformationTemplateService
    {
        public InformationTemplateService(
            IInformationTemplateRepository repository
        )
        {
            _repository = repository;
        }

        public IInformationTemplateRepository _repository { get; }

        public ResultAndPage<InformationTemplateVo> List(PageDto pageDto, InformationTemplateVo informationTemplateVo)
        {
            Page<InformationTemplateVo> page = PageUtils.GetPageByPageDto<InformationTemplateVo>(pageDto);
            Expression<Func<InformationTemplate, bool>> expression = a => a.IsDel == 0;
            if (!string.IsNullOrEmpty(informationTemplateVo.Name))
            {
                expression = expression.And(a => a.Name.Contains(informationTemplateVo.Name));
            }

            PagingInfo  pagingInfo = page.ToPagingInfo();
            var list= this._repository.GetInformationTemplateByExpression(expression,  pagingInfo);
            List<InformationTemplate> informationTemplates = list;
              
            List<InformationTemplateVo> results = new List<InformationTemplateVo>();
            foreach (InformationTemplate informationTemplate in informationTemplates)
            {
                InformationTemplateVo templateVo = informationTemplate.MapToVo();
                templateVo.JumpUrlValue = JumpUrlEnum.GetDescByCode(informationTemplate.JumpUrl);
                templateVo.StatusValue = informationTemplate.Status == 0 ? "启用" : "禁用";
                results.Add(templateVo);
            }

            return PageUtils.GetResultAndPage(page.Of(results, pagingInfo));
        }

        public long Edit(InformationTemplateVo informationTemplateVo)
        {
            Expression<Func<InformationTemplate, bool>> expression = a =>
                a.IsDel == 0 && a.Name == informationTemplateVo.Name;
            if (informationTemplateVo.Id != null && informationTemplateVo.Id > 0)
            {
                expression = expression.And(a => a.Id == informationTemplateVo.Id);
            }
            List<InformationTemplate> list = _repository.GetQueryable()
                .Where(expression)
                .ToList();
            if (list.Count > 0)
            {
                throw new AFBizException("模板名称重复");
            }

            InformationTemplate informationTemplate = informationTemplateVo.MapToEntity();

            if (informationTemplate.Id > 0)
            {
                if (informationTemplate.Status == 1)
                {
                    // Template in-use check removed (dependent services deleted)
                }

                informationTemplate.UpdateUser = SecurityUtils.GetLogInEmpIdSafe();
            }
            else
            {
                informationTemplate.CreateUser = SecurityUtils.GetLogInEmpNameSafe();
                informationTemplate.UpdateUser = SecurityUtils.GetLogInEmpNameSafe();
                informationTemplate.TenantId = MultiTenantUtil.GetCurrentTenantId();
                informationTemplate.Num = StringConstants.BIG_WHITE_BLANK;
                informationTemplate.MailTitle ??= StringConstants.BIG_WHITE_BLANK;
                informationTemplate.MailContent ??= StringConstants.BIG_WHITE_BLANK;
                informationTemplate.NoteContent ??= StringConstants.BIG_WHITE_BLANK;
                informationTemplate.SystemTitle ??= StringConstants.BIG_WHITE_BLANK;
                informationTemplate.CreateTime = DateTime.Now;
                informationTemplate.UpdateTime = DateTime.Now;
                _repository.Add(informationTemplate);
                informationTemplate.Num = $"LCTZ_{informationTemplate.Id:D3}";
            }

            _repository.Update(informationTemplate);
            return informationTemplate.Id;
        }

        public List<InformationTemplateVo> GetList()
        {
            List<InformationTemplate> templates = _repository.GetQueryable()
                .Where(a => a.IsDel == 0 && a.IsDefault == 1)
                .ToList();

            List<InformationTemplateVo> results = new List<InformationTemplateVo>();
            foreach (InformationTemplate template in templates)
            {
                InformationTemplateVo templateVo = template.MapToVo();
                templateVo.JumpUrlValue = JumpUrlEnum.GetDescByCode(template.JumpUrl);
                templateVo.StatusValue = template.Status == 0 ? "启用" : "禁用";
                results.Add(templateVo);
            }

            return results;
        }

        public void SetList(List<InformationTemplateVo> vos)
        {
            if (vos == null || vos.Count == 0) return;

            foreach (InformationTemplateVo vo in vos)
            {
                if (vo.Id == null) continue;
                InformationTemplate template = _repository.GetQueryable()
                    .Where(a => a.Id == vo.Id)
                    .FirstOrDefault();
                if (template != null)
                {
                    template.IsDefault = vo.IsDefault;
                    template.UpdateUser = SecurityUtils.GetLogInEmpNameSafe();
                    template.UpdateTime = DateTime.Now;
                    _repository.Update(template);
                }
            }
        }

        public InformationTemplateVo GetInformationTemplateById(long id)
        {
            InformationTemplate informationTemplate = _repository.GetQueryable()
                .Where(a => a.Id == id)
                .FirstOrDefault() ?? new InformationTemplate();
            if (informationTemplate == null)
            {
                throw new AFBizException("模板消息通知模板不存在");
            }
            InformationTemplateVo templateVo = informationTemplate.MapToVo();
            templateVo.JumpUrlValue = JumpUrlEnum.GetDescByCode(informationTemplate.JumpUrl);
            templateVo.StatusValue = informationTemplate.Status == 0 ? "启用" : "禁用";
            return templateVo;
        }
    }
}
