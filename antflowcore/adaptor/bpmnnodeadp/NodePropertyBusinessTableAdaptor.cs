using antflowcore.constant.enus;
using AntFlowCore.Entity;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor;

public class NodePropertyBusinessTableAdaptor : AbstractAdditionSignNodeAdaptor
    {
        private readonly BpmnNodeBusinessTableConfService _bpmnNodeBusinessTableConfService;
        private readonly ILogger<NodePropertyBusinessTableAdaptor> _logger;

        public NodePropertyBusinessTableAdaptor(
            BpmnNodeBusinessTableConfService bpmnNodeBusinessTableConfService,
            BpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
            IRoleService roleService,
            ILogger<NodePropertyBusinessTableAdaptor> logger) : base(bpmnNodeAdditionalSignConfService, roleService)
        {
            _bpmnNodeBusinessTableConfService = bpmnNodeBusinessTableConfService;
            _logger = logger;
        }

        public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
        {
            base.FormatToBpmnNodeVo(bpmnNodeVo);
            var bpmnNodeBusinessTableConf = _bpmnNodeBusinessTableConfService.baseRepo.Where(conf => conf.BpmnNodeId == bpmnNodeVo.Id).First();

            if (bpmnNodeBusinessTableConf != null)
            {
                AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p =>
                {
                    p.ConfigurationTableType = bpmnNodeBusinessTableConf.ConfigurationTableType;
                    p.TableFieldType = bpmnNodeBusinessTableConf.TableFieldType;
                    p.SignType = bpmnNodeBusinessTableConf.SignType;
                });
            }
            
        }

       
        public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
        {
            base.EditBpmnNode(bpmnNodeVo);
            var bpmnNodePropertysVo = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

            var bpmnNodeBusinessTableConf = new BpmnNodeBusinessTableConf
            {
                BpmnNodeId = bpmnNodeVo.Id,
                ConfigurationTableType = bpmnNodePropertysVo.ConfigurationTableType,
                TableFieldType = bpmnNodePropertysVo.TableFieldType,
                SignType = bpmnNodePropertysVo.SignType,
                CreateTime = DateTime.Now,
                CreateUser = SecurityUtils.GetLogInEmpName(),
                UpdateTime = DateTime.Now,
                UpdateUser = SecurityUtils.GetLogInEmpName(),
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };

            _bpmnNodeBusinessTableConfService.baseRepo.Insert(bpmnNodeBusinessTableConf);
        }

        public override void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_BUSINESSTABLE);
        }
    }