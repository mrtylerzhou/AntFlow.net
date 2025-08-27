using AntFlow.Core.Vo;

namespace AntFlow.Core.Dto;

public class DIYProcessInfoDTO
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string Type { get; set; }
    public string Remark { get; set; }
    public DateTime CreateTime { get; set; }

    /**
     * ??????????????????,?????????,true?????
     */
    public bool HasStarUserChooseModule { get; set; } = false;

    public List<BaseNumIdStruVo> ProcessNotices { get; set; }
}