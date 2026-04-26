using System.Linq.Expressions;
using AntFlowCore.Abstraction.Orm.ext;
using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.biz;
using AntFlowCore.Persist.api.interf.repository;
using FreeSql.Internal.Model;

namespace AntFlowCore.Business.service.biz;

public class DicDataBizService :  IDicDataBizSerivce
{
    private readonly IDicDataSerivce _dicDataSerivce;
    private readonly IBpmProcessNoticeService _bpmProcessNoticeService;


    public DicDataBizService(IFreeSql freeSql, IDicDataSerivce dicDataSerivce, IBpmProcessNoticeService bpmProcessNoticeService)
    {
        _dicDataSerivce = dicDataSerivce;
        _bpmProcessNoticeService = bpmProcessNoticeService;
    }

    public ResultAndPage<BaseKeyValueStruVo> SelectLFActiveFormCodePageList(PageDto pageDto, TaskMgmtVO taskMgmtVO)
    {
        Page<BaseKeyValueStruVo> page = PageUtils.GetPageByPageDto<BaseKeyValueStruVo>(pageDto);
        List<DictData> dictDataList = this.SelectLFActiveFormCodePageList(page, taskMgmtVO);
        return HandleLFFormCodePageList(page, dictDataList);
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
            return HandleLFFormCodePageList(page, dictDataList);
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
        private ResultAndPage<BaseKeyValueStruVo> HandleLFFormCodePageList(Page<BaseKeyValueStruVo> page, List<DictData> dictlist)
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
                    Type = "LF",
                    Remark = item.Remark
                });
            }

            var formCodes = results.Select(r => r.Key).ToList();

           
            if (formCodes.Any())
            {
                IBpmnConfService bpmnConfService = ServiceProviderUtils.GetService<IBpmnConfService>();

                List<BpmnConf> bpmnConfs = bpmnConfService
                    .baseRepo
                    .Where(a => formCodes.Contains(a.FormCode) && a.EffectiveStatus == 1)
                    .ToList();
                if (bpmnConfs != null && bpmnConfs.Any())
                {
                    var formCode2Flags = bpmnConfs.ToDictionary(
                        b => b.FormCode,
                        b => b.ExtraFlags
                    );
                    IDictionary<string,List<BpmProcessNotice>> processNoticeMap= _bpmProcessNoticeService.ProcessNoticeMap(formCodes);
                    foreach (var lfDto in results)
                    {
                        if (formCode2Flags.TryGetValue(lfDto.Key, out var flags))
                        {
                            var hasStartUserChooseModules = BpmnConfFlagsEnum.HasFlag(flags, BpmnConfFlagsEnum.HAS_STARTUSER_CHOOSE_MODULES);
                            lfDto.HasStarUserChooseModule = hasStartUserChooseModules;
                        }
                        String formCode = lfDto.Key;
                        if (processNoticeMap.TryGetValue(formCode, out var bpmProcessNotices) && bpmProcessNotices.Any())
                        {
                            var processNotices = new List<BaseNumIdStruVo>();
                            foreach (ProcessNoticeEnum processNoticeEnum in ProcessNoticeEnum.Values)
                            {
                                var type = processNoticeEnum.Code;
                                var descByCode = processNoticeEnum.Desc;

                                var struVo = new BaseNumIdStruVo
                                {
                                    Id = type,
                                    Name = descByCode,
                                    Active = bpmProcessNotices.Any(n => n.Type == type)
                                };

                                processNotices.Add(struVo);
                            }
                           
                            lfDto.ProcessNotices = processNotices;
                        }

                    }
                }
            }

            page.Records = results;
            return PageUtils.GetResultAndPage(page);
        }
   
}