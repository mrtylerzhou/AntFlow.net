using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class OutSideBpmBusinessPartyService: AFBaseCurdRepositoryService<OutSideBpmBusinessParty>
{
    public OutSideBpmBusinessPartyService(IFreeSql freeSql) : base(freeSql)
    {
    }
}