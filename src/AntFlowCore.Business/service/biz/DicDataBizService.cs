using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.biz;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service.biz;

public class DicDataBizService :  IDicDataBizSerivce
{
    private readonly IDicDataSerivce _dicDataSerivce;


    public DicDataBizService(IDicDataSerivce dicDataSerivce)
    {
        _dicDataSerivce = dicDataSerivce;
    }

    public ResultAndPage<BaseKeyValueStruVo> SelectLFActiveFormCodePageList(PageDto pageDto, TaskMgmtVO taskMgmtVO)
    {
        Page<BaseKeyValueStruVo> page = PageUtils.GetPageByPageDto<BaseKeyValueStruVo>(pageDto);
        List<DictData> dictDataList = this.SelectLFActiveFormCodePageList(page, taskMgmtVO);
        return HandleFormCodePageList(page, dictDataList, "LF");
    }

    private List<DictData> SelectLFActiveFormCodePageList(Page<BaseKeyValueStruVo> page, TaskMgmtVO taskMgmtVO)
    {

        Expression<Func<DictData, BpmnConf,bool>> expression = (a, b) => a.DictType == "lowcodeflow";
        if (taskMgmtVO.ProcessState != null && taskMgmtVO.ProcessState > 0)
        {
            expression = LinqExtensions.And(expression,(a,b)=>b.EffectiveStatus==taskMgmtVO.ProcessState);
        }

        if (!string.IsNullOrEmpty(taskMgmtVO.Description))
        {
            expression = LinqExtensions.And(expression,(a, b) =>
                a.DictType.Contains(taskMgmtVO.Description) || a.Value.Contains(taskMgmtVO.Description));
        }

        var pagingInfo = page.ToPagingInfo();
        List<DictData> dictDataList = _dicDataSerivce._repository.QueryDictDataListByExpression(expression, pagingInfo);
        page.Total = (int)pagingInfo.Count;
        return dictDataList;
    }

      public ResultAndPage<BaseKeyValueStruVo> SelectLFFormCodePageList(PageDto pageDto, TaskMgmtVO taskMgmtVo)
        {
            Page<BaseKeyValueStruVo> page = PageUtils.GetPageByPageDto<BaseKeyValueStruVo>(pageDto);
            List<DictData> dictDataList = this.SelectLFFormCodePageList(page,taskMgmtVo);
            return HandleFormCodePageList(page, dictDataList, "LF");
        }

        /// <summary>
        /// 获取 page-added DIY FormCode Page List 模板列表使用(dict_type=diylowcodeflow)
        /// </summary>
        public ResultAndPage<BaseKeyValueStruVo> SelectDIYFormCodePageList(PageDto pageDto, TaskMgmtVO taskMgmtVo)
        {
            Page<BaseKeyValueStruVo> page = PageUtils.GetPageByPageDto<BaseKeyValueStruVo>(pageDto);
            List<DictData> dictDataList = this.SelectDIYFormCodePageList(page, taskMgmtVo);
            return HandleFormCodePageList(page, dictDataList, "DIY");
        }



        List<DictData> SelectLFFormCodePageList(Page<BaseKeyValueStruVo> page, TaskMgmtVO taskMgmtVO)
        {
            Expression<Func<DictData,bool>> expression = a => a.DictType == "lowcodeflow";

            if (!string.IsNullOrEmpty(taskMgmtVO.Description))
            {
               expression= LinqExtensions.And(expression, a =>
                    a.Label.Contains(taskMgmtVO.Description) || a.Value.Contains(taskMgmtVO.Description));
            }

            PagingInfo pagingInfo = page.ToPagingInfo();
            List<DictData> dictDatas = this._dicDataSerivce
                ._repository.QueryDictDataListByExpression(expression, pagingInfo);
            page.Total = (int)pagingInfo.Count;
            return dictDatas;
        }

