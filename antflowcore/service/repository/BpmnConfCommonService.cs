using antflowcore.constant.enus;
using antflowcore.entity;
using antflowcore.factory;
using antflowcore.service.biz;
using antflowcore.service.processor;
using antflowcore.service.processor.filter;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.repository;

public class BpmnConfCommonService
{
    private readonly BpmnConfBizService _bpmnConfBizService;
    private readonly BpmnStartFormatFactory _bpmnStartFormatFactory;
    private readonly IBpmnPersonnelFormat _personnelFormat;
    private readonly IBpmnDeduplicationFormat _deduplicationFormat;
    private readonly IBpmnOptionalDuplicatesAdaptor _optionalDuplicatesAdaptor;
    private readonly BpmnRemoveConfFormatFactory _bpmnRemoveConfFormatFactory;
    private readonly BpmnNodeFormatService _bpmnNodeFormatService;
    private readonly BpmnInsertVariablesService _bpmnInsertVariablesService;
    private readonly BpmnCreateAndStartService _bpmnCreateAndStartService;
    private readonly IFreeSql _freeSql;
    private readonly ILogger<BpmnConfCommonService> _logger;

    public BpmnConfCommonService(
        BpmnConfBizService bpmnConfBizService,
        BpmnStartFormatFactory bpmnStartFormatFactory,
        IBpmnPersonnelFormat personnelFormat,
        IBpmnDeduplicationFormat deduplicationFormat,
        IBpmnOptionalDuplicatesAdaptor optionalDuplicatesAdaptor,
        BpmnRemoveConfFormatFactory bpmnRemoveConfFormatFactory,
        BpmnNodeFormatService bpmnNodeFormatService,
        BpmnInsertVariablesService bpmnInsertVariablesService,
        BpmnCreateAndStartService bpmnCreateAndStartService,
        IFreeSql freeSql,
        ILogger<BpmnConfCommonService> logger)
    {
        _bpmnConfBizService = bpmnConfBizService;
        _bpmnStartFormatFactory = bpmnStartFormatFactory;
        _personnelFormat = personnelFormat;
        _deduplicationFormat = deduplicationFormat;
        _optionalDuplicatesAdaptor = optionalDuplicatesAdaptor;
        _bpmnRemoveConfFormatFactory = bpmnRemoveConfFormatFactory;
        _bpmnNodeFormatService = bpmnNodeFormatService;
        _bpmnInsertVariablesService = bpmnInsertVariablesService;
        _bpmnCreateAndStartService = bpmnCreateAndStartService;
        _freeSql = freeSql;
        _logger = logger;
    }
    public BpmnConf GetBpmnConfByFormCode(String formCode)
    {
        BpmnConf bpmnConf = _freeSql
            .Select<BpmnConf>()
            .Where(a => a.FormCode == formCode && a.EffectiveStatus == 1)
            .ToOne()??new BpmnConf();
        return bpmnConf;
    }

    public void StartProcess(String bpmnCode, BpmnStartConditionsVo bpmnStartConditions)
    {
        //to query the process's config information
        BpmnConfVo bpmnConfVo = _bpmnConfBizService.Detail(bpmnCode);
        // format process's floating direction,set assignees,assignees deduplication and remove some nodes on conditions
        BpmnConfVo confVo = GetBpmnConfVo(bpmnStartConditions, bpmnConfVo);
            
        //to convert the process element information
        //set some basic information
        BpmnConfCommonVo bpmnConfCommonVo = new BpmnConfCommonVo
        {
            BpmnCode = confVo.BpmnCode,
            FormCode = confVo.FormCode,
            BpmnName = confVo.BpmnName,
            ProcessNum = bpmnStartConditions.ProcessNum,
            ProcessName = bpmnConfVo.BpmnName,
           ProcessDesc = bpmnStartConditions.ProcessDesc,
           TemplateVos = bpmnConfVo.TemplateVos,
        };
        //set view page's buttons information
        SetViewPageButtons(bpmnConfVo, bpmnConfCommonVo);

        List<BpmnConfCommonElementVo> bpmnConfCommonElementVoList = _bpmnNodeFormatService.GetBpmnConfCommonElementVoList(bpmnConfCommonVo, bpmnConfVo.Nodes, bpmnStartConditions);
        bpmnConfCommonVo.ElementList=bpmnConfCommonElementVoList;
        string s = "hello";
        _bpmnInsertVariablesService.InsertVariables(bpmnConfCommonVo,bpmnStartConditions);
        _bpmnCreateAndStartService.CreateBpmnAndStart(bpmnConfCommonVo,bpmnStartConditions);
    }

    private BpmnConfVo GetBpmnConfVo(BpmnStartConditionsVo bpmnStartConditions, BpmnConfVo bpmnConfVo)
    {
        //1. Format the process,filter it by condition
        _bpmnStartFormatFactory.formatBpmnConf(bpmnConfVo,bpmnStartConditions);
        
        
        //2、set consignees information and finally determine the flow's direction
        _personnelFormat.FormatPersonnelsConf(bpmnConfVo, bpmnStartConditions);
        //3. to determine whether it is necessary to deduplication
        if (bpmnConfVo.DeduplicationType!=(int)DeduplicationTypeEnum.DEDUPLICATION_TYPE_NULL) {
            
            if (bpmnConfVo.DeduplicationType==(int)DeduplicationTypeEnum.DEDUPLICATION_TYPE_FORWARD) {
                //deduplication forward
                _deduplicationFormat.ForwardDeduplication(bpmnConfVo, bpmnStartConditions);
            } else if (bpmnConfVo.DeduplicationType==(int)(DeduplicationTypeEnum.DEDUPLICATION_TYPE_BACKWARD)) {
                //deduplication backword
                _deduplicationFormat.BackwardDeduplication(bpmnConfVo, bpmnStartConditions);
            }
        }

        _optionalDuplicatesAdaptor.OptionalDuplicate(bpmnConfVo, bpmnStartConditions);
        //4、format the nodes by pipelines
        _bpmnRemoveConfFormatFactory.RemoveBpmnConf(bpmnConfVo,bpmnStartConditions);
        return bpmnConfVo;
    }
    private void SetViewPageButtons(BpmnConfVo bpmnConfVo, BpmnConfCommonVo bpmnConfCommonVo)
    {
        bpmnConfCommonVo.ViewPageButtons = new BpmnConfViewPageButtonVo
        {
            ViewPageStart = bpmnConfVo.ViewPageButtons.ViewPageStart
                .Select(o => new BpmnConfCommonButtonPropertyVo
                {
                    ButtonType = o,
                    ButtonName = ButtonTypeEnumExtensions.GetDescByCode(o)
                })
                .ToList(),

            ViewPageOther = bpmnConfVo.ViewPageButtons.ViewPageOther
                .Select(o => new BpmnConfCommonButtonPropertyVo
                {
                    ButtonType = o,
                    ButtonName = ButtonTypeEnumExtensions.GetDescByCode(o)
                })
                .ToList()
        };
    }

}