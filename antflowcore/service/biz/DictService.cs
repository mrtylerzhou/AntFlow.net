using antflowcore.entity;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;

namespace antflowcore.service.biz;

public class DictService
{
    private readonly DicMainService _dictMainService;
    private readonly DicDataSerivce _dicDataSerivce;

    public DictService(DicMainService dictMainService, DicDataSerivce dicDataSerivce)
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
        List<DictData> dictDatas = _dicDataSerivce.baseRepo.Where(a => a.Value == dto.Key).ToList();
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

    private List<DictData> GetDictItemsByType(string dictType)
    {
        List<DictData> dictDatas = _dicDataSerivce.baseRepo.Where(a => a.DictType == dictType).OrderByDescending(a => a.CreateTime).ToList();

        return dictDatas;
    }
}