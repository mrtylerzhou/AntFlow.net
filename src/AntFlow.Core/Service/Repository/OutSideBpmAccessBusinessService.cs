using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Repository;

public class OutSideBpmAccessBusinessService : AFBaseCurdRepositoryService<OutSideBpmAccessBusiness>,
    IOutSideBpmAccessBusinessService
{
    public OutSideBpmAccessBusinessService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void UpdateById(OutSideBpmAccessBusiness outSideBpmAccessBusiness)
    {
        baseRepo.Update(outSideBpmAccessBusiness);
    }

    public OutSideBpmAccessRespVo AccessBusinessStart(OutSideBpmAccessBusinessVo vo)
    {
        OutSideBpmAccessBusiness outSideBpmAccessBusiness = baseRepo
            .Where(a => a.Id == vo.Id).ToOne();

        if (outSideBpmAccessBusiness != null)
        {
            // 更新现有业务记录
            vo.CopyPropertiesTo(outSideBpmAccessBusiness);
            outSideBpmAccessBusiness.UpdateUser = SecurityUtils.GetLogInEmpId();
            outSideBpmAccessBusiness.UpdateTime = DateTime.Now;
            UpdateById(outSideBpmAccessBusiness);
        }
        else
        {
            string? formCode = vo.FormCode;
            BpmnConfService bpmnConfService = ServiceProviderUtils.GetService<BpmnConfService>();
            BpmnConf effectiveConfByFormCode = bpmnConfService
                .baseRepo
                .Where(a => a.FormCode == formCode && a.EffectiveStatus == 1 && a.IsDel == 0)
                .ToOne();


            if (effectiveConfByFormCode == null)
            {
                throw new AFBizException($"根据表单编码{formCode}查询不到有效的流程配置，请联系管理员");
            }

            vo.BpmnConfId = effectiveConfByFormCode.Id;
            outSideBpmAccessBusiness = vo.MapToEntity();


            if (vo.TemplateMarks != null && vo.TemplateMarks.Any())
            {
                outSideBpmAccessBusiness.TemplateMark = string.Join(",", vo.TemplateMarks);
            }

            outSideBpmAccessBusiness.BusinessPartyId = effectiveConfByFormCode.BusinessPartyId ?? 0;
            string? empId = SecurityUtils.GetLogInEmpIdSafe();
            string? empName = SecurityUtils.GetLogInEmpNameSafe();
            DateTime now = DateTime.Now;

            outSideBpmAccessBusiness.CreateUser = empId;
            outSideBpmAccessBusiness.CreateTime = now;
            outSideBpmAccessBusiness.UpdateUser = empId;
            outSideBpmAccessBusiness.UpdateTime = now;

            baseRepo.Insert(outSideBpmAccessBusiness);
        }

        // 构建业务数据对象
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
            throw new AFBizException("用户ID不能为空，请检查登录状态");
        }

        businessDataVo.StartUserId = SecurityUtils.GetLogInEmpIdSafe();
        businessDataVo.StartUserName = SecurityUtils.GetLogInEmpNameSafe();
        businessDataVo.EmpId = SecurityUtils.GetLogInEmpIdSafe();
        businessDataVo.SubmitUser = SecurityUtils.GetLogInEmpNameSafe();
        businessDataVo.EmbedNodes = vo.EmbedNodes;
        ButtonOperationService ButtonOperationService = ServiceProviderUtils.GetService<ButtonOperationService>();
        ButtonOperationService.ButtonsOperationTransactional(businessDataVo);

        OutSideBpmAccessBusiness result = baseRepo
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
        List<BpmVerifyInfoVo>? bpmVerifyInfoVos = bpmVerifyInfoBizService.GetBpmVerifyInfoVos(processNumber, false);

        List<OutSideBpmAccessProcessRecordVo>? result = bpmVerifyInfoVos
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
                    : o.VerifyUserIds != null && o.VerifyUserIds.Any()
                        ? string.Join(",", o.VerifyUserIds)
                        : string.Empty
            })
            .ToList();

        return result;
    }

    public ResultAndPage<BpmnConfVo> SelectOutSideFormCodePageList(PageDto pageDto, BpmnConfVo vo)
    {
        Page<BpmnConfVo> page = PageUtils.GetPageByPageDto<BpmnConfVo>(pageDto);
        BpmnConfService bpmnConfService = ServiceProviderUtils.GetService<BpmnConfService>();
        List<BpmnConfVo> bpmnConfVos = Frsql
            .Select<BpmnConf, BpmProcessAppApplication>()
            .InnerJoin((a, b) => a.FormCode == b.ProcessKey)
            .Where((a, b) => a.EffectiveStatus == 1 && a.IsOutSideProcess == 1)
            .OrderByDescending((a, b) => a.CreateTime)
            .ToList<BpmnConfVo>((a, b) => new BpmnConfVo
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
            throw new AFBizException("表单编码不能为空");
        }

        if (string.IsNullOrEmpty(vo.ProcessNumber))
        {
            throw new AFBizException("流程编号不能为空");
        }

        BusinessDataVo? businessDataVo = new()
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
            Employee? employee = GetEmployeeByUserId(vo.UserId);
            if (employee != null)
            {
                businessDataVo.ObjectMap = new Dictionary<string, object>
                {
                    { "employeeId", employee.Id }, { "employeeName", employee.Username }
                };
                businessDataVo.StartUserId = employee.Id;
            }
        }

        ButtonOperationService buttonOperationService = ServiceProviderUtils.GetService<ButtonOperationService>();
        buttonOperationService.ButtonsOperationTransactional(businessDataVo);
    }

    private Employee GetEmployeeByUserId(string userName)
    {
        BpmnEmployeeInfoProviderService bpmnEmployeeInfoProviderService =
            ServiceProviderUtils.GetService<BpmnEmployeeInfoProviderService>();
        Dictionary<string, string> stringStringMap =
            bpmnEmployeeInfoProviderService.ProvideEmployeeInfo(new List<string> { userName });
        if (!stringStringMap.Any())
        {
            return null;
        }

        Employee employee = new();
        employee.Id = userName;
        stringStringMap.TryGetValue(userName, out string? uname);
        employee.Username = uname ?? "";
        return employee;
    }

    public List<OutSideBpmAccessProcessRecordVo> OutSideProcessRecord(string processNumber)
    {
        return GetProcessRecord(processNumber);
    }
}