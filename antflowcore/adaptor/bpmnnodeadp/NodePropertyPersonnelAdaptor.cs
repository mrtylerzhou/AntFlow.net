using antflowcore.constant.enus;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;

namespace antflowcore.adaptor;

public class NodePropertyPersonnelAdaptor : BpmnNodeAdaptor
{
    private readonly BpmnNodePersonnelConfService _bpmnNodePersonnelConfService;
    private readonly BpmnNodePersonnelEmplConfService _bpmnNodePersonnelEmplConfService;
    private readonly IBpmnEmployeeInfoProviderService _bpmnEmployeeInfoProviderService;

    public NodePropertyPersonnelAdaptor(
        BpmnNodePersonnelConfService bpmnNodePersonnelConfService,
        BpmnNodePersonnelEmplConfService bpmnNodePersonnelEmplConfService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService
        )
    {
        _bpmnNodePersonnelConfService = bpmnNodePersonnelConfService;
        _bpmnNodePersonnelEmplConfService = bpmnNodePersonnelEmplConfService;
        _bpmnEmployeeInfoProviderService = bpmnEmployeeInfoProviderService;
    }
    public override BpmnNodeVo FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        BpmnNodePersonnelConf bpmnNodePersonnelConf = _bpmnNodePersonnelConfService.baseRepo.Where(a => a.BpmnNodeId == bpmnNodeVo.Id).First();
        if (bpmnNodePersonnelConf == null)
        {
            throw new AFBizException($"未能根据节点id: {bpmnNodeVo.Id}查到指定人员信息!");
        }
        List<String> emplIds = new List<string>();
        List<String> emplNames=new List<string>();
        IEnumerable<BpmnNodePersonnelEmplConf> bpmnNodePersons = _bpmnNodePersonnelEmplConfService
            .baseRepo
            .Where(a => a.BpmnNodePersonneId == bpmnNodePersonnelConf.Id)
            .ToList().Distinct();
        if(ObjectUtils.IsEmpty(bpmnNodePersons)){
            throw  new AFBizException("配置错误或者数据被删除,指定员人审批未获取到人员");
        }
        foreach (var bpmnNodePersonnelEmplConf in bpmnNodePersons)
        {
            String emplId = bpmnNodePersonnelEmplConf.EmplId;
            String emplName = bpmnNodePersonnelEmplConf.EmplName;
            emplIds.Add(emplId);
            if(!String.IsNullOrEmpty(emplName)){
                emplNames.Add(emplName);
            }
        }

        bpmnNodeVo.Property = new BpmnNodePropertysVo
        {
            SignType = bpmnNodePersonnelConf.SignType,
            EmplIds = emplIds,
            EmplList = GetEmplList(emplIds, emplNames)
        };
     return bpmnNodeVo;
    }

    public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        var bpmnNodePropertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

        var bpmnNodePersonnelConf = new BpmnNodePersonnelConf
        {
            BpmnNodeId = (int)bpmnNodeVo.Id,
            SignType = bpmnNodePropertysVo.SignType??0,
            CreateTime = DateTime.Now,
            CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
            UpdateTime = DateTime.Now,
            UpdateUser = SecurityUtils.GetLogInEmpNameSafe()
        };

        _bpmnNodePersonnelConfService.baseRepo.Insert(bpmnNodePersonnelConf);
        int nodePersonnelId = bpmnNodePersonnelConf.Id;

        if (bpmnNodePropertysVo.EmplIds == null || !bpmnNodePropertysVo.EmplIds.Any())
        {
            return;
        }

        var personnelEmplConfs = new List<BpmnNodePersonnelEmplConf>();
        var emplList = bpmnNodePropertysVo.EmplList ?? new List<BaseIdTranStruVo>();
        var id2nameMap = emplList.ToDictionary(x => x.Id, x => x.Name, StringComparer.Ordinal);

        foreach (var emplId in bpmnNodePropertysVo.EmplIds)
        {
            var personnelEmplConf = new BpmnNodePersonnelEmplConf
            {
                BpmnNodePersonneId = nodePersonnelId,
                EmplId = emplId,
                CreateTime = DateTime.Now,
                CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
                UpdateTime = DateTime.Now,
                UpdateUser = SecurityUtils.GetLogInEmpNameSafe(),
                EmplName = id2nameMap.ContainsKey(emplId) ? id2nameMap[emplId] : null
            };

            personnelEmplConfs.Add(personnelEmplConf);
        }

        _bpmnNodePersonnelEmplConfService.baseRepo.Insert(personnelEmplConfs);
    }

    /// <summary>
    /// Get employee list.
    /// If emplNames is not empty, it is stored in the database and then loaded.
    /// </summary>
    /// <param name="emplIds">List of employee IDs.</param>
    /// <param name="emplNames">List of employee names.</param>
    /// <returns>List of BaseIdTranStruVo objects.</returns>
    private List<BaseIdTranStruVo> GetEmplList(List<string> emplIds, List<string> emplNames)
    {
        var result = new List<BaseIdTranStruVo>();

        if (emplNames != null && emplNames.Any())
        {
            if (emplIds.Count != emplNames.Count)
            {
                throw new AFBizException("指定人员审批存在姓名不存在的人员!");
            }

            for (int i = 0; i < emplIds.Count; i++)
            {
                var vo = new BaseIdTranStruVo
                {
                    Id = emplIds[i],
                    Name = emplNames[i]
                };
                result.Add(vo);
            }

            return result;
        }

        var employeeInfos = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(emplIds);

        foreach (var emplId in emplIds)
        {
            var vo = new BaseIdTranStruVo
            {
                Id = emplId,
                Name = employeeInfos.TryGetValue(emplId, out var empName) ? empName : null
            };
            result.Add(vo);
        }

        return result;
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(
            BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_PERSONNEL,
            BpmnNodeAdpConfEnum.ADP_CONF_NODE_TYPE_COPY);
    }
}