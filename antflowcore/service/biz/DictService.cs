using System.Linq.Expressions;
using antflowcore.constant.enus;
using antflowcore.dto;
using antflowcore.entity;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class DictService
{
    private readonly DicMainService _dictMainService;
    private readonly DicDataSerivce _dicDataSerivce;

    public DictService(DicMainService dictMainService,DicDataSerivce dicDataSerivce)
    {
        _dictMainService = dictMainService;
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
        List<DictData> SelectLFActiveFormCodePageList(Page<TaskMgmtVO> page, TaskMgmtVO taskMgmtVO)
        {

            Expression<Func<DictData, BpmnConf,bool>> expression = (a, b) => a.DictType == "lowcodeflow";
            if (taskMgmtVO.ProcessState != null && taskMgmtVO.ProcessState > 0)
            {
                expression = expression.And((a,b)=>b.EffectiveStatus==taskMgmtVO.ProcessState);
            }

            if (!string.IsNullOrEmpty(taskMgmtVO.Description))
            {
                expression.And((a, b) =>
                    a.DictType.Contains(taskMgmtVO.Description) || a.Value.Contains(taskMgmtVO.Description));
            }

            List<DictData> dictDataList = _dicDataSerivce
                .Frsql
                .Select<DictData, BpmnConf>()
                .InnerJoin((a, b) => a.Value == b.FormCode && b.IsLowCodeFlow == 1)
                .Where(expression)
                .OrderByDescending((a, b) => a.CreateTime)
                .Page(page.Current,page.Size)
                .ToList<DictData>((a, b) => a);
            return dictDataList;
        }

        List<DictData> SelectLFFormCodePageList(Page<BaseKeyValueStruVo> page, TaskMgmtVO taskMgmtVO)
        {
            Expression<Func<DictData,bool>> expression = a => a.DictType == "lowcodeflow";

            if (!string.IsNullOrEmpty(taskMgmtVO.Description))
            {
                expression.And(a =>
                    a.DictType.Contains(taskMgmtVO.Description) || a.Value.Contains(taskMgmtVO.Description));
            }

            List<DictData> dictDatas = this._dicDataSerivce
                .baseRepo
                .Where(expression)
                .Page(page.Current,page.Size)
                .ToList();
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
                BpmnConfService bpmnConfService = ServiceProviderUtils.GetService<BpmnConfService>();

                List<BpmnConf> bpmnConfs = bpmnConfService
                    .baseRepo
                    .Where(a => formCodes.Contains(a.FormCode) && a.EffectiveStatus == 1 && a.ExtraFlags != null)
                    .ToList();
                if (bpmnConfs != null && bpmnConfs.Any())
                {
                    var formCode2Flags = bpmnConfs.ToDictionary(
                        b => b.FormCode,
                        b => b.ExtraFlags
                    );

                    foreach (var lfDto in results)
                    {
                        if (formCode2Flags.TryGetValue(lfDto.Key, out var flags))
                        {
                            var hasStartUserChooseModules = BpmnConfFlagsEnum.HasFlag(flags, BpmnConfFlagsEnum.HAS_STARTUSER_CHOOSE_MODULES);
                            lfDto.HasStarUserChooseModule = hasStartUserChooseModules;
                        }
                    }
                }
            }

            page.Records = results;
            return PageUtils.GetResultAndPage(page);
        }

}