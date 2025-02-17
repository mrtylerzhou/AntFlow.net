using antflowcore.constant.enus;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor;

public class NodePropertyBusinessTableAdaptor : BpmnNodeAdaptor
    {
        private readonly BpmnNodeBusinessTableConfService _bpmnNodeBusinessTableConfService;
        private readonly ILogger<NodePropertyBusinessTableAdaptor> _logger;

        public NodePropertyBusinessTableAdaptor(
            BpmnNodeBusinessTableConfService bpmnNodeBusinessTableConfService,
            ILogger<NodePropertyBusinessTableAdaptor> logger)
        {
            _bpmnNodeBusinessTableConfService = bpmnNodeBusinessTableConfService;
            _logger = logger;
        }

        public override BpmnNodeVo FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
        {
            var bpmnNodeBusinessTableConf = _bpmnNodeBusinessTableConfService.baseRepo.Where(conf => conf.BpmnNodeId == bpmnNodeVo.Id).First();

            if (bpmnNodeBusinessTableConf != null)
            {
                bpmnNodeVo.Property = new BpmnNodePropertysVo
                {
                    ConfigurationTableType = bpmnNodeBusinessTableConf.ConfigurationTableType,
                    TableFieldType = bpmnNodeBusinessTableConf.TableFieldType,
                    SignType = bpmnNodeBusinessTableConf.SignType
                };
            }

            return bpmnNodeVo;
        }

       
        public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
        {
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
                UpdateUser = SecurityUtils.GetLogInEmpName()
            };

            _bpmnNodeBusinessTableConfService.baseRepo.Insert(bpmnNodeBusinessTableConf);
        }

        public override void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_BUSINESSTABLE);
        }
    }