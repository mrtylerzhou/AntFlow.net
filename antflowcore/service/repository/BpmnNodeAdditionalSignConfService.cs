using antflowcore.entity;
using antflowcore.service.interf.repository;

namespace antflowcore.service.repository;

public class BpmnNodeAdditionalSignConfService  : AFBaseCurdRepositoryService<BpmnNodeAdditionalSignConf>,IBpmnNodeAdditionalSignConfService
{
    public BpmnNodeAdditionalSignConfService(IFreeSql freeSql) : base(freeSql)
    {
    }
}