using antflowcore.constant.enus;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor;

 public class NodePropertyRoleAdaptor : BpmnNodeAdaptor
    {
        private readonly BpmnNodeRoleConfService _bpmnNodeRoleConfService;
        private readonly BpmnNodeRoleOutsideEmpConfService _bpmnNodeRoleOutsideEmpConfService;
        private readonly ILogger<NodePropertyRoleAdaptor> _logger;

        public NodePropertyRoleAdaptor(
            BpmnNodeRoleConfService bpmnNodeRoleConfService,
          
            BpmnNodeRoleOutsideEmpConfService bpmnNodeRoleOutsideEmpConfService,
            ILogger<NodePropertyRoleAdaptor> logger)
        {
            _bpmnNodeRoleConfService = bpmnNodeRoleConfService;
            _bpmnNodeRoleOutsideEmpConfService = bpmnNodeRoleOutsideEmpConfService;
            _logger = logger;
        }

        public override BpmnNodeVo FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
        {
            var list = _bpmnNodeRoleConfService.baseRepo.Where(conf => conf.BpmnNodeId == bpmnNodeVo.Id).ToList();

            if (list == null || !list.Any()) return bpmnNodeVo;

            var roles = list.Select(conf => new BaseIdTranStruVo
            {
                Id = conf.RoleId,
                Name = conf.RoleName
            }).ToList();

            bpmnNodeVo.Property = new BpmnNodePropertysVo
            {
                RoleIds = roles.Select(r => r.Id).ToList(),
                RoleList = roles,
                SignType = list.First().SignType
            };

            if (bpmnNodeVo.IsOutSideProcess == 1)
            {
                var outsideEmpConfs = _bpmnNodeRoleOutsideEmpConfService.baseRepo.Where(conf => conf.NodeId == bpmnNodeVo.Id).ToList();
                if (outsideEmpConfs != null && outsideEmpConfs.Any())
                {
                    bpmnNodeVo.Property.EmplIds = outsideEmpConfs.Select(e => e.EmplId).ToList();
                    bpmnNodeVo.Property.EmplList = outsideEmpConfs.Select(e => new BaseIdTranStruVo
                    {
                        Id = e.EmplId,
                        Name = e.EmplName
                    }).ToList();
                }
            }

            return bpmnNodeVo;
        }
        

        public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
        {
            var property = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

            if (property.RoleList != null && property.RoleList.Any())
            {
                var roleList = property.RoleList;

                _bpmnNodeRoleConfService.baseRepo.Insert(roleList.Select(role => new BpmnNodeRoleConf
                {
                    BpmnNodeId = bpmnNodeVo.Id,
                    RoleId = role.Id,
                    RoleName = role.Name,
                    SignType = property.SignType,
                    CreateTime = DateTime.Now,
                    CreateUser = SecurityUtils.GetLogInEmpName(),
                    UpdateTime = DateTime.Now,
                    UpdateUser = SecurityUtils.GetLogInEmpName(),
                    TenantId = MultiTenantUtil.GetCurrentTenantId(),
                }).ToList());

                if (bpmnNodeVo.IsOutSideProcess == 1)
                {
                    var emplList = property.EmplList;
                    if (emplList != null && emplList.Any())
                    {
                        var outsideEmpConfs = emplList.Select(empl => new BpmnNodeRoleOutsideEmpConf
                        {
                            NodeId = bpmnNodeVo.Id,
                            EmplId = empl.Id,
                            EmplName = empl.Name,
                            CreateUser = SecurityUtils.GetLogInEmpName(),
                            UpdateUser = SecurityUtils.GetLogInEmpName(),
                            TenantId = MultiTenantUtil.GetCurrentTenantId(),
                        }).ToList();

                        _bpmnNodeRoleOutsideEmpConfService.baseRepo.Insert(outsideEmpConfs);
                    }
                }
            }
        }

        public override void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_ROLE);
        }
    }