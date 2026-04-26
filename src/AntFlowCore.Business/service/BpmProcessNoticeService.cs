using AntFlowCore.Base.entity;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

public class BpmProcessNoticeService : IBpmProcessNoticeService
{
    private readonly IBpmnTemplateService _bpmnTemplateService;

    public BpmProcessNoticeService(IBpmProcessNoticeRepository repository, IBpmnTemplateService bpmnTemplateService)
    {
        _repository = repository;
        _bpmnTemplateService = bpmnTemplateService;
    }

    public IBpmProcessNoticeRepository _repository { get; }

    public List<BpmProcessNotice> ProcessNoticeList(string processKey)
    {
        return _repository.Find(a => a.ProcessKey == processKey);
    }

    public void SaveProcessNotice(BpmProcessDeptVo vo)
    {
        String processKey = vo.ProcessKey;
        List<int> notifyTypeIds = vo.NotifyTypeIds;

        List<BpmnTemplateVo> templateVos = vo.TemplateVos;
        if (!templateVos.IsEmpty())
        {
            if (notifyTypeIds.IsEmpty())
            {
                List<int> list = templateVos.SelectMany(a => a.MessageSendTypeList).Select(a => Convert.ToInt32(a.Id)).Distinct().ToList();
                notifyTypeIds = list;
            }

            BpmnConfVo confVo = new BpmnConfVo();
            confVo.FormCode = processKey;
            confVo.TemplateVos = templateVos;

            _bpmnTemplateService.EditBpmnTemplate(confVo, 0);
        }
        if (!notifyTypeIds.IsEmpty())
        {
            _repository.DeleteByProcessKey(processKey);
            List<BpmProcessNotice> notices = notifyTypeIds.Select(notifyTypeId => new BpmProcessNotice()
            {
                ProcessKey = processKey,
                Type = notifyTypeId
            }).ToList();
            _repository.AddRange(notices);
        }
    }

    public IDictionary<String, List<BpmProcessNotice>> ProcessNoticeMap(List<string> formCodes)
    {
        List<BpmProcessNotice> bpmProcessNotices = _repository.Find(a => formCodes.Contains(a.ProcessKey));
        Dictionary<string, List<BpmProcessNotice>> grouped = bpmProcessNotices
            .GroupBy(x => x.ProcessKey)
            .ToDictionary(g => g.Key, g => g.ToList());

        return grouped;
    }
}
