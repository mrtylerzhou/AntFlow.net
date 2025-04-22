using antflowcore.adaptor.processoperation;
using antflowcore.aop;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.factory.tagparser;

public class FormOperationTagParser: TagParser<IProcessOperationAdaptor,BusinessDataVo>
{
    private static Dictionary<Type, IProcessOperationAdaptor> dynamicProxyMap =
        new Dictionary<Type, IProcessOperationAdaptor>();
    public IProcessOperationAdaptor ParseTag(BusinessDataVo data)
    {
        
        if(data==null){
            throw new AFBizException("provided data to find a processing method is null");
        }
        int? operationType = data.OperationType;
        bool? isOutSideAccessProc = data.IsOutSideAccessProc;
        if(operationType==null){
            throw new AFBizException("provided data has no property of operationType!");
        }
        ProcessOperationEnum? poEnum = ProcessOperationEnumExtensions.GetEnumByCode(operationType);
        if(poEnum==null){
            throw new AFBizException("can not find a processing method by providing data with your given operationType of"+operationType);
        }

        IEnumerable<IProcessOperationAdaptor> processOperationAdaptors = ServiceProviderUtils.GetServices<IProcessOperationAdaptor>();
        foreach (IProcessOperationAdaptor processOperationAdaptor in processOperationAdaptors)
        {
            IProcessOperationAdaptor processOperationProxy = null;
            Type adaptorType = processOperationAdaptor.GetType();
            if (dynamicProxyMap.TryGetValue(adaptorType, out var adaptor))
            {
                processOperationProxy = adaptor;
            }
            else
            {
                processOperationProxy= BpmnSendMessageAspect<IProcessOperationAdaptor>.Create(processOperationAdaptor);
                dynamicProxyMap[adaptorType] = processOperationProxy;
            }
           
            if(!isOutSideAccessProc.HasValue||isOutSideAccessProc.Value==false){
                if(processOperationAdaptor.IsSupportBusinessObject(poEnum)){
                    return processOperationProxy;
                }
            }else{
                int? outSideType = data.OutSideType;
              
                String outSideMarker=outSideType==0?"outSide":"outSideAccess";
                if(processOperationAdaptor.IsSupportBusinessObject(outSideMarker,poEnum)){
                    return processOperationProxy;
                }
            }
        }

        return null;
    }
}