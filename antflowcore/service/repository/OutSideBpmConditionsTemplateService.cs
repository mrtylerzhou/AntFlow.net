using AntFlowCore.Entity;

namespace antflowcore.service.repository;

public class OutSideBpmConditionsTemplateService: AFBaseCurdRepositoryService<OutSideBpmConditionsTemplate>
{
    public OutSideBpmConditionsTemplateService(IFreeSql freeSql) : base(freeSql)
    {
    }
}