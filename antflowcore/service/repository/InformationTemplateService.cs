using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class InformationTemplateService: AFBaseCurdRepositoryService<InformationTemplate>
{
    public InformationTemplateService(IFreeSql freeSql) : base(freeSql)
    {
    }
}