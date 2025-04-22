using antflowcore.vo;

namespace antflowcore.adaptor.nodetypecondition;

public class BpmnNodeConditionsAccountTypeAdaptor: IBpmnNodeConditionsAdaptor
{
    public void SetConditionsResps(BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo)
    {
        //todo for demo only
        List<BaseIdTranStruVo>vos=new List<BaseIdTranStruVo>();

        for (int i = 1; i < 4; i++) {
            BaseIdTranStruVo vo=new BaseIdTranStruVo();
            vo.Id=i.ToString();
            String name= "";
            switch (i){
                case 1:name="百度云";
                    break;
                case 2:name="腾讯云";
                    break;
                case 3:name="中通云";
                    break;
            }
            vo.Name=(name);
            vos.Add(vo);
        }
        bpmnNodeConditionsConfBaseVo.AccountTypeList=vos;
    }
}