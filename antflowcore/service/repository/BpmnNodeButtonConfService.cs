using antflowcore.constant.enus;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.service.interf.repository;
using antflowcore.util.Extension;
using antflowcore.vo;

namespace antflowcore.service.repository;

public class BpmnNodeButtonConfService: AFBaseCurdRepositoryService<BpmnNodeButtonConf>,IBpmnNodeButtonConfService
{
    public BpmnNodeButtonConfService(IFreeSql freeSql) : base(freeSql)
    {
    }

    public void EditButtons(BpmnNodeVo bpmnNodeVo, long bpmnNodeId)
    {
        // Delete the old configs
        this.Frsql.DeleteDict(new Dictionary<string, object>
        {
            {"bpmn_node_id",bpmnNodeId}
        });

        // To check whether the start node is started by resubmitting the process
        bool isHaveCxtjButton = false;

        var buttons = bpmnNodeVo.Buttons;
        if (buttons == null || !buttons.StartPage.Any() && !buttons.ApprovalPage.Any())
        {
            buttons = new BpmnNodeButtonConfBaseVo
            {
                StartPage = new List<int>(),
                ApprovalPage = new List<int> { 2 }
            };
            // return; // todo for easy testing purposes
        }

        // Start page buttons
        List<int> startPageButtons = buttons.StartPage;
        if (startPageButtons != null && startPageButtons.Any())
        {
            baseRepo.Insert(GetBpmnNodeButtonConfs(bpmnNodeId,startPageButtons, ButtonPageTypeEnum.INITIATE));
        }

        // Approval page buttons
        List<int> approvalPageButtons = buttons.ApprovalPage;
        if (approvalPageButtons != null && approvalPageButtons.Any())
        {
            baseRepo.Insert(GetBpmnNodeButtonConfs(bpmnNodeId, approvalPageButtons, ButtonPageTypeEnum.AUDIT));
            // Check whether the approval page buttons contain the resubmit button
            if (approvalPageButtons.Contains((int)ButtonTypeEnum.BUTTON_TYPE_SUBMIT))
            {
                isHaveCxtjButton = true;
            }
        }
        List<int> viewPageButtons = buttons.ViewPage;
        if(!viewPageButtons.IsEmpty()){
            baseRepo.Insert(GetBpmnNodeButtonConfs(bpmnNodeId,viewPageButtons,ButtonPageTypeEnum.TOVIEW));
        }
        // If the initiator node and the approval page buttons do not contain the resubmit button, configure the default resubmit button
        if (bpmnNodeVo.NodeType == (int)NodeTypeEnum.NODE_TYPE_START && !isHaveCxtjButton)
        {
            // Resubmit button on the approval page
            baseRepo.Insert(new BpmnNodeButtonConf
            {
                BpmnNodeId = bpmnNodeId,
                ButtonPageType =(int)ButtonTypeEnum.BUTTON_TYPE_SUBMIT,
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
        List<BpmnNodeButtonConf> bpmnNodeButtonConfs = this.Frsql
            .Select<BpmnConf,BpmnNode,BpmnNodeButtonConf>()
            .InnerJoin((a,b,c)=>a.Id==b.ConfId)
            .InnerJoin((a,b,c)=>b.Id==c.BpmnNodeId&& c.ButtonPageType==(int)ButtonPageTypeEnum.INITIATE)
            .Where((a,b,c)=>a.BpmnCode==version)
            .ToList<BpmnNodeButtonConf>();
        return bpmnNodeButtonConfs;
    }
}