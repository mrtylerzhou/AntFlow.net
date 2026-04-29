using AntFlowCore.Abstraction.Orm.repository;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Persist.repository;

public class SsBpmnNodeButtonConfRepository : RepositoryBase<BpmnNodeButtonConf>, IBpmnNodeButtonConfRepository
{
    public SsBpmnNodeButtonConfRepository(AntFlowOrmContext context) : base(context)
    {
    }

    public void EditButtons(BpmnNodeVo bpmnNodeVo, long bpmnNodeId)
    {
        Db.Deleteable<BpmnNodeButtonConf>()
            .Where(a => a.BpmnNodeId == bpmnNodeId)
            .ExecuteCommand();

        bool isHaveCxtjButton = false;

        var buttons = bpmnNodeVo.Buttons;
        if (buttons == null || !buttons.StartPage.Any() && !buttons.ApprovalPage.Any())
        {
            buttons = new BpmnNodeButtonConfBaseVo
            {
                StartPage = new List<int>(),
                ApprovalPage = new List<int> { 2 }
            };
        }

        List<int> startPageButtons = buttons.StartPage;
        if (startPageButtons != null && startPageButtons.Any())
        {
            AddRange(GetBpmnNodeButtonConfs(bpmnNodeId, startPageButtons, ButtonPageTypeEnum.INITIATE));
        }

        List<int> approvalPageButtons = buttons.ApprovalPage;
        if (approvalPageButtons != null && approvalPageButtons.Any())
        {
            AddRange(GetBpmnNodeButtonConfs(bpmnNodeId, approvalPageButtons, ButtonPageTypeEnum.AUDIT));
            if (approvalPageButtons.Contains((int)ButtonTypeEnum.BUTTON_TYPE_SUBMIT))
            {
                isHaveCxtjButton = true;
            }
        }
        List<int> viewPageButtons = buttons.ViewPage;
        if (!viewPageButtons.IsEmpty())
        {
            AddRange(GetBpmnNodeButtonConfs(bpmnNodeId, viewPageButtons, ButtonPageTypeEnum.TOVIEW));
        }

        if (bpmnNodeVo.NodeType == (int)NodeTypeEnum.NODE_TYPE_START && !isHaveCxtjButton)
        {
            Add(new BpmnNodeButtonConf
            {
                BpmnNodeId = bpmnNodeId,
                ButtonPageType = (int)ButtonTypeEnum.BUTTON_TYPE_SUBMIT,
                ButtonType = (int)ButtonTypeEnum.BUTTON_TYPE_SUBMIT,
                ButtonName = ButtonTypeEnumExtensions.GetDescByCode((int)ButtonTypeEnum.BUTTON_TYPE_SUBMIT),
                CreateTime = DateTime.Now,
            });
        }
    }

    private List<BpmnNodeButtonConf> GetBpmnNodeButtonConfs(long bpmnNodeId, List<int> buttons, ButtonPageTypeEnum buttonPageTypeEnum)
    {
        return buttons
            .Distinct()
            .Select(o => new BpmnNodeButtonConf
            {
                BpmnNodeId = bpmnNodeId,
                ButtonPageType = (int)buttonPageTypeEnum,
                ButtonType = o,
                ButtonName = ButtonTypeEnumExtensions.GetDescByCode(o),
                CreateTime = DateTime.Now,
            })
            .ToList();
    }

    public List<BpmnNodeButtonConf>? QueryConfByBpmnConde(string version)
    {
        List<BpmnNodeButtonConf> bpmnNodeButtonConfs = Db.Queryable<BpmnConf>()
            .InnerJoin<BpmnNode>((a, b) => a.Id == b.ConfId)
            .InnerJoin<BpmnNodeButtonConf>((a, b, c) => b.Id == c.BpmnNodeId && c.ButtonPageType == (int)ButtonPageTypeEnum.INITIATE)
            .Where((a, b, c) => a.BpmnCode == version)
            .Select((a, b, c) => c)
            .ToList();
        return bpmnNodeButtonConfs;
    }
}
