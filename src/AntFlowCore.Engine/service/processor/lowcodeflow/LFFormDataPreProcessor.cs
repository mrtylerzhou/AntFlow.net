using System.Text.Json;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Engine.service.processor.lowcodeflow;

public class LFFormDataPreProcessor : IAntFlowOrderPreProcessor<BpmnConfVo>
{
    private readonly IBpmnConfLfFormdataService _lfFormdataService;
    private readonly IBpmnConfLfFormdataFieldService _lfFormdataFieldService;
    private readonly IBpmnConfLfFormdataRepository _lfFormdataRepository;

    public LFFormDataPreProcessor(
        IBpmnConfLfFormdataService lfFormdataService,
        IBpmnConfLfFormdataFieldService lfFormdataFieldService,
        IBpmnConfLfFormdataRepository lfFormdataRepository)
    {
        _lfFormdataService = lfFormdataService;
        _lfFormdataFieldService = lfFormdataFieldService;
        _lfFormdataRepository = lfFormdataRepository;
    }

    public void PreWriteProcess(BpmnConfVo confVo)
    {
        if (confVo == null) return;

        var isLowCodeFlow = confVo.IsLowCodeFlow == 1;
        if (!isLowCodeFlow) return;

        // 外部表单模式: 表单由独立表单管理模块维护,此处不保存内联表单数据
        if (BpmnConfFlagsEnum.HasFlag(confVo.ExtraFlags, BpmnConfFlagsEnum.USE_EXTERNAL_FORM))
        {
            return;
        }

        // 内联表单模式: 保存表单数据
        var confId = confVo.Id;
        var lfForm = confVo.LfFormData;

        var lfFormdata = new BpmnConfLfFormdata
        {
            BpmnConfId = confId,
            Formdata = lfForm,
            CreateUser = SecurityUtils.GetLogInEmpName()
        };
        _lfFormdataService._repository.Add(lfFormdata);
        confVo.LfFormDataId = lfFormdata.Id;

        // 使用共享的 widget 解析器提取字段元数据
        List<BpmnConfLfFormdataField> formdataFields = LfFormWidgetParser.ParseFields(lfForm, confId, lfFormdata.Id);
        _lfFormdataFieldService._repository.AddRange(formdataFields);
    }

    public void PreReadProcess(BpmnConfVo confVo)
    {
        if (confVo == null) return;

        var isLowCodeFlow = confVo.IsLowCodeFlow == 1;
        if (!isLowCodeFlow) return;

        // 外部表单模式: 按 CSV 加载引用的表单版本(含已软删,保证运行中流程可读)
        if (BpmnConfFlagsEnum.HasFlag(confVo.ExtraFlags, BpmnConfFlagsEnum.USE_EXTERNAL_FORM))
        {
            string lfFormdataIds = confVo.LfFormdataIds;
            if (string.IsNullOrEmpty(lfFormdataIds))
            {
                throw new AFBizException($"external form mode but lf_formdata_ids is empty, confId:{confVo.Id}");
            }

            List<long> ids = lfFormdataIds.Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(long.Parse)
                .ToList();

            List<BpmnConfLfFormdata> forms = _lfFormdataRepository.ListByIdsIgnoreDeleted(ids);
            if (forms == null || !forms.Any())
            {
                throw new AFBizException($"can not get external forms by ids:{lfFormdataIds}");
            }

            confVo.LfFormdataList = forms;
            return;
        }

        // 内联表单模式: 兼容旧逻辑,加载单个表单
        var confId = confVo.Id;

        BpmnConfConfigJson? confConfig = JsonConfUtil.ParseConfConfig(confVo.ConfConfigJson);
        if (!string.IsNullOrWhiteSpace(confConfig?.LowCodeFormConfig?.Formdata))
        {
            confVo.LfFormData = confConfig.LowCodeFormConfig.Formdata;
            return;
        }

        var bpmnConfLfFormdataList = _lfFormdataService.ListByConfId(confId);
        if (bpmnConfLfFormdataList == null || !bpmnConfLfFormdataList.Any())
        {
            throw new AFBizException($"Cannot get low-code flow formdata by confId: {confId}");
        }

        var lfFormdata = bpmnConfLfFormdataList.First();
        confVo.LfFormData = lfFormdata.Formdata;
        confVo.LfFormDataId = lfFormdata.Id;
        // 同时填充 lfFormdataList,供前端统一渲染多tab表单视图
        confVo.LfFormdataList = bpmnConfLfFormdataList;
    }

    public int Order()
    {
        return 0;
    }
}
