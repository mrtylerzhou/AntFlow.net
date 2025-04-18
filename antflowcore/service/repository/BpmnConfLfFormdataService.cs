﻿using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.biz;

namespace antflowcore.service.repository;

public class BpmnConfLfFormdataService: AFBaseCurdRepositoryService<BpmnConfLfFormdata>
{
    public BpmnConfLfFormdataService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public List<BpmnConfLfFormdata> ListByConfId(long confId)
    {
        List<BpmnConfLfFormdata> lfFormdatas = baseRepo.Select.Where(a=>a.BpmnConfId == confId).ToList();
        return lfFormdatas;
    }
    
}