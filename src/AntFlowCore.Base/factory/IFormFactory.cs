using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.factory;

public interface IFormFactory
{
    IFormOperationAdaptor<BusinessDataVo> GetFormAdaptor(string formCode);
    IFormOperationAdaptor<BusinessDataVo> GetFormAdaptor(BusinessDataVo vo);
    BusinessDataVo DataFormConversion(string parameters, string formCode);
}
