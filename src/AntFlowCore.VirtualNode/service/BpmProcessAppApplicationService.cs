using System.Web;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.VirtualNode.service;

public class BpmProcessAppApplicationService : IBpmProcessAppApplicationService
{
    private const int appCommonId = 2;
    public const int pcCommonId = 1;

    public BpmProcessAppApplicationService(IBpmProcessAppApplicationRepository repository)
    {
        _repository = repository;
    }

    public IBpmProcessAppApplicationRepository _repository { get; }

    public BpmProcessAppApplicationVo GetApplicationUrl(String businessCode, String processKey)
    {
        if (string.IsNullOrEmpty(businessCode) && string.IsNullOrEmpty(processKey))
        {
            return null;
        }

        List<BpmProcessAppApplication> list = _repository.GetApplicationUrl(businessCode, processKey);
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
        return _repository.SelectApplicationList();
    }

    public ResultAndPage<BpmProcessAppApplicationVo> ApplicationsPageList(PageDto page, BpmProcessAppApplicationVo vo)
    {
        return ApplicationsNewList(page, vo);
    }

    public ResultAndPage<BpmProcessAppApplicationVo> ApplicationsNewList(PageDto pageDto, BpmProcessAppApplicationVo vo)
    {
        SortedDictionary<String, SortTypeEnum> orderFieldMap = new SortedDictionary<string, SortTypeEnum>();
        orderFieldMap.Add("id", SortTypeEnum.DESC);
        Page<BpmProcessAppApplicationVo> page =
            PageUtils.GetPageByPageDto<BpmProcessAppApplicationVo>(pageDto, orderFieldMap);
        page.Records = NewListPage(page, vo);
        GetPcProcessData(page);
        return PageUtils.GetResultAndPage(page);
    }

    public Page<BpmProcessAppApplicationVo> GetPcProcessData(Page<BpmProcessAppApplicationVo> page)
    {
        var records = page.Records.Select(o =>
        {
            o.Name = !string.IsNullOrWhiteSpace(o.Entrance)
                  ? $"{o.Title},{o.Entrance}"
                  : o.Title;

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

            o.ApplyTypeName = ApplyType.GetDescByCode(o.ApplyType);

            return o;
        }).ToList();

        page.Records = records;
        return page;
    }

    private List<BpmProcessAppApplicationVo> NewListPage(Page<BpmProcessAppApplicationVo> page,
        BpmProcessAppApplicationVo vo)
    {
        PagingInfo basePagingInfo = page.ToPagingInfo();
        List<BpmProcessAppApplicationVo> list = _repository.NewListPage(basePagingInfo);
        page.Total = (int)basePagingInfo.Count;
        return list;
    }

    public bool AddBpmProcessAppApplication(BpmProcessAppApplicationVo vo)
    {
        vo.Route = System.Net.WebUtility.HtmlDecode(vo.Route);
        BpmProcessAppApplication entity = vo.MapToEntity();
        entity.IsAll = 0;

        if (vo.Id != null && vo.Id > 0)
        {
            entity.EffectiveSource = vo.EffectiveSource;
            entity.ProcessKey = entity.Title;
            return _repository.UpdateApplication(entity);
        }
        else
        {
            entity.CreateTime = DateTime.Now;
            entity.EffectiveSource = vo.EffectiveSource;

            entity.ProcessKey = $"{vo.BusinessCode}_{StrUtils.GetFirstLetters(vo.Title)}";

            if (_repository.ExistsByTitle(vo.Title))
            {
                throw new AFBizException("该选项名称已存在");
            }

            return _repository.InsertApplication(entity);
        }
    }
}
