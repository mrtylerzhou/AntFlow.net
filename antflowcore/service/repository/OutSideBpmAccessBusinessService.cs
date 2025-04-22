using antflowcore.constant.enums;
using antflowcore.dto;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Entity;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class OutSideBpmAccessBusinessService : AFBaseCurdRepositoryService<OutSideBpmAccessBusiness>
{
    public OutSideBpmAccessBusinessService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void UpdateById(OutSideBpmAccessBusiness outSideBpmAccessBusiness)
    {
        this.baseRepo.Update(outSideBpmAccessBusiness);
    }

    public OutSideBpmAccessRespVo AccessBusinessStart(OutSideBpmAccessBusinessVo vo)
    {
        OutSideBpmAccessBusiness outSideBpmAccessBusiness = this.baseRepo
            .Where(a => a.Id == vo.Id).ToOne();

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
            BpmnConfService bpmnConfService = ServiceProviderUtils.GetService<BpmnConfService>();
            BpmnConf effectiveConfByFormCode = bpmnConfService
                .baseRepo
                .Where(a => a.FormCode == formCode && a.EffectiveStatus == 1 && a.IsDel == 0)
                .ToOne();

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

            this.baseRepo.Insert(outSideBpmAccessBusiness);
        }

        // 组装业务数据对象
        BusinessDataVo businessDataVo = vo.IsLowCodeFlow == true ? new UDLFApplyVo() : new BusinessDataVo();

        if (businessDataVo is UDLFApplyVo udlf)
        {
            udlf.lfFields = vo.LfFields;
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
        ButtonOperationService ButtonOperationService = ServiceProviderUtils.GetService<ButtonOperationService>();
        ButtonOperationService.ButtonsOperationTransactional(businessDataVo);

        OutSideBpmAccessBusiness result = this.baseRepo
            .Where(a => a.Id == outSideBpmAccessBusiness.Id).ToOne();

        return new OutSideBpmAccessRespVo
        {
            ProcessNumber = result.ProcessNumber,
            BusinessId = result.Id.ToString(),
            ProcessRecord = GetProcessRecord(result.ProcessNumber)
        };
    }

    private List<OutSideBpmAccessProcessRecordVo> GetProcessRecord(string processNumber)
    {
        BpmVerifyInfoBizService bpmVerifyInfoBizService = ServiceProviderUtils.GetService<BpmVerifyInfoBizService>();
        var bpmVerifyInfoVos = bpmVerifyInfoBizService.GetBpmVerifyInfoVos(processNumber, false);

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
    public ResultAndPage<BpmnConfVo> SelectOutSideFormCodePageList(PageDto pageDto, BpmnConfVo vo)
    {
        Page<BpmnConfVo> page = PageUtils.GetPageByPageDto<BpmnConfVo>(pageDto);
        BpmnConfService bpmnConfService = ServiceProviderUtils.GetService<BpmnConfService>();
        List<BpmnConfVo> bpmnConfVos = this.Frsql
            .Select<BpmnConf, BpmProcessAppApplication>()
            .InnerJoin((a, b) => a.FormCode == b.ProcessKey)
            .Where((a, b) => a.EffectiveStatus == 1 && a.IsOutSideProcess == 1)
            .OrderByDescending((a, b) => a.CreateTime)
            .ToList<BpmnConfVo>((a, b) => new BpmnConfVo()
            {
                BpmnCode = a.BpmnCode,
                FormCode = a.FormCode,
                BpmnName = a.BpmnName,
                DeduplicationType = a.DeduplicationType,
                EffectiveStatus = a.EffectiveStatus,
                BusinessPartyId = a.BusinessPartyId,
                ApplicationId = b.Id,
                UpdateTime = a.UpdateTime,
                Remark = a.Remark
            });

        if (bpmnConfVos == null || !bpmnConfVos.Any())
        {
            return PageUtils.GetResultAndPage(page);
        }
        page.Records = bpmnConfVos;
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

        ButtonOperationService buttonOperationService = ServiceProviderUtils.GetService<ButtonOperationService>();
        buttonOperationService.ButtonsOperationTransactional(businessDataVo);
    }
    private Employee GetEmployeeByUserId(String userName)
    {
        BpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService = ServiceProviderUtils.GetService<BpmnEmployeeInfoProviderService>();
        Dictionary<String, String> stringStringMap = bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(new List<string>() { userName });
        if (!stringStringMap.Any())
        {
            return null;
        }
        Employee employee = new Employee();
        employee.Id = userName;
        stringStringMap.TryGetValue(userName, out string? uname);
        employee.Username = uname ?? "";
        return employee;
    }

    public List<OutSideBpmAccessProcessRecordVo> OutSideProcessRecord(String processNumber)
    {
        return GetProcessRecord(processNumber);
    }
}