        List<DictData> SelectDIYFormCodePageList(Page<BaseKeyValueStruVo> page, TaskMgmtVO taskMgmtVO)
        {
            Expression<Func<DictData,bool>> expression = a => a.DictType == "diylowcodeflow";

            if (!string.IsNullOrEmpty(taskMgmtVO.Description))
            {
               expression= LinqExtensions.And(expression, a =>
                    a.Label.Contains(taskMgmtVO.Description) || a.Value.Contains(taskMgmtVO.Description));
            }

            PagingInfo pagingInfo = page.ToPagingInfo();
            List<DictData> dictDatas = this._dicDataSerivce
                ._repository.QueryDictDataListByExpression(expression, pagingInfo);
            page.Total = (int)pagingInfo.Count;
            return dictDatas;
        }

        /// <summary>私有方法: type 传入 "LF" 或 "DIY"</summary>
        private ResultAndPage<BaseKeyValueStruVo> HandleFormCodePageList(Page<BaseKeyValueStruVo> page, List<DictData> dictlist, string type)
        {
            if (dictlist == null)
            {
                return PageUtils.GetResultAndPage<BaseKeyValueStruVo>(page);
            }

            List<BaseKeyValueStruVo> results = new List<BaseKeyValueStruVo>();
            foreach (var item in dictlist)
            {
                results.Add(new BaseKeyValueStruVo
                {
                    Key = item.Value,
                    Value = item.Label,
                    CreateTime = item.CreateTime ?? DateTime.Now,
                    Type = type,
                    Remark = item.Remark
                });
            }

            var formCodes = results.Select(r => r.Key).ToList();


            if (formCodes.Any())
            {
                IBpmnConfService bpmnConfService = ServiceProviderUtils.GetService<IBpmnConfService>();

                List<BpmnConf> bpmnConfs = bpmnConfService
                    ._repository
                    .Find(a => formCodes.Contains(a.FormCode) && a.EffectiveStatus == 1);
                if (bpmnConfs != null && bpmnConfs.Any())
                {
                    var formCode2Flags = bpmnConfs.ToDictionary(
                        b => b.FormCode,
                        b => b.ExtraFlags
                    );

                    // 解析每个流程配置的通知渠道类型
                    Dictionary<string, List<int>> formCode2NoticeTypes = new Dictionary<string, List<int>>(StringComparer.Ordinal);
                    foreach (var conf in bpmnConfs)
                    {
                        BpmnConfConfigJson? confConfig = JsonConfUtil.ParseConfConfig(conf.ConfConfigJson);
                        if (confConfig?.NoticeChannelTypes != null && confConfig.NoticeChannelTypes.Count > 0)
                        {
                            formCode2NoticeTypes[conf.FormCode] = confConfig.NoticeChannelTypes;
                        }
                    }

                    foreach (var lfDto in results)
                    {
                        string formCode = lfDto.Key;

                        if (formCode2Flags.TryGetValue(formCode, out var flags))
                        {
                            var hasStartUserChooseModules = BpmnConfFlagsEnum.HasFlag(flags, BpmnConfFlagsEnum.HAS_STARTUSER_CHOOSE_MODULES);
                            lfDto.HasStarUserChooseModule = hasStartUserChooseModules;
                        }

                        // 构建流程通知渠道列表(遍历所有渠道,active 标记是否启用)
                        if (formCode2NoticeTypes.TryGetValue(formCode, out List<int> noticeChannelTypes) && noticeChannelTypes.Any())
                        {
                            List<BaseNumIdStruVo> processNotices = new List<BaseNumIdStruVo>();
                            foreach (var noticeEnum in ProcessNoticeEnum.Values)
                            {
                                processNotices.Add(new BaseNumIdStruVo
                                {
                                    Id = noticeEnum.Code,
                                    Name = noticeEnum.Desc,
                                    Active = noticeChannelTypes.Contains(noticeEnum.Code)
                                });
                            }
                            lfDto.ProcessNotices = processNotices;
                        }

                        // 填充通知模板配置列表
                        IBpmnConfBizService bpmnConfBizService = ServiceProviderUtils.GetService<IBpmnConfBizService>();
                        BpmnConfVo confVo = new BpmnConfVo { FormCode = formCode };
                        bpmnConfBizService.SetBpmnTemplateVos(confVo);
                        lfDto.TemplateVos = confVo.TemplateVos;
                    }
                }
            }

            page.Records = results;
            return PageUtils.GetResultAndPage(page);
        }

}
