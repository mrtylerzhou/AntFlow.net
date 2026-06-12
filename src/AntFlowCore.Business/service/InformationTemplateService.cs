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

        public List<DefaultTemplateVo> GetList()
        {
            var result = Enum.GetValues(typeof(EventTypeEnum))
                .Cast<EventTypeEnum>()
                .Select(o =>
                {
                    var eventCode = (int)o;

                    return new DefaultTemplateVo
                    {
                        Event = eventCode,
                        EventValue = o.GetDescription(),
                        TemplateId = null,
                        TemplateName = null
                    };
                })
                .ToList();

            return result;
        }

        public void SetList(List<DefaultTemplateVo> vos)
        {
            throw new NotImplementedException();
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
