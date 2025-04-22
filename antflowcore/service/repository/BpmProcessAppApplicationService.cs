using antflowcore.constant.enums;
using antflowcore.dto;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.util;
using AntFlowCore.Entity;
using AntFlowCore.Vo;
using System.Web;

namespace antflowcore.service.repository;

public class BpmProcessAppApplicationService : AFBaseCurdRepositoryService<BpmProcessAppApplication>
{
    // app's frequently used function's id
    private const int appCommonId = 2;

    // pc's frequently used function's id
    public const int pcCommonId = 1;

    public BpmProcessAppApplicationService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public BpmProcessAppApplicationVo GetApplicationUrl(String businessCode, String processKey)
    {
        if (string.IsNullOrEmpty(businessCode) && string.IsNullOrEmpty(processKey))
        {
            return null;
        }

        List<BpmProcessAppApplication> list = this.baseRepo.Where(a =>
            a.BusinessCode.Equals(businessCode) && a.ProcessKey.Equals(processKey) && a.IsDel == 0).ToList();
        if (ObjectUtils.IsEmpty(list))
        {
            return null;
        }

        BpmProcessAppApplication application = list[0];
        BpmProcessAppApplicationVo vo = application.MapToVo();

        if (!string.IsNullOrEmpty(vo.LookUrl))
        {
            vo.LookUrl = HttpUtility.HtmlDecode(vo.LookUrl);
        }

        if (!string.IsNullOrEmpty(vo.SubmitUrl))
        {
            vo.SubmitUrl = HttpUtility.HtmlDecode(vo.SubmitUrl);
        }

        if (!string.IsNullOrEmpty(vo.ConditionUrl))
        {
            vo.ConditionUrl = HttpUtility.HtmlDecode(vo.ConditionUrl);
        }

        return vo;
    }

    public List<BpmProcessAppApplication> SelectApplicationList()
    {
        List<BpmProcessAppApplication> bpmProcessAppApplications = this.baseRepo.Where(a => true).ToList();
        return bpmProcessAppApplications;
    }

    public ResultAndPage<BpmProcessAppApplicationVo> ApplicationsPageList(PageDto page, BpmProcessAppApplicationVo vo)
    {
        return ApplicationsNewList(page, vo);
    }

    public ResultAndPage<BpmProcessAppApplicationVo> ApplicationsNewList(PageDto pageDto, BpmProcessAppApplicationVo vo)
    {
        //排序字段链表
        SortedDictionary<String, SortTypeEnum> orderFieldMap = new SortedDictionary<string, SortTypeEnum>();
        orderFieldMap.Add("id", SortTypeEnum.DESC);
        Page<BpmProcessAppApplicationVo> page =
            PageUtils.GetPageByPageDto<BpmProcessAppApplicationVo>(pageDto, orderFieldMap);
        page.Records = this.NewListPage(page, vo);
        this.GetPcProcessData(page);
        return PageUtils.GetResultAndPage(page);
    }

    public Page<BpmProcessAppApplicationVo> GetPcProcessData(Page<BpmProcessAppApplicationVo> page)
    {
        var records = page.Records.Select(o =>
        {
            // 设置 Name
            o.Name = !string.IsNullOrWhiteSpace(o.Entrance)
                ? $"{o.Title},{o.Entrance}"
                : o.Title;

            // 处理 TypeIds -> ProcessTypes
            if (!string.IsNullOrWhiteSpace(o.TypeIds))
            {
                var typeIds = o.TypeIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var list = new List<long>();

                if (typeIds.Length > 1)
                {
                    foreach (var typeId in typeIds)
                    {
                        var stringList = new List<string> { appCommonId.ToString(), pcCommonId.ToString() };
                        if (!stringList.Contains(typeId))
                        {
                            if (long.TryParse(typeId, out var longId))
                            {
                                list.Add(longId);
                            }
                        }
                    }

                    o.ProcessTypes = list;
                }
                else
                {
                    if (long.TryParse(o.TypeIds, out var longId))
                    {
                        list.Add(longId);
                    }

                    o.ProcessTypes = list;
                }
            }

            // 设置 ApplyTypeName
            o.ApplyTypeName = ApplyType.GetDescByCode(o.ApplyType);

            return o;
        }).ToList();

        page.Records = records;
        return page;
    }

    private List<BpmProcessAppApplicationVo> NewListPage(Page<BpmProcessAppApplicationVo> page,
        BpmProcessAppApplicationVo vo)
    {
        List<BpmProcessAppApplicationVo> list = this.Frsql.Select<BpmProcessAppApplication, OutSideBpmBusinessParty>()
            .LeftJoin((app, party) => app.BusinessCode == party.BusinessPartyMark)
            .Where((app, party) => app.IsDel == 0)
            .OrderByDescending((app, party) => app.CreateTime)
            .Page(page.Current, page.Size)
            .ToList((app, party) => new BpmProcessAppApplicationVo
            {
                Id = app.Id,
                BusinessName = party.Name,
                BusinessPartyId = party.Id,
                Title = app.Title,
                BusinessCode = app.BusinessCode,
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
                CreateUserId = app.CreateUserId,
                CreateTime = app.CreateTime
            });
        return list;
    }

    public bool AddBpmProcessAppApplication(BpmProcessAppApplicationVo vo)
    {
        // 反转义 HTML 中的实体字符
        vo.Route = System.Net.WebUtility.HtmlDecode(vo.Route);
        BpmProcessAppApplication entity = vo.MapToEntity();
        entity.IsAll = 0;

        if (vo.Id != null && vo.Id > 0)
        {
            entity.EffectiveSource = vo.EffectiveSource;
            return this.baseRepo.Update(entity) > 0;
        }
        else
        {
            entity.CreateTime = DateTime.Now;
            entity.EffectiveSource = vo.EffectiveSource;

            // 自动生成流程key
            entity.ProcessKey = $"{vo.BusinessCode}_{StrUtils.GetFirstLetters(vo.Title)}";

            // 校验名称是否重复
            var exists = this.baseRepo
                .Select
                .Where(a => a.Title == vo.Title && a.IsDel == 0)
                .Any();

            if (exists)
            {
                throw new AFBizException("该选项名称已存在");
            }

            return this.baseRepo.Insert(entity) != null;
        }
    }
}