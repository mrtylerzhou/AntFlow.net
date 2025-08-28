using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using FreeSql.Internal.Model;
using System.Net;
using System.Web;

namespace AntFlow.Core.Service.Repository;

public class BpmProcessAppApplicationService : AFBaseCurdRepositoryService<BpmProcessAppApplication>,
    IBpmProcessAppApplicationService
{
    // app's frequently used function's id
    private const int appCommonId = 2;

    // pc's frequently used function's id
    public const int pcCommonId = 1;

    public BpmProcessAppApplicationService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public BpmProcessAppApplicationVo GetApplicationUrl(string businessCode, string processKey)
    {
        if (string.IsNullOrEmpty(businessCode) && string.IsNullOrEmpty(processKey))
        {
            return null;
        }

        List<BpmProcessAppApplication> list = baseRepo.Where(a =>
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
        List<BpmProcessAppApplication> bpmProcessAppApplications = baseRepo.Where(a => true).ToList();
        return bpmProcessAppApplications;
    }

    public ResultAndPage<BpmProcessAppApplicationVo> ApplicationsNewList(PageDto pageDto, BpmProcessAppApplicationVo vo)
    {
        //???????????
        SortedDictionary<string, SortTypeEnum> orderFieldMap = new();
        orderFieldMap.Add("id", SortTypeEnum.DESC);
        Page<BpmProcessAppApplicationVo> page =
            PageUtils.GetPageByPageDto<BpmProcessAppApplicationVo>(pageDto, orderFieldMap);
        page.Records = NewListPage(page, vo);
        GetPcProcessData(page);
        return PageUtils.GetResultAndPage(page);
    }

    public Page<BpmProcessAppApplicationVo> GetPcProcessData(Page<BpmProcessAppApplicationVo> page)
    {
        List<BpmProcessAppApplicationVo>? records = page.Records.Select(o =>
        {
            // ???? Name
            o.Name = !string.IsNullOrWhiteSpace(o.Entrance)
                ? $"{o.Title},{o.Entrance}"
                : o.Title;

            // ???? TypeIds -> ProcessTypes
            if (!string.IsNullOrWhiteSpace(o.TypeIds))
            {
                string[]? typeIds = o.TypeIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                List<long>? list = new();

                if (typeIds.Length > 1)
                {
                    foreach (string? typeId in typeIds)
                    {
                        List<string>? stringList = new() { appCommonId.ToString(), pcCommonId.ToString() };
                        if (!stringList.Contains(typeId))
                        {
                            if (long.TryParse(typeId, out long longId))
                            {
                                list.Add(longId);
                            }
                        }
                    }

                    o.ProcessTypes = list;
                }
                else
                {
                    if (long.TryParse(o.TypeIds, out long longId))
                    {
                        list.Add(longId);
                    }

                    o.ProcessTypes = list;
                }
            }

            // ???? ApplyTypeName
            o.ApplyTypeName = ApplyType.GetDescByCode(o.ApplyType);

            return o;
        }).ToList();

        page.Records = records;
        return page;
    }

    public bool AddBpmProcessAppApplication(BpmProcessAppApplicationVo vo)
    {
        // ????? HTML ?§Ö???????
        vo.Route = WebUtility.HtmlDecode(vo.Route);
        BpmProcessAppApplication entity = vo.MapToEntity();
        entity.IsAll = 0;

        if (vo.Id != null && vo.Id > 0)
        {
            entity.EffectiveSource = vo.EffectiveSource;
            return baseRepo.Update(entity) > 0;
        }

        entity.CreateTime = DateTime.Now;
        entity.EffectiveSource = vo.EffectiveSource;

        // ???????????key
        entity.ProcessKey = $"{vo.BusinessCode}_{StrUtils.GetFirstLetters(vo.Title)}";

        // §µ????????????
        bool exists = baseRepo
            .Select
            .Where(a => a.Title == vo.Title && a.IsDel == 0)
            .Any();

        if (exists)
        {
            throw new AFBizException("??????????????");
        }

        return baseRepo.Insert(entity) != null;
    }

    public ResultAndPage<BpmProcessAppApplicationVo> ApplicationsPageList(PageDto page, BpmProcessAppApplicationVo vo)
    {
        return ApplicationsNewList(page, vo);
    }

    private List<BpmProcessAppApplicationVo> NewListPage(Page<BpmProcessAppApplicationVo> page,
        BpmProcessAppApplicationVo vo)
    {
        BasePagingInfo basePagingInfo = page.ToPagingInfo();
        List<BpmProcessAppApplicationVo> list = Frsql.Select<BpmProcessAppApplication, OutSideBpmBusinessParty>()
            .LeftJoin((app, party) => app.BusinessCode == party.BusinessPartyMark)
            .Where((app, party) => app.IsDel == 0)
            .OrderByDescending((app, party) => app.CreateTime)
            .Page(basePagingInfo)
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
        page.Total = (int)basePagingInfo.Count;
        return list;
    }
}