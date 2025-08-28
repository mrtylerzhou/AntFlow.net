using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.NodeTypeCondition;

public class BpmnNodeConditionsPurchaseTypeAdaptor : IBpmnNodeConditionsAdaptor
{
    public void SetConditionsResps(BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo)
    {
        List<BaseIdTranStruVo> vos = new();

        for (int i = 1; i < 4; i++)
        {
            BaseIdTranStruVo vo = new();
            vo.Id = i.ToString();
            string name = "";
            switch (i)
            {
                case 1:
                    name = "台式机";
                    break;
                case 2:
                    name = "笔记本";
                    break;
                case 3:
                    name = "一体机";
                    break;
            }

            vo.Name = name;
            vos.Add(vo);
        }

        bpmnNodeConditionsConfBaseVo.PurchaseTypeList = vos;
    }
}