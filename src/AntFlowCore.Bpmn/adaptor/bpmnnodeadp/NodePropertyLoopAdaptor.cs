using AntFlowCore.Abstraction;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.util;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Engine.Engine.service;

using AntFlowCore.Entity;
using AntFlowCore.Extensions.Extensions.adaptor;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

public class NodePropertyLoopAdaptor : AbstractAdditionSignNodeAdaptor
    {
        private readonly IBpmnNodeLoopConfService _bpmnNodeLoopConfService;
        private readonly IBpmnEmployeeInfoProviderService _bpmnEmployeeInfoProviderService;
        private readonly ILogger<NodePropertyLoopAdaptor> _logger;

        public NodePropertyLoopAdaptor(
            IBpmnNodeLoopConfService bpmnNodeLoopConfService,
            IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService,
            IBpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
            IRoleService roleService,
            ILogger<NodePropertyLoopAdaptor> logger) : base(bpmnNodeAdditionalSignConfService, roleService)
        {
            _bpmnNodeLoopConfService = bpmnNodeLoopConfService;
            _bpmnEmployeeInfoProviderService = bpmnEmployeeInfoProviderService;
            _logger = logger;
        }

        public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
        {
            base.FormatToBpmnNodeVo(bpmnNodeVo);
            BpmnNodeLoopConf bpmnNodeLoopConf = _bpmnNodeLoopConfService
                .baseRepo.Where(conf => conf.BpmnNodeId == bpmnNodeVo.Id)
                .First();

            if (bpmnNodeLoopConf != null)
            {
                List<string> loopEndPersonIds = !string.IsNullOrEmpty(bpmnNodeLoopConf.LoopEndPerson)
                    ? bpmnNodeLoopConf.LoopEndPerson.Split(',').ToList()
                    : new List<string>();

                List<string> noParticipatingStaffIds = !string.IsNullOrEmpty(bpmnNodeLoopConf.NoparticipatingStaffIds)
                    ? bpmnNodeLoopConf.NoparticipatingStaffIds.Split(',').ToList()
                    : new List<string>();

                var loopEndPersonList = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(loopEndPersonIds)
                    .Select(entry => new BaseIdTranStruVo
                    {
                        Id = entry.Key,
                        Name = entry.Value
                    }).ToList();

                var noParticipatingStaffList = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(noParticipatingStaffIds)
                    .Select(entry => new BaseIdTranStruVo
                    {
                        Id = entry.Key,
                        Name = entry.Value
                    }).ToList();

                AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p =>
                {
                    p.LoopEndType = bpmnNodeLoopConf.LoopEndType;
                    p.LoopNumberPlies = bpmnNodeLoopConf.LoopNumberPlies;
                    p.LoopEndGrade = bpmnNodeLoopConf.LoopEndGrade;
                    p.LoopEndPersonList = loopEndPersonIds;
                    p.LoopEndPersonObjList = loopEndPersonList;
                    p.NoparticipatingStaffIds = noParticipatingStaffIds;
                    p.NoparticipatingStaffs = noParticipatingStaffList;
                    
                });
                bpmnNodeVo.OrderedNodeType = (int)OrderNodeTypeEnum.LOOP_NODE;
            }
            
        }


        public override void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
        {
            base.EditBpmnNode(bpmnNodeVo);
            var property = bpmnNodeVo.Property ?? new BpmnNodePropertysVo();

            var bpmnNodeLoopConf = new BpmnNodeLoopConf
            {
                BpmnNodeId = bpmnNodeVo.Id,
                LoopEndType = property.LoopEndType,
                LoopNumberPlies = property.LoopNumberPlies,
                LoopEndGrade = property.LoopEndGrade,
                LoopEndPerson = property.LoopEndPersonList != null
                    ? string.Join(",", property.LoopEndPersonList)
                    : null,
                NoparticipatingStaffIds = property.NoparticipatingStaffIds != null
                    ? string.Join(",", property.NoparticipatingStaffIds)
                    : null,
                CreateTime = DateTime.Now,
                CreateUser = SecurityUtils.GetLogInEmpName(),
                UpdateTime = DateTime.Now,
                UpdateUser = SecurityUtils.GetLogInEmpName(),
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };

            _bpmnNodeLoopConfService.baseRepo.Insert(bpmnNodeLoopConf);
        }

        public override void SetSupportBusinessObjects()
        {
            ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_LOOP);
        }
    }