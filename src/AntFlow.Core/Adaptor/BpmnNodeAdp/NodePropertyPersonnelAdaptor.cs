using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor;

public class NodePropertyPersonnelAdaptor : BpmnNodeAdaptor
{
    private readonly IBpmnEmployeeInfoProviderService _bpmnEmployeeInfoProviderService;
    private readonly BpmnNodePersonnelConfService _bpmnNodePersonnelConfService;
    private readonly BpmnNodePersonnelEmplConfService _bpmnNodePersonnelEmplConfService;

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
        BpmnNodePersonnelConf bpmnNodePersonnelConf =
            _bpmnNodePersonnelConfService.baseRepo.Where(a => a.BpmnNodeId == bpmnNodeVo.Id).First();
        if (bpmnNodePersonnelConf == null)
        {
            throw new AFBizException($"未能根据节点id: {bpmnNodeVo.Id}找到指定人员信息!");
        }

        List<string> emplIds = new();
        List<string> emplNames = new();
        IEnumerable<BpmnNodePersonnelEmplConf> bpmnNodePersons = _bpmnNodePersonnelEmplConfService
            .baseRepo
            .Where(a => a.BpmnNodePersonneId == bpmnNodePersonnelConf.Id)
            .ToList().Distinct();
        if (ObjectUtils.IsEmpty(bpmnNodePersons))
        {
            throw new AFBizException("���ô���������ݱ�ɾ��,ָ��Ա������δ��ȡ����Ա");
        }

        foreach (BpmnNodePersonnelEmplConf? bpmnNodePersonnelEmplConf in bpmnNodePersons)
        {
            string emplId = bpmnNodePersonnelEmplConf.EmplId;
            string emplName = bpmnNodePersonnelEmplConf.EmplName;
            emplIds.Add(emplId);
            if (!string.IsNullOrEmpty(emplName))
            {
                emplNames.Add(emplName);
            }
        }

        bpmnNodeVo.Property = new BpmnNodePropertysVo
        {
            SignType = bpmnNodePersonnelConf.SignType, EmplIds = emplIds, EmplList = GetEmplList(emplIds, emplNames)
        };
        return bpmnNodeVo;
    }

    public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        BpmnNodePropertysVo? bpmnNodePropertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

        BpmnNodePersonnelConf? bpmnNodePersonnelConf = new()
        {
            BpmnNodeId = (int)bpmnNodeVo.Id,
            SignType = bpmnNodePropertysVo.SignType ?? 0,
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

        List<BpmnNodePersonnelEmplConf>? personnelEmplConfs = new();
        List<BaseIdTranStruVo>? emplList = bpmnNodePropertysVo.EmplList ?? new List<BaseIdTranStruVo>();
        Dictionary<string, string>? id2nameMap = emplList.ToDictionary(x => x.Id, x => x.Name, StringComparer.Ordinal);

        foreach (string? emplId in bpmnNodePropertysVo.EmplIds)
        {
            BpmnNodePersonnelEmplConf? personnelEmplConf = new()
            {
                BpmnNodePersonneId = nodePersonnelId,
                EmplId = emplId,
                CreateTime = DateTime.Now,
                CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
                UpdateTime = DateTime.Now,
                UpdateUser = SecurityUtils.GetLogInEmpNameSafe(),
                EmplName = id2nameMap.ContainsKey(emplId) ? id2nameMap[emplId] : null,
                TenantId = MultiTenantUtil.GetCurrentTenantId()
            };

            personnelEmplConfs.Add(personnelEmplConf);
        }

        _bpmnNodePersonnelEmplConfService.baseRepo.Insert(personnelEmplConfs);
    }

    /// <summary>
    ///     Get employee list.
    ///     If emplNames is not empty, it is stored in the database and then loaded.
    /// </summary>
    /// <param name="emplIds">List of employee IDs.</param>
    /// <param name="emplNames">List of employee names.</param>
    /// <returns>List of BaseIdTranStruVo objects.</returns>
    private List<BaseIdTranStruVo> GetEmplList(List<string> emplIds, List<string> emplNames)
    {
        List<BaseIdTranStruVo>? result = new();

        if (emplNames != null && emplNames.Any())
        {
            if (emplIds.Count != emplNames.Count)
            {
                throw new AFBizException("ָ����Ա�����������������ڵ���Ա!");
            }

            for (int i = 0; i < emplIds.Count; i++)
            {
                BaseIdTranStruVo? vo = new() { Id = emplIds[i], Name = emplNames[i] };
                result.Add(vo);
            }

            return result;
        }

        Dictionary<string, string>? employeeInfos = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(emplIds);

        foreach (string? emplId in emplIds)
        {
            BaseIdTranStruVo? vo = new()
            {
                Id = emplId, Name = employeeInfos.TryGetValue(emplId, out string? empName) ? empName : null
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