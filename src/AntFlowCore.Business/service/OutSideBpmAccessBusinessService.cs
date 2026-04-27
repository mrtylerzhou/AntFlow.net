using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.constant;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class OutSideBpmAccessBusinessService : IOutSideBpmAccessBusinessService
{
    private readonly IBpmnConfRepository _bpmnConfRepository;
    private readonly IButtonOperationService _buttonOperationService;
    private readonly IBpmVerifyInfoBizService _bpmVerifyInfoBizService;
    private readonly IBpmnEmployeeInfoProviderService _bpmnEmployeeInfoProviderService;

    public OutSideBpmAccessBusinessService(
        IOutSideBpmAccessBusinessRepository repository,
        IBpmnConfRepository bpmnConfRepository,
        IButtonOperationService buttonOperationService,
        IBpmVerifyInfoBizService bpmVerifyInfoBizService,
        IBpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService)
    {
        _repository = repository;
        _bpmnConfRepository = bpmnConfRepository;
        _buttonOperationService = buttonOperationService;
        _bpmVerifyInfoBizService = bpmVerifyInfoBizService;
        _bpmnEmployeeInfoProviderService = bpmnEmployeeInfoProviderService;
    }

    public IOutSideBpmAccessBusinessRepository _repository { get; }

    public void UpdateById(OutSideBpmAccessBusiness outSideBpmAccessBusiness)
    {
        _repository.Update(outSideBpmAccessBusiness);
    }

    public OutSideBpmAccessRespVo AccessBusinessStart(OutSideBpmAccessBusinessVo vo)
    {
        OutSideBpmAccessBusiness outSideBpmAccessBusiness = _repository
            .FirstOrDefault(a => a.Id == vo.Id);

        if (outSideBpmAccessBusiness != null)
        {
            // 更新已有记录
            vo.CopyPropertiesTo(outSideBpmAccessBusiness);
            outSideBpmAccessBusiness.UpdateUser = SecurityUtils.GetLogInEmpId();
            outSideBpmAccessBusiness.UpdateTime = DateTime.Now;
            UpdateById(outSideBpmAccessBusiness);
        }
        else
        {
            var formCode = vo.FormCode;
            BpmnConf effectiveConfByFormCode = _bpmnConfRepository
                .FirstOrDefault(a => a.FormCode == formCode && a.EffectiveStatus == 1 && a.IsDel == 0);

            if (effectiveConfByFormCode == null)
            {
                throw new AFBizException($"未能根据流程编号{formCode}找到有效的流程配置,请检查同业入参");
            }

            vo.BpmnConfId = effectiveConfByFormCode.Id;
            outSideBpmAccessBusiness = vo.MapToEntity();


            if (vo.TemplateMarks != null && vo.TemplateMarks.Any())
            {
                outSideBpmAccessBusiness.TemplateMark = string.Join(",", vo.TemplateMarks);
            }

            outSideBpmAccessBusiness.BusinessPartyId = effectiveConfByFormCode.BusinessPartyId ?? 0;
            var empId = SecurityUtils.GetLogInEmpIdSafe();
            var empName = SecurityUtils.GetLogInEmpNameSafe();
            var now = DateTime.Now;

            outSideBpmAccessBusiness.CreateUser = empId;
            outSideBpmAccessBusiness.CreateTime = now;
            outSideBpmAccessBusiness.UpdateUser = empId;
            outSideBpmAccessBusiness.UpdateTime = now;

            _repository.Add(outSideBpmAccessBusiness);
        }

        // 组装业务数据对象
        BusinessDataVo businessDataVo = vo.IsLowCodeFlow ? new UDLFApplyVo() : new BusinessDataVo();

        if (businessDataVo is UDLFApplyVo udlf)
        {
            udlf.LfFields = vo.LfFields;
        }

        businessDataVo.FormCode = vo.FormCode;
        businessDataVo.OperationType = (int)ButtonTypeEnum.BUTTON_TYPE_SUBMIT;
        businessDataVo.BusinessId = outSideBpmAccessBusiness.Id.ToString();
        businessDataVo.OutSideType = NumberConstants.BPMN_FLOW_TYPE_OUTSIDE;
        businessDataVo.ApproversList = vo.ApproversList;
        businessDataVo.IsLowCodeFlow = vo.IsLowCodeFlow ? 1 : 0;

        if (string.IsNullOrWhiteSpace(vo.UserId))
        {
            throw new AFBizException("发起人用户名为空，无法发起流程！");
        }

        businessDataVo.StartUserId = SecurityUtils.GetLogInEmpIdSafe();
        businessDataVo.StartUserName = SecurityUtils.GetLogInEmpNameSafe();
        businessDataVo.EmpId = SecurityUtils.GetLogInEmpIdSafe();
        businessDataVo.SubmitUser = SecurityUtils.GetLogInEmpNameSafe();
        businessDataVo.EmbedNodes = vo.EmbedNodes;
        _buttonOperationService.ButtonsOperationTransactional(businessDataVo);

        OutSideBpmAccessBusiness result = _repository
            .FirstOrDefault(a => a.Id == outSideBpmAccessBusiness.Id);

        return new OutSideBpmAccessRespVo
        {
            ProcessNumber = result.ProcessNumber,
            BusinessId = result.Id.ToString(),
            ProcessRecord = GetProcessRecord(result.ProcessNumber)
        };
    }

    private List<OutSideBpmAccessProcessRecordVo> GetProcessRecord(string processNumber)
    {
        var bpmVerifyInfoVos = _bpmVerifyInfoBizService.GetBpmVerifyInfoVos(processNumber, false);

        var result = bpmVerifyInfoVos
            .Select(o => new OutSideBpmAccessProcessRecordVo
            {
                NodeName = o.TaskName,
                ApprovalTime = o.VerifyDate.HasValue
                    ? o.VerifyDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    : string.Empty,
                ApprovalStatus = o.VerifyStatus,
                ApprovalStatusName = o.VerifyStatusName,
                ApprovalUserName = o.VerifyUserName,
                ApprovalUserId = !string.IsNullOrEmpty(o.VerifyUserId)
                    ? o.VerifyUserId
                    : (o.VerifyUserIds != null && o.VerifyUserIds.Any()
                        ? string.Join(",", o.VerifyUserIds)
                        : string.Empty)
            })
            .ToList();

        return result;
    }
    public ResultAndPage<BpmnConfVo> SelectOutSideFormCodePageList(PageDto pageDto, BpmnConfVo vo) {
        Page<BpmnConfVo> page = PageUtils.GetPageByPageDto<BpmnConfVo>(pageDto);
        List<BpmnConfVo> bpmnConfVos = _repository.SelectOutSideFormCodePageList();
            
        if (bpmnConfVos==null||!bpmnConfVos.Any()) {
            return PageUtils.GetResultAndPage(page);
        }
        page.Records=bpmnConfVos;
        return PageUtils.GetResultAndPage(page);
    }

    public void ProcessBreak(OutSideBpmAccessBusinessVo vo)
    {
        if (string.IsNullOrEmpty(vo.FormCode))
        {
            throw new AFBizException("表单编号为空，无法终止流程");
        }

        if (string.IsNullOrEmpty(vo.ProcessNumber))
        {
            throw new AFBizException("流程编号为空，无法终止流程");
        }

        var businessDataVo = new BusinessDataVo
        {
            FormCode = vo.FormCode,
            ProcessNumber = vo.ProcessNumber,
            OperationType = (int)ButtonTypeEnum.BUTTON_TYPE_ABANDONED,
            ApprovalComment = vo.ProcessBreakDesc,
            IsOutSideAccessProc = true,
            OutSideType = 1
        };

        if (!string.IsNullOrEmpty(vo.ProcessBreakUserId))
        {
            vo.UserId = vo.ProcessBreakUserId;
            var employee = GetEmployeeByUserId(vo.UserId);
            if (employee != null)
            {
                businessDataVo.ObjectMap = new Dictionary<string, object>
                {
                    { "employeeId", employee.Id },
                    { "employeeName", employee.Username }
                };
                businessDataVo.StartUserId = employee.Id.ToString();
            }
        }

        _buttonOperationService.ButtonsOperationTransactional(businessDataVo);
    }
    private Employee GetEmployeeByUserId(String userName) {
        Dictionary<String, String> stringStringMap = _bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(new List<string>(){userName});
        if(!stringStringMap.Any()){
            return null;
        }
        Employee employee=new Employee();
        employee.Id=userName;
        stringStringMap.TryGetValue(userName,out string? uname);
        employee.Username = uname??"";
        return employee;
    }

    public List<OutSideBpmAccessProcessRecordVo> OutSideProcessRecord(String processNumber) {
        return GetProcessRecord(processNumber);
    }
}