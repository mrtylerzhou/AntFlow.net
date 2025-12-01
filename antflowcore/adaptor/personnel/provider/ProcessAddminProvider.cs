using antflowcore.vo;

namespace antflowcore.adaptor.personnel.provider;

public class ProcessAddminProvider: IBpmnProcessAdminProvider
{
    public BaseIdTranStruVo ProvideProcessAdminInfo()
    {
        return new BaseIdTranStruVo
        {
            Id = "20",
            Name = "任盈盈",
        };
    }
}