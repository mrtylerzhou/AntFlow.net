using antflowcore.vo;

namespace antflowcore.adaptor.nodetypecondition;

public class BpmnNodeConditionsPurchaseTypeAdaptor: IBpmnNodeConditionsAdaptor
{
    public void SetConditionsResps(BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo)
    {
        List<BaseIdTranStruVo>vos=new List<BaseIdTranStruVo>();

        for (int i = 1; i < 4; i++) {
            BaseIdTranStruVo vo=new BaseIdTranStruVo();
            vo.Id=(i.ToString());
            String name= "";
            switch (i){
                case 1:name="台式机";
                    break;
                case 2:name="笔记本";
                    break;
                case 3:name="一体机";
                    break;
            }
            vo.Name=(name);
            vos.Add(vo);
        }
        bpmnNodeConditionsConfBaseVo.PurchaseTypeList = vos;
    }
}