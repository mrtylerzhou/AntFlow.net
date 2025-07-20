using antflowcore.vo;

namespace antflowcore.dto;

public class DIYProcessInfoDTO
{
    public String Key { get; set; }
    public String Value { get; set; }
    public String Type { get; set; }
    public String Remark { get; set; }
    public DateTime CreateTime { get; set; }

    /**
     * 是否包含发起人自选模块,否为不包含,true为包含
     */
    public bool HasStarUserChooseModule { get; set; } = false;
    public List<BaseNumIdStruVo> ProcessNotices { get; set; }
}