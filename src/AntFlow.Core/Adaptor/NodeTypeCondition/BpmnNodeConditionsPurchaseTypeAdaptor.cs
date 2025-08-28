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
                    name = "̨ʽ��";
                    break;
                case 2:
                    name = "�ʼǱ�";
                    break;
                case 3:
                    name = "һ���";
                    break;
            }

            vo.Name = name;
            vos.Add(vo);
        }

        bpmnNodeConditionsConfBaseVo.PurchaseTypeList = vos;
    }
}