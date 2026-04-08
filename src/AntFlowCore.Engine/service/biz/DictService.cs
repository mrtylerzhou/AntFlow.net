using System.Linq.Expressions;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.util;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.extension;
using AntFlowCore.Core.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;
using FreeSql.Internal.Model;

namespace AntFlowCore.Engine.service.biz;

public class DictService : IDictService
{
    private readonly IDicMainService _dictMainService;
    private readonly IBpmProcessNoticeService _bpmProcessNoticeService;
    private readonly IDicDataSerivce _dicDataSerivce;

    public DictService(
        IDicMainService dictMainService,
        IBpmProcessNoticeService bpmProcessNoticeService,
        IDicDataSerivce dicDataSerivce)
    {
        _dictMainService = dictMainService;
        _bpmProcessNoticeService = bpmProcessNoticeService;
        _dicDataSerivce = dicDataSerivce;
    }
    
        /// <summary>
        /// 获取全部 LF FormCodes 在流程设计时选择使用
        /// </summary>
        public List<BaseKeyValueStruVo> GetLowCodeFlowFormCodes()
        {
            var lowCodeList = GetDictItemsByType("lowcodeflow");
            var results = new List<BaseKeyValueStruVo>();

            foreach (var item in lowCodeList)
            {
                results.Add(new BaseKeyValueStruVo()
                {
                    Key = item.Value,
                    Value = item.Label,
                    Type = "LF",
                    Remark = item.Remark
                });
            }

            return results;
        }


        /// <summary>
        /// 新增LF FormCode
        /// </summary>
        public int AddFormCode(BaseKeyValueStruVo dto)
        {
            List<DictData> dictDatas = _dicDataSerivce.baseRepo.Where(a=>a.Value==dto.Key).ToList();
            if (!dictDatas.Any())
            {
                var entity = new DictData()
                {
                    DictType = "lowcodeflow",
                    Value = dto.Key,
                    Label = dto.Value,
                    Remark = dto.Remark,
                    IsDefault = "N",
                    IsDel = 0,
                    CreateUser = SecurityUtils.GetLogInEmpName(),
                    CreateTime = DateTime.UtcNow
                };
                _dicDataSerivce.baseRepo.Insert(entity);
            }
           
            return 0;
        }

    
        private List<DictData> GetDictItemsByType(String dictType){
            List<DictData> dictDatas = _dicDataSerivce
                .baseRepo
                .Where(a=>a.DictType==dictType)
                .OrderByDescending(a=>a.CreateTime).ToList();
            
            return dictDatas;
        }

        public ResultAndPage<BaseKeyValueStruVo> SelectLFFormCodePageList(PageDto pageDto, TaskMgmtVO taskMgmtVo)
        {
            Page<BaseKeyValueStruVo> page = PageUtils.GetPageByPageDto<BaseKeyValueStruVo>(pageDto);
            List<DictData> dictDataList = this.SelectLFFormCodePageList(page,taskMgmtVo);
            return HandleLFFormCodePageList(page, dictDataList);
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

            BasePagingInfo basePagingInfo = page.ToPagingInfo();
            List<DictData> dictDataList = _dicDataSerivce
                .Frsql
                .Select<DictData, BpmnConf>()
                .InnerJoin((a, b) => a.Value == b.FormCode && b.IsLowCodeFlow == 1)
                .Where(expression)
                .OrderByDescending((a, b) => a.CreateTime)
                .Page(basePagingInfo)
                .ToList<DictData>((a, b) => a);
            page.Total = (int)basePagingInfo.Count;
            return dictDataList;
        }

        List<DictData> SelectLFFormCodePageList(Page<BaseKeyValueStruVo> page, TaskMgmtVO taskMgmtVO)
        {
            Expression<Func<DictData,bool>> expression = a => a.DictType == "lowcodeflow";

            if (!string.IsNullOrEmpty(taskMgmtVO.Description))
            {
               expression= LinqExtensions.And(expression, a =>
                    a.Label.Contains(taskMgmtVO.Description) || a.Value.Contains(taskMgmtVO.Description));
            }

            BasePagingInfo basePagingInfo = page.ToPagingInfo();
            List<DictData> dictDatas = this._dicDataSerivce
                .baseRepo
                .Where(expression)
                .OrderByDescending(c=>c.CreateTime)
                .Page(basePagingInfo)
                .ToList();
            page.Total = (int)basePagingInfo.Count;
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