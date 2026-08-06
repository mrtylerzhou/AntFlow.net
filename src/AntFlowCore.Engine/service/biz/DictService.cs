using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Engine.service.biz;

public class DictService : IDictService
{
    /** page-added DIY FormCode 的 dict_type */
    public const string DIY_LOW_CODE_DICT_TYPE = "diylowcodeflow";
    private readonly IDicDataSerivce _dicDataSerivce;

    public DictService(
        IDicDataSerivce dicDataSerivce)
    {
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
            List<DictData> dictDatas = _dicDataSerivce._repository.Find(a=>a.Value==dto.Key);
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
                _dicDataSerivce._repository.Add(entity);
            }

            return 0;
        }

        /// <summary>
        /// 新增 page-added DIY FormCode(dict_type=diylowcodeflow)。
        /// 该类流程后端走 LowFlowApprovalService(IsLowCodeFlow=1),前端渲染自定义 Vue 组件(bizFormMaps)。
        /// </summary>
        public int AddDIYFormCode(BaseKeyValueStruVo dto)
        {
            // formCode 全局唯一(路由键),按 Value 校验,与 LF/coded DIY 不冲突
            List<DictData> dictDatas = _dicDataSerivce._repository.Find(a => a.Value == dto.Key);
            if (!dictDatas.Any())
            {
                var entity = new DictData()
                {
                    DictType = DIY_LOW_CODE_DICT_TYPE,
                    Value = dto.Key,
                    Label = dto.Value,
                    Remark = dto.Remark,
                    IsDefault = "N",
                    IsDel = 0,
                    CreateUser = SecurityUtils.GetLogInEmpName(),
                    CreateTime = DateTime.UtcNow
                };
                _dicDataSerivce._repository.Add(entity);
            }
            return 0;
        }

        /// <summary>
        /// page-added DIY(有效版本): 供"流程中心-可用流程(DIY)"合并展示。
        /// 返回 dict_type=diylowcodeflow 且有有效 BpmnConf(IsLowCodeFlow=1 + EffectiveStatus=1) 的 FormCode。
        /// </summary>
        public List<DIYProcessInfoDTO> GetDIYActiveFormCodes()
        {
            List<DictData> dictDatas = _dicDataSerivce._repository.Find(a => a.DictType == DIY_LOW_CODE_DICT_TYPE);
            if (dictDatas == null || !dictDatas.Any())
            {
                return new List<DIYProcessInfoDTO>();
            }
            var formCodes = dictDatas.Select(d => d.Value).ToList();
            IBpmnConfService bpmnConfService = ServiceProviderUtils.GetService<IBpmnConfService>();
            var effectiveFormCodes = bpmnConfService._repository
                .Find(b => formCodes.Contains(b.FormCode) && b.IsLowCodeFlow == 1 && b.EffectiveStatus == 1)
                .Select(b => b.FormCode)
                .ToHashSet(StringComparer.Ordinal);
            var results = new List<DIYProcessInfoDTO>();
            foreach (var item in dictDatas)
            {
                if (effectiveFormCodes.Contains(item.Value))
                {
                    results.Add(new DIYProcessInfoDTO
                    {
                        Key = item.Value,
                        Value = item.Label,
                        Type = "DIY",
                        Remark = item.Remark,
                        CreateTime = item.CreateTime ?? DateTime.Now
                    });
                }
            }
            return results;
        }

        private List<DictData> GetDictItemsByType(String dictType){
            List<DictData> dictDatas = _dicDataSerivce
                ._repository
                .Find(a=>a.DictType==dictType);

            return dictDatas;
        }



}
