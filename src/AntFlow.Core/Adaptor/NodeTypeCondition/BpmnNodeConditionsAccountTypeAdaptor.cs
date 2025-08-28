using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.NodeTypeCondition;

public class BpmnNodeConditionsAccountTypeAdaptor : IBpmnNodeConditionsAdaptor
{
    public void SetConditionsResps(BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo)
    {
        //todo for demo only
        List<BaseIdTranStruVo> vos = new();

        for (int i = 1; i < 4; i++)
        {
            BaseIdTranStruVo vo = new();
            vo.Id = i.ToString();
            string name = "";
            switch (i)
            {
                case 1:
                    name = "�ٶ���";
                    break;
                case 2:
                    name = "��Ѷ��";
                    break;
                case 3:
                    name = "��ͨ��";
                    break;
            }

            vo.Name = name;
            vos.Add(vo);
        }

        bpmnNodeConditionsConfBaseVo.AccountTypeList = vos;
    }
}