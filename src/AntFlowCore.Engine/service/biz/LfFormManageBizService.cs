using System.Text.RegularExpressions;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Engine.service.processor.lowcodeflow;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Engine.service.biz;

/// <summary>
/// 独立表单管理业务实现
/// </summary>
public class LfFormManageBizService : ILfFormManageBizService
{
    private const string FORM_CODE_PREFIX = "LFFM";
    private const int FORM_CODE_SEQ_LEN = 5;
    private static readonly string FORM_CODE_FORMAT = "D" + FORM_CODE_SEQ_LEN;
    private static readonly Regex FORM_CODE_PATTERN = new Regex(@".*-([0-9]{" + FORM_CODE_SEQ_LEN + "})", RegexOptions.Compiled);

    private readonly IBpmnConfLfFormdataService _lfFormdataService;
    private readonly IBpmnConfLfFormdataFieldService _lfFormdataFieldService;
    private readonly IBpmnConfLfFormdataRepository _lfFormdataRepository;
    private readonly IBpmnConfRepository _bpmnConfRepository;

    public LfFormManageBizService(
        IBpmnConfLfFormdataService lfFormdataService,
        IBpmnConfLfFormdataFieldService lfFormdataFieldService,
        IBpmnConfLfFormdataRepository lfFormdataRepository,
        IBpmnConfRepository bpmnConfRepository)
    {
        _lfFormdataService = lfFormdataService;
        _lfFormdataFieldService = lfFormdataFieldService;
        _lfFormdataRepository = lfFormdataRepository;
        _bpmnConfRepository = bpmnConfRepository;
    }

    public ResultAndPage<LfFormManageVo> ListPage(PageDto pageDto, LfFormManageVo vo)
    {
        Page<LfFormManageVo> page = PageUtils.GetPageByPageDto<LfFormManageVo>(pageDto);
        List<LfFormManageVo> records = _lfFormdataRepository.ListEffectiveFormPage(page, vo);
        page.Records = records;
        return PageUtils.GetResultAndPage(page);
    }

    public LfFormManageVo GetById(long id)
    {
        BpmnConfLfFormdata? formdata = _lfFormdataService._repository.GetById(id);
        if (formdata == null)
        {
            throw new AFBizException("表单不存在或已删除");
        }

        return new LfFormManageVo
        {
            Id = formdata.Id,
            FormCode = formdata.FormCode,
            FormName = formdata.FormName,
            Formdata = formdata.Formdata,
            EffectiveStatus = formdata.EffectiveStatus,
            CreateUser = formdata.CreateUser,
            CreateTime = formdata.CreateTime,
            UpdateUser = formdata.UpdateUser,
            UpdateTime = formdata.UpdateTime,
        };
    }

    public long Save(LfFormManageVo vo)
    {
        if (string.IsNullOrEmpty(vo.FormName))
        {
            throw new AFBizException("表单名称不能为空");
        }
        if (string.IsNullOrEmpty(vo.Formdata))
        {
            throw new AFBizException("表单内容不能为空");
        }

        string currentUser = SecurityUtils.GetLogInEmpName();
        string? formCode = vo.FormCode;

        bool isNewFamily = string.IsNullOrEmpty(formCode);
        if (isNewFamily)
        {
            formCode = GenerateNewFormCode();
        }

        // 新建家族首版本默认生效; 编辑产生的新版本默认不生效,由用户在历史版本中手动点击生效
        var formdata = new BpmnConfLfFormdata
        {
            BpmnConfId = null,
            FormCode = formCode,
            FormName = vo.FormName,
            Formdata = vo.Formdata,
            EffectiveStatus = isNewFamily ? 1 : 0,
            CreateUser = currentUser,
        };
        _lfFormdataService._repository.Add(formdata);

        // 同步字段元数据
        List<BpmnConfLfFormdataField> fields = LfFormWidgetParser.ParseFields(vo.Formdata, null, formdata.Id);
        _lfFormdataFieldService._repository.AddRange(fields);

        return formdata.Id;
    }

    public void Delete(long id)
    {
        BpmnConfLfFormdata? formdata = _lfFormdataService._repository.GetById(id);
        if (formdata == null)
        {
            throw new AFBizException("表单不存在或已删除");
        }

        // 删除保护：被生效流程引用时拒绝
        int refCount = _bpmnConfRepository.CountEffectiveConfReferencingFormdata(id);
        if (refCount > 0)
        {
            throw new AFBizException($"该表单版本已被{refCount}个生效流程引用，请先解除引用后再删除");
        }

        // 软删除：设置 is_del = 1
        formdata.IsDel = 1;
        _lfFormdataService._repository.Update(formdata);
    }

    public List<LfFormManageVo> ListHistory(string formCode)
    {
        if (string.IsNullOrEmpty(formCode))
        {
            throw new AFBizException("formCode不能为空");
        }
        return _lfFormdataRepository.ListVersionsByFormCode(formCode);
    }

    public void Effective(long id)
    {
        BpmnConfLfFormdata? formdata = _lfFormdataService._repository.GetById(id);
        if (formdata == null)
        {
            throw new AFBizException("表单不存在或已删除");
        }

        string formCode = formdata.FormCode;

        // 同 formCode 的其他生效版本置为非生效
        List<BpmnConfLfFormdata> effectiveSiblings = _lfFormdataService._repository
            .Find(a => a.FormCode == formCode && a.EffectiveStatus == 1 && a.Id != id);
        foreach (var sibling in effectiveSiblings)
        {
            sibling.EffectiveStatus = 0;
        }
        if (effectiveSiblings.Any())
        {
            _lfFormdataService._repository.UpdateRange(effectiveSiblings);
        }

        // 当前版本置为生效
        formdata.EffectiveStatus = 1;
        _lfFormdataService._repository.Update(formdata);
    }

    public List<LfFormManageVo> ListEffectiveForSelect()
    {
        return _lfFormdataRepository.ListAllEffectiveForms();
    }

    /// <summary>
    /// 生成新的家族 formCode：LFFM-00001, LFFM-00002, ...
    /// </summary>
    private string GenerateNewFormCode()
    {
        string prefix = FORM_CODE_PREFIX + "-";
        string? maxFormCode = _lfFormdataRepository.GetMaxFormCode(prefix);
        int nextSeq = 1;
        if (!string.IsNullOrEmpty(maxFormCode))
        {
            Match matcher = FORM_CODE_PATTERN.Match(maxFormCode);
            if (matcher.Success)
            {
                nextSeq = int.Parse(matcher.Groups[1].Value) + 1;
            }
        }
        return prefix + nextSeq.ToString(FORM_CODE_FORMAT);
    }
}